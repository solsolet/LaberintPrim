using UnityEngine;
using UnityEngine.UI;
using TMPro;

// It reads the real screen size at runtime and positions the title
// and restart button correctly on any Android device automatically.
[RequireComponent(typeof(RectTransform))]
public class PanelAutoLayout : MonoBehaviour
{
    [Header("Children to position (assign in Inspector)")]
    public TextMeshProUGUI titleText;
    public Button          restartButton;

    [Header("Optional subtitle (e.g. final score text)")]
    public TextMeshProUGUI subtitleText;

    void Start()
    {
        Apply();
    }

    void Apply()
    {
        float sw = Screen.width;
        float sh = Screen.height;

        // --- Panel background: stretch to fill the whole screen ---
        RectTransform panelRT = GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // --- Title text ---
        // Sits at 60% from the bottom (upper half of screen)
        if (titleText != null)
        {
            RectTransform rt = titleText.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0.55f);
            rt.anchorMax = new Vector2(0.9f, 0.75f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // Font size: ~7% of screen height, clamped to a readable range
            titleText.fontSize          = Mathf.Clamp(sh * 0.07f, 40f, 120f);
            titleText.alignment         = TextAlignmentOptions.Center;
            titleText.enableAutoSizing  = false;
        }

        // --- Subtitle (score summary) ---
        if (subtitleText != null)
        {
            RectTransform rt = subtitleText.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0.42f);
            rt.anchorMax = new Vector2(0.9f, 0.54f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            subtitleText.fontSize         = Mathf.Clamp(sh * 0.045f, 28f, 80f);
            subtitleText.alignment        = TextAlignmentOptions.Center;
            subtitleText.enableAutoSizing = false;
        }

        // --- Restart button ---
        // Sits in the lower-centre of the panel
        if (restartButton != null)
        {
            RectTransform rt = restartButton.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.25f, 0.25f);
            rt.anchorMax = new Vector2(0.75f, 0.38f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // Also scale the button's label text
            TextMeshProUGUI btnLabel = restartButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnLabel != null)
            {
                btnLabel.fontSize         = Mathf.Clamp(sh * 0.04f, 24f, 70f);
                btnLabel.alignment        = TextAlignmentOptions.Center;
                btnLabel.enableAutoSizing = false;
            }
        }
    }
}