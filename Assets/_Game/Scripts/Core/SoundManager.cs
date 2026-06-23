using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundEntry
{
    public string Key;
    public AudioClip Clip;
    [Range(0f, 1f)] public float Volume;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private List<SoundEntry> _sounds = new();

    private const int PoolSize = 20;
    private AudioSource[] _pool;
    private int _poolIndex;

    private readonly Dictionary<string, SoundEntry> _map = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        BuildPool();
        BuildMap();
    }

    public void Play(string key)
    {
        if (!_map.TryGetValue(key, out var entry))
        {
            Debug.LogWarning($"[SoundManager] Key not found: {key}");
            return;
        }

        var source = NextSource();
        source.PlayOneShot(entry.Clip, entry.Volume > 0f ? entry.Volume : 1f);
    }

    private AudioSource NextSource()
    {
        // Round-robin: en son kullanılanın üstüne yazar, busy kontrolü yok (one-shot için yeterli)
        var source = _pool[_poolIndex];
        _poolIndex = (_poolIndex + 1) % PoolSize;
        return source;
    }

    private void BuildPool()
    {
        _pool = new AudioSource[PoolSize];
        for (int i = 0; i < PoolSize; i++)
        {
            var go = new GameObject($"SoundSource_{i}");
            go.transform.SetParent(transform);
            _pool[i] = go.AddComponent<AudioSource>();
            _pool[i].playOnAwake = false;
        }
    }

    private void BuildMap()
    {
        foreach (var entry in _sounds)
        {
            if (string.IsNullOrEmpty(entry.Key) || entry.Clip == null) continue;
            _map[entry.Key] = entry;
        }
    }
}