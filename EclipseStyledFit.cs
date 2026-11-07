using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class EclipseStyledFit : MonoBehaviour
{
    public float referenceWidth  = 1920f;
    public float referenceHeight = 1080f;

    RectTransform rect;
    RectTransform parentRect;

    void OnEnable()   { Cache(); Apply(); }
    void OnValidate() { Cache(); Apply(); }
    void LateUpdate() { Apply(); }

    void Cache()
    {
        rect = GetComponent<RectTransform>();
        parentRect = transform.parent as RectTransform;
    }

    void Apply()
    {
        if (rect == null || parentRect == null) return;

        float sx = parentRect.rect.width  / referenceWidth;
        float sy = parentRect.rect.height / referenceHeight;
        float s  = Mathf.Min(sx, sy);

        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(referenceWidth, referenceHeight);
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = new Vector3(s, s, 1f);
    }
}
