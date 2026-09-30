using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Scene-local controller: no persistent menu objects survive scene changes.
[DefaultExecutionOrder(-100)]
public sealed class DungeonMenuController : MonoBehaviour
{
    public const string MainScene = "MainMenu";
    public const string GameScene = "Dungeon_PrototypeUnityAI";
    public const string GameTitle = "Cendrelith";
    private const string VolumeKey = "Dungeon.Menu.MasterVolume";
    private const string FullscreenKey = "Dungeon.Menu.Fullscreen";
    private enum Page { Closed, Home, Pause, Settings, Credits, Confirm, Error }
    private Page page;
    private Page returnPage;
    private Action confirmedAction;
    private string message;
    private bool main;
    private bool loading;
    private bool ownsPause;
    private float previousTimeScale;
    private bool previousAudioPause;
    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;
    private RectTransform root;
    private GameObject content;
    private Font font;
    private readonly List<MonoBehaviour> suspended = new List<MonoBehaviour>();
    private readonly Color ivory = new Color(0.88f, 0.84f, 0.73f);
    private readonly Color bronze = new Color(0.47f, 0.35f, 0.21f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneLoaded += SceneLoaded;
        Attach(SceneManager.GetActiveScene());
    }

    private static void SceneLoaded(Scene scene, LoadSceneMode mode) => Attach(scene);

    private static void Attach(Scene scene)
    {
        if (scene.name != MainScene && scene.name != GameScene && scene.name != "Cendrelith_Level01_Blockout") return;
        if (FindFirstObjectByType<DungeonMenuController>() != null) return;
        var obj = new GameObject("Dungeon Menus");
        SceneManager.MoveGameObjectToScene(obj, scene);
        obj.AddComponent<DungeonMenuController>();
    }

    private void Start()
    {
        main = gameObject.scene.name == MainScene;
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        AudioListener.volume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, 1f));
        if (PlayerPrefs.HasKey(FullscreenKey))
            Screen.fullScreen = PlayerPrefs.GetInt(FullscreenKey) != 0;

        var canvasObject = new GameObject("Menu Canvas", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30000;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        root = canvasObject.GetComponent<RectTransform>();
        EnsureEventSystem();
        if (main)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        Show(main ? Page.Home : Page.Closed);
    }

    private static void EnsureEventSystem()
    {
        var system = FindFirstObjectByType<EventSystem>();
        if (system == null) system = new GameObject("Menu EventSystem", typeof(EventSystem)).GetComponent<EventSystem>();
        var input = system.GetComponent<InputSystemUIInputModule>();
        if (input == null)
        {
            foreach (var module in system.GetComponents<BaseInputModule>()) module.enabled = false;
            input = system.gameObject.AddComponent<InputSystemUIInputModule>();
            input.AssignDefaultActions();
        }
        input.enabled = true;
    }

    private bool GameOver()
    {
        var manager = FindFirstObjectByType<GameManager>();
        return manager != null && manager.IsGameOver;
    }

    private void Update()
    {
        if (loading || root == null || Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
        if (page == Page.Closed)
        {
            if (!main && !GameOver() && Time.timeScale > 0f) Pause();
        }
        else if (page == Page.Pause) Resume();
        else if (page == Page.Settings || page == Page.Credits) Show(main ? Page.Home : Page.Pause);
        else if (page == Page.Confirm || page == Page.Error) Show(returnPage);
    }

    private void Pause()
    {
        previousTimeScale = Time.timeScale;
        previousAudioPause = AudioListener.pause;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        ownsPause = true;
        // The scene also contains asset-pack controllers that read keys directly.
        // Disable those input/AI behaviours temporarily; restore only ones we disabled.
        foreach (var behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (!behaviour.enabled || behaviour.GetType().Namespace != "FantasyDungeonPixelPack") continue;
            string name = behaviour.GetType().Name;
            if (name == "PlayerCombat" || name == "PlayerDodge" || name == "InteractionSystem" ||
                name == "EquipmentSystem" || name == "SaveSystem" || name == "EnemyAI" ||
                name == "TopDownCharacterController" || name == "AbilityHotbar")
            { suspended.Add(behaviour); behaviour.enabled = false; }
        }
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Show(Page.Pause);
    }

    private void ReleasePause(bool restoreTime)
    {
        if (!ownsPause) return;
        if (restoreTime) Time.timeScale = previousTimeScale;
        AudioListener.pause = previousAudioPause;
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
        ownsPause = false;
        foreach (var behaviour in suspended) if (behaviour != null) behaviour.enabled = true;
        suspended.Clear();
    }

    private void Resume()
    {
        ReleasePause(!GameOver());
        Show(Page.Closed);
    }

    public void NotifyGameOver()
    {
        ReleasePause(false);
        if (root != null) Show(Page.Closed);
    }

    private void OnDestroy() => ReleasePause(true);

    private void Show(Page next)
    {
        page = next;
        if (content != null)
        {
            content.SetActive(false);
            Destroy(content);
        }
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        content = new GameObject("Menu Page", typeof(RectTransform));
        content.transform.SetParent(root, false);
        var container = content.GetComponent<RectTransform>();
        Stretch(container);
        if (next == Page.Closed) { content.SetActive(false); return; }

        if (main)
        {
            var background = new GameObject("Background", typeof(RectTransform), typeof(RawImage)).GetComponent<RectTransform>();
            background.SetParent(container, false);
            Stretch(background);
            var raw = background.GetComponent<RawImage>();
            raw.texture = Resources.Load<Texture2D>("DungeonMenus/main_menu_background");
            raw.raycastTarget = false;
            // Crop background to fill rather than distorting architecture on ultrawide screens.
            if (raw.texture != null)
            {
                float textureAspect = (float)raw.texture.width / raw.texture.height;
                float screenAspect = (float)Screen.width / Mathf.Max(1, Screen.height);
                if (screenAspect > textureAspect)
                { float h = textureAspect / screenAspect; raw.uvRect = new Rect(0, (1-h)/2, 1, h); }
                else
                { float w = screenAspect / textureAspect; raw.uvRect = new Rect((1-w)/2, 0, w, 1); }
            }
            if (PlayerPrefs.GetInt(MenuAtmosphere.PreferenceKey, 1) != 0)
                background.gameObject.AddComponent<MenuAtmosphere>();
        }
        var scrim = ImageRect(container, "Scrim", new Color(0, 0, 0, main && next == Page.Home ? 0.10f : 0.65f));
        if (next == Page.Home)
        {
            var home = new GameObject("Home", typeof(RectTransform)).GetComponent<RectTransform>();
            home.SetParent(container, false);
            home.anchorMin = home.anchorMax = new Vector2(0, 0.5f);
            home.pivot = new Vector2(0, 0.5f);
            home.anchoredPosition = new Vector2(80, 0);
            home.sizeDelta = new Vector2(520, 900);
            Label(home, GameTitle.ToUpperInvariant(), 46, 90, 170);
            var unavailable = AddButton(home, "Continue", 325, null);
            unavailable.interactable = false;
            AddButton(home, "New Game", 415, () => Load(GameScene));
            AddButton(home, "Settings", 505, () => Show(Page.Settings));
            AddButton(home, "Credits", 595, () => Show(Page.Credits));
            AddButton(home, "Quit", 685, () => Confirm("Quit the game?", Quit));
            Label(home, "PROTOTYPE", 18, 820, 40);
            SelectFirst(home);
            return;
        }

        var panel = ImageRect(container, "Panel Border", bronze);
        panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(600, 730);
        var inner = ImageRect(panel, "Panel", new Color(0.055f, 0.05f, 0.045f, 0.99f));
        inner.offsetMin = new Vector2(3, 3); inner.offsetMax = new Vector2(-3, -3);
        if (next == Page.Pause)
        {
            Label(inner, "PAUSED", 44, 55, 70);
            AddButton(inner, "Resume", 170, Resume);
            AddButton(inner, "Settings", 270, () => Show(Page.Settings));
            AddButton(inner, "Main Menu", 370, () => Confirm("Return to the main menu?\nProgress in this run will be lost.", () => Load(MainScene)));
            AddButton(inner, "Quit to Desktop", 470, () => Confirm("Quit the game?\nProgress in this run will be lost.", Quit));
            Label(inner, "Esc — Resume", 22, 620, 40);
        }
        else if (next == Page.Settings)
        {
            Label(inner, "SETTINGS", 44, 55, 70);
            Label(inner, $"Master volume: {Mathf.RoundToInt(AudioListener.volume * 100)}%", 26, 150, 50);
            AddButton(inner, "Volume −", 220, () => SetVolume(-0.1f));
            AddButton(inner, "Volume +", 310, () => SetVolume(0.1f));
            AddButton(inner, "Fullscreen: " + (Screen.fullScreen ? "On" : "Off"), 400, ToggleFullscreen);
            AddButton(inner, "Menu animation: " + (PlayerPrefs.GetInt(MenuAtmosphere.PreferenceKey, 1) != 0 ? "On" : "Off"), 490, ToggleAnimation);
            AddButton(inner, "Back", 600, () => Show(main ? Page.Home : Page.Pause));
        }
        else if (next == Page.Credits)
        {
            Label(inner, "CREDITS", 44, 55, 70);
            Label(inner, GameTitle + "\n\nGame in development.\nFull credits coming soon.", 26, 190, 220);
            AddButton(inner, "Back", 560, () => Show(Page.Home));
        }
        else
        {
            Label(inner, next == Page.Error ? "UNAVAILABLE" : "CONFIRM", 40, 55, 70);
            Label(inner, message, 26, 180, 180);
            if (next == Page.Confirm) AddButton(inner, "Confirm", 430, () => confirmedAction?.Invoke());
            AddButton(inner, next == Page.Error ? "Back" : "Cancel", 540, () => Show(returnPage));
        }
        SelectFirst(inner);
    }

    private void SetVolume(float delta)
    {
        AudioListener.volume = Mathf.Clamp01(AudioListener.volume + delta);
        PlayerPrefs.SetFloat(VolumeKey, AudioListener.volume);
        PlayerPrefs.Save();
        Show(Page.Settings);
    }

    private void ToggleAnimation()
    {
        PlayerPrefs.SetInt(MenuAtmosphere.PreferenceKey, PlayerPrefs.GetInt(MenuAtmosphere.PreferenceKey, 1) == 0 ? 1 : 0);
        PlayerPrefs.Save();
        Show(Page.Settings);
    }

    private void ToggleFullscreen()
    {
        bool enabled = !Screen.fullScreen;
        Screen.fullScreen = enabled;
        PlayerPrefs.SetInt(FullscreenKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
        // Fullscreen changes apply at the end of the frame.
        StartCoroutine(RefreshSettings());
    }

    private System.Collections.IEnumerator RefreshSettings()
    {
        yield return null;
        if (page == Page.Settings) Show(Page.Settings);
    }

    private void Confirm(string text, Action action)
    {
        returnPage = page;
        message = text;
        confirmedAction = action;
        Show(Page.Confirm);
    }

    private void Load(string scene)
    {
        if (loading) return;
        if (!Application.CanStreamedLevelBeLoaded(scene))
        {
            returnPage = main ? Page.Home : Page.Pause;
            message = "This destination is unavailable in this build.";
            Debug.LogError("Missing build scene: " + scene + ". Run Tools/Dungeon/Set Up Menus in the Editor.");
            Show(Page.Error);
            return;
        }
        loading = true;
        ReleasePause(true);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(scene);
    }

    private void Quit()
    {
        ReleasePause(true);
        Time.timeScale = 1f;
        AudioListener.pause = false;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private RectTransform ImageRect(Transform parent, string name, Color color)
    {
        var rect = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        Stretch(rect);
        rect.GetComponent<Image>().color = color;
        return rect;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private Text Label(Transform parent, string value, int size, float top, float height)
    {
        var label = new GameObject("Text", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
        label.transform.SetParent(parent, false);
        TopRect(label.rectTransform, top, height, 25);
        label.font = font;
        label.fontSize = size;
        label.color = ivory;
        label.text = value;
        label.alignment = TextAnchor.MiddleCenter;
        label.raycastTarget = false;
        return label;
    }

    private Button AddButton(Transform parent, string label, float top, Action action)
    {
        var rect = ImageRect(parent, label, bronze);
        TopRect(rect, top, 70, 38);
        var body = ImageRect(rect, "Button Face", new Color(0.07f, 0.06f, 0.05f, 0.95f));
        body.offsetMin = new Vector2(2, 2); body.offsetMax = new Vector2(-2, -2);
        body.GetComponent<Image>().raycastTarget = false;
        var button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = body.GetComponent<Image>();
        var colors = button.colors;
        colors.normalColor = new Color(0.7f, 0.7f, 0.7f);
        colors.highlightedColor = colors.selectedColor = new Color(1.5f, 1.3f, 1.0f);
        colors.pressedColor = new Color(0.4f, 0.3f, 0.2f);
        colors.disabledColor = new Color(0.25f, 0.25f, 0.25f);
        button.colors = colors;
        var text = Label(rect, label, 28, 0, 70);
        if (action == null) text.color = new Color(0.4f, 0.4f, 0.38f);
        if (action != null) button.onClick.AddListener(() => action());
        return button;
    }

    private static void TopRect(RectTransform rect, float top, float height, float margin)
    {
        rect.anchorMin = new Vector2(0, 1); rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.offsetMin = new Vector2(margin, -top - height);
        rect.offsetMax = new Vector2(-margin, -top);
    }

    private static void SelectFirst(Transform parent)
    {
        if (EventSystem.current == null) return;
        foreach (var button in parent.GetComponentsInChildren<Button>())
            if (button.interactable) { EventSystem.current.SetSelectedGameObject(button.gameObject); break; }
    }
}
