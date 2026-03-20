using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls hint overlay — bottom centre of screen.
/// Uses Unity's built-in Arial font (always available, no TMP setup needed).
/// Auto-creates itself at runtime via RuntimeInitializeOnLoadMethod.
/// Shows briefly on launch, then reappears when cursor is unlocked (ESC mode).
/// </summary>
public class ControlsOverlay : MonoBehaviour
{
    private const float LaunchDuration = 3.5f;
    private const float FadeSpeed      = 6f;
    private const int   FontSize       = 13;

    private CanvasGroup _canvasGroup;
    private bool        _cursorWasLocked;
    private bool        _launchHintComplete;
    private Font        _arial;

    private static readonly (string key, string desc)[] Controls =
    {
        ("B",      "Browse Video"),
        ("SPACE",  "Play / Pause"),
        ("← →",   "Skip 3s"),
        ("SCROLL", "Zoom"),
        ("CLICK",  "Look Around"),
    };

    private static readonly Color PanelBg   = new Color(0.06f, 0.06f, 0.06f, 0.90f);
    private static readonly Color BadgeBg   = new Color(0.22f, 0.22f, 0.24f, 1.00f);
    private static readonly Color KeyCol    = Color.white;
    private static readonly Color DescCol   = new Color(0.72f, 0.72f, 0.75f, 1.00f);
    private static readonly Color SepCol    = new Color(1.00f, 1.00f, 1.00f, 0.12f);

    // -------------------------------------------------------------------------

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoCreate()
    {
        var go = new GameObject("ControlsOverlay");
        DontDestroyOnLoad(go);
        go.AddComponent<ControlsOverlay>();
    }

    // -------------------------------------------------------------------------

    private void Awake()
    {
        _arial = Resources.GetBuiltinResource<Font>("Arial.ttf");

        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        _canvasGroup                = gameObject.AddComponent<CanvasGroup>();
        _canvasGroup.alpha          = 0f;
        _canvasGroup.interactable   = false;
        _canvasGroup.blocksRaycasts = false;

        BuildUI();
    }

    private void Start()
    {
        _cursorWasLocked = Cursor.lockState == CursorLockMode.Locked;
        StartCoroutine(LaunchHint());
    }

    private void Update()
    {
        if (!_launchHintComplete) return;

        bool locked = Cursor.lockState == CursorLockMode.Locked;
        if (locked == _cursorWasLocked) return;

        _cursorWasLocked = locked;
        StopAllCoroutines();
        StartCoroutine(FadeTo(locked ? 0f : 1f));
    }

    // -------------------------------------------------------------------------

    private IEnumerator LaunchHint()
    {
        yield return FadeTo(1f);
        yield return new WaitForSecondsRealtime(LaunchDuration);

        _launchHintComplete = true;
        _cursorWasLocked    = Cursor.lockState == CursorLockMode.Locked;

        if (_cursorWasLocked)
            yield return FadeTo(0f);
    }

    private IEnumerator FadeTo(float target)
    {
        float start = _canvasGroup.alpha;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * FadeSpeed;
            _canvasGroup.alpha = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)));
            yield return null;
        }
        _canvasGroup.alpha = target;
    }

    // -------------------------------------------------------------------------
    // UI construction
    // -------------------------------------------------------------------------

    private void BuildUI()
    {
        var rounded = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");

        // Panel — anchored bottom-centre
        var panel   = MakeRect("Panel", transform);
        var panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin        = new Vector2(0.5f, 0f);
        panelRT.anchorMax        = new Vector2(0.5f, 0f);
        panelRT.pivot            = new Vector2(0.5f, 0f);
        panelRT.anchoredPosition = new Vector2(0f, 32f);

        var panelImg = panel.AddComponent<Image>();
        panelImg.color = PanelBg;
        if (rounded != null) { panelImg.sprite = rounded; panelImg.type = Image.Type.Sliced; }

        var panelHLG = panel.AddComponent<HorizontalLayoutGroup>();
        panelHLG.padding               = new RectOffset(28, 28, 14, 14);
        panelHLG.spacing               = 4f;
        panelHLG.childAlignment        = TextAnchor.MiddleCenter;
        panelHLG.childControlWidth     = true;
        panelHLG.childControlHeight    = true;
        panelHLG.childForceExpandWidth  = false;
        panelHLG.childForceExpandHeight = false;

        var panelCSF = panel.AddComponent<ContentSizeFitter>();
        panelCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        panelCSF.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

        for (int i = 0; i < Controls.Length; i++)
        {
            if (i > 0) AddSeparator(panel.transform);
            AddControlItem(panel.transform, Controls[i].key, Controls[i].desc, rounded);
        }
    }

    private void AddSeparator(Transform parent)
    {
        var sep = MakeRect("Sep", parent);
        sep.AddComponent<Image>().color = SepCol;
        var le = sep.AddComponent<LayoutElement>();
        le.minWidth       = 1f;
        le.preferredWidth = 1f;
        le.minHeight      = 18f;
        le.preferredHeight = 18f;
    }

    private void AddControlItem(Transform parent, string key, string desc, Sprite rounded)
    {
        // Item container
        var item    = MakeRect("Item_" + key, parent);
        var itemHLG = item.AddComponent<HorizontalLayoutGroup>();
        itemHLG.spacing               = 7f;
        itemHLG.padding               = new RectOffset(12, 12, 0, 0);
        itemHLG.childAlignment        = TextAnchor.MiddleCenter;
        itemHLG.childControlWidth     = true;
        itemHLG.childControlHeight    = true;
        itemHLG.childForceExpandWidth  = false;
        itemHLG.childForceExpandHeight = false;

        // Key badge background
        var badge    = MakeRect("Badge", item.transform);
        var badgeImg = badge.AddComponent<Image>();
        badgeImg.color = BadgeBg;
        if (rounded != null) { badgeImg.sprite = rounded; badgeImg.type = Image.Type.Sliced; }

        var badgeHLG = badge.AddComponent<HorizontalLayoutGroup>();
        badgeHLG.padding               = new RectOffset(10, 10, 5, 5);
        badgeHLG.childAlignment        = TextAnchor.MiddleCenter;
        badgeHLG.childControlWidth     = true;
        badgeHLG.childControlHeight    = true;
        badgeHLG.childForceExpandWidth  = false;
        badgeHLG.childForceExpandHeight = false;

        // Key label
        var keyGO   = MakeRect("Key", badge.transform);
        var keyText = keyGO.AddComponent<Text>();
        keyText.font      = _arial;
        keyText.text      = key;
        keyText.fontSize  = FontSize;
        keyText.fontStyle = FontStyle.Bold;
        keyText.color     = KeyCol;
        keyText.alignment = TextAnchor.MiddleCenter;
        keyText.horizontalOverflow = HorizontalWrapMode.Overflow;
        keyText.verticalOverflow   = VerticalWrapMode.Overflow;

        // Description label
        var descGO   = MakeRect("Desc", item.transform);
        var descText = descGO.AddComponent<Text>();
        descText.font      = _arial;
        descText.text      = desc;
        descText.fontSize  = FontSize;
        descText.color     = DescCol;
        descText.alignment = TextAnchor.MiddleLeft;
        descText.horizontalOverflow = HorizontalWrapMode.Overflow;
        descText.verticalOverflow   = VerticalWrapMode.Overflow;
    }

    private static GameObject MakeRect(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }
}
