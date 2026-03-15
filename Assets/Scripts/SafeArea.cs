using UnityEngine;

// Attach this to the HUD GameObject (NOT the Canvas itself).
// It resizes the HUD RectTransform every frame to stay within
// the device's safe area, avoiding notches and home bars.
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform rect;
    private Rect lastSafeArea = Rect.zero;
    private Vector2Int lastScreenSize = Vector2Int.zero;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        Apply();
    }

    void Update()
    {
        // Re-apply if screen size or safe area changes (e.g. device rotation)
        if (Screen.safeArea != lastSafeArea ||
            Screen.width  != lastScreenSize.x ||
            Screen.height != lastScreenSize.y)
        {
            Apply();
        }
    }

    void Apply()
    {
        Rect safe = Screen.safeArea;
        lastSafeArea   = safe;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);

        // Convert safe area rectangle to anchor coordinates (0-1 range)
        Vector2 anchorMin = safe.position;
        Vector2 anchorMax = safe.position + safe.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;

        // Reset offsets so the rect fills exactly the safe area
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}