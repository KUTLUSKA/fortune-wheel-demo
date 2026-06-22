using UnityEngine;

public class SliceFactory : MonoBehaviour
{
    [SerializeField] private SliceView _slicePrefab;

    public SliceView CreateSlice(SliceDataSO data, Transform parent, float angleDeg, float radius)
    {
        SliceView view = Instantiate(_slicePrefab, parent);

        float rad = angleDeg * Mathf.Deg2Rad;
        float x = Mathf.Sin(rad) * radius;
        float y = Mathf.Cos(rad) * radius;

        RectTransform rt = view.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        rt.localRotation = Quaternion.Euler(0f, 0f, -angleDeg);

        view.Initialize(data);
        return view;
    }

    public void ClearSlices(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    private void OnValidate()
    {
        if (_slicePrefab == null)
            Debug.LogWarning("[SliceFactory] SlicePrefab is not assigned.", this);
    }
}
