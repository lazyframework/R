using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace LeakOSHUD.Editor
{
    // =====================================================================
    // BAGIAN 1: EDITOR BUILDER (Untuk Generate HUD)
    // =====================================================================
    public class TacticalHUDBuilderWindow : EditorWindow
    {
        const string RootName = "LEAKOS_TACTICAL_HUD";
        
        // Palet Warna
        static readonly Color Navy = new Color(0.018f, 0.045f, 0.095f, 0.85f);
        static readonly Color Blue = new Color(0.05f, 0.48f, 1f, 0.96f);
        static readonly Color Cyan = new Color(0.12f, 0.72f, 1f, 1f);
        static readonly Color White = new Color(0.91f, 0.96f, 1f, 1f);
        static readonly Color Muted = new Color(0.60f, 0.73f, 0.88f, 1f);
        static readonly Color DarkPanel = new Color(0.02f, 0.06f, 0.12f, 0.90f);

        [MenuItem("Tools/LeakOS/Tactical HUD Builder V2")]
        public static void Open()
        {
            var w = GetWindow<TacticalHUDBuilderWindow>("Tactical HUD");
            w.minSize = new Vector2(360, 250);
        }

        void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("LEAKOS / TACTICAL HUD BUILDER", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Generate a complete mobile shooter HUD. Gunakan tombol Remove lalu Generate jika ingin update.", MessageType.Info);
            EditorGUILayout.Space(8);

            if (GUILayout.Button("GENERATE COMPLETE HUD", GUILayout.Height(38)))
                Generate();

            if (GUILayout.Button("Remove Generated HUD"))
                RemoveHUD();
        }

        static void Generate()
        {
            RemoveHUD();

            // 1. Root Canvas
            var root = new GameObject(RootName, typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TacticalHUDRuntime));
            Undo.RegisterCreatedObjectUndo(root, "Generate Tactical HUD");

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // Background Gelap untuk melihat HUD
            var bgGo = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            bgGo.transform.SetParent(root.transform, false);
            var bgRt = bgGo.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;
            bgGo.GetComponent<Image>().color = new Color(0.08f, 0.12f, 0.18f, 1f); 
            bgGo.GetComponent<Image>().raycastTarget = false;

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1648, 928);
            scaler.matchWidthOrHeight = 0.5f;

            if (FindObjectOfType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
            }

            // ==========================================
            // TOP LEFT: PROFILE & MISSION
            // ==========================================
            var profile = Panel(root.transform, "Player Profile", 28, 24, 320, 110, Navy);
            Panel(profile, "Avatar Frame", 10, 10, 90, 90, new Color(0.05f, 0.15f, 0.3f, 0.9f));
            Text(profile, "Avatar Initial", "FL", 10, 10, 90, 90, 34, Cyan, TextAnchor.MiddleCenter);
            
            Text(profile, "Player Name", "FamaLine", 115, 10, 190, 30, 22, White, TextAnchor.MiddleLeft);
            Text(profile, "Player Level", "Lv. 12", 115, 40, 60, 25, 18, White, TextAnchor.MiddleLeft);
            Bar(profile, "XP Bar", 180, 48, 125, 8, Cyan, 0.36f);
            Text(profile, "XP Text", "720 / 2,000", 180, 25, 125, 18, 12, Muted, TextAnchor.MiddleRight);
            Text(profile, "Rank", "◆ Silver III", 115, 70, 190, 25, 16, White, TextAnchor.MiddleLeft);

            var mission = Panel(root.transform, "Main Mission", 28, 150, 360, 120, Navy);
            Text(mission, "Mission Header", "◆ MAIN MISSION", 15, 10, 330, 25, 18, Cyan, TextAnchor.MiddleLeft);
            Text(mission, "Mission Title", "Infiltrate the Research Facility", 15, 40, 330, 25, 18, White, TextAnchor.MiddleLeft);
            Text(mission, "Mission Objective", "◇ Reach the Control Room (0/1)", 15, 75, 330, 25, 16, White, TextAnchor.MiddleLeft);

            // ==========================================
            // TOP CENTER: COMPASS
            // ==========================================
            var compass = Panel(root.transform, "Compass", 550, 22, 548, 40, DarkPanel);
            Text(compass, "Compass Labels", "W     285     300     NW     330     N     15     30     NE", 8, 0, 532, 40, 18, White, TextAnchor.MiddleCenter);

            // ==========================================
            // TOP RIGHT: CURRENCY, SETTINGS, MINIMAP
            // ==========================================
            var currency = Panel(root.transform, "Currency", 1160, 24, 310, 40, DarkPanel);
            Text(currency, "Currency Values", "◆ 1,200   +       ◉ 15,340   +", 8, 0, 294, 40, 16, White, TextAnchor.MiddleCenter);
            
            CircleButton(root.transform, "Notifications", "!", 1490, 24, 44, 44, 20);
            CircleButton(root.transform, "Settings", "⚙", 1545, 24, 44, 44, 20);

            // Minimap
            var map = Panel(root.transform, "Minimap", 1412, 80, 210, 200, new Color(0.04f, 0.1f, 0.18f, 0.9f));
            Text(map, "Map North", "N", 0, 5, 210, 20, 14, White, TextAnchor.MiddleCenter);
            for (int i = 1; i <= 4; i++)
            {
                Box(map, "Map Grid V" + i, i * 42, 25, 1, 160, new Color(0.5f, 0.7f, 0.9f, 0.2f));
                Box(map, "Map Grid H" + i, 10, 25 + i * 32, 190, 1, new Color(0.5f, 0.7f, 0.9f, 0.2f));
            }
            Text(map, "Map Marker", "◆", 120, 40, 40, 40, 24, new Color(1f, 0.75f, 0.08f), TextAnchor.MiddleCenter);
            Text(map, "Player Marker", "▲", 85, 110, 40, 40, 22, Cyan, TextAnchor.MiddleCenter);
            
            var timer = Panel(root.transform, "Match Timer", 1412, 288, 210, 38, DarkPanel);
            Text(timer, "Timer Text", "◷ 14:32", 0, 0, 210, 36, 20, White, TextAnchor.MiddleCenter);

            // ==========================================
            // BOTTOM LEFT: MOVEMENT JOYSTICK
            // ==========================================
            CircleButton(root.transform, "Quick Run", "↗", 200, 460, 75, 75, 24);
            
            var joystickBg = CircleButton(root.transform, "Movement Joystick", "", 110, 580, 220, 220, 0);
            joystickBg.GetComponent<Image>().color = new Color(0.18f, 0.36f, 0.58f, 0.15f);
            Outline(joystickBg, new Color(0.45f, 0.72f, 1f, 0.5f), 3);
            
            var knob = CircleButton(joystickBg.transform, "Joystick Knob", "", 0, 0, 90, 90, 0);
            var knobRt = knob.GetComponent<RectTransform>();
            knobRt.anchorMin = knobRt.anchorMax = new Vector2(0.5f, 0.5f);
            knobRt.anchoredPosition = Vector2.zero;
            knob.GetComponent<Image>().color = new Color(0.75f, 0.88f, 1f, 0.6f);
            
            var pointer = joystickBg.AddComponent<HUDJoystickInput>();
            pointer.area = joystickBg.GetComponent<RectTransform>();
            pointer.knob = knob.GetComponent<RectTransform>();

            // ==========================================
            // BOTTOM RIGHT: ACTION BUTTONS
            // ==========================================
            CircleButton(root.transform, "Skill Q", "Q", 1544, 340, 70, 70, 24);
            CircleButton(root.transform, "Heal E", "E", 1544, 424, 70, 70, 24);
            CircleButton(root.transform, "Sprint R", "R", 1544, 508, 70, 70, 24);
            
            CircleButton(root.transform, "Aim", "◎", 1445, 575, 80, 80, 28);
            CircleButton(root.transform, "Fire", "FIRE", 1305, 620, 140, 140, 28); // Tombol Fire besar
            
            CircleButton(root.transform, "Crouch", "▼", 1360, 790, 65, 65, 20);
            CircleButton(root.transform, "Prone", "━", 1480, 790, 65, 65, 20);

            // ==========================================
            // BOTTOM CENTER: WEAPONS & CONSUMABLES
            // ==========================================
            var weapons = Panel(root.transform, "Weapon Inventory", 576, 758, 620, 82, new Color(0.01f, 0.04f, 0.08f, 0.7f));
            
            var primary = Panel(weapons, "Primary Weapon", 0, 0, 245, 76, new Color(0.03f, 0.12f, 0.25f, 0.95f));
            Text(primary, "Primary Icon", "ASSAULT RIFLE", 8, 7, 225, 28, 16, White, TextAnchor.MiddleCenter);
            Text(primary, "Primary Ammo", "30 / 120", 8, 35, 225, 30, 22, Cyan, TextAnchor.MiddleCenter);
            
            var secondary = Panel(weapons, "Secondary Weapon", 252, 0, 155, 76, new Color(0.03f, 0.09f, 0.18f, 0.95f));
            Text(secondary, "Secondary Icon", "PISTOL", 4, 7, 147, 28, 14, White, TextAnchor.MiddleCenter);
            Text(secondary, "Secondary Ammo", "15 / 45", 4, 35, 147, 30, 18, Muted, TextAnchor.MiddleCenter);
            
            var grenade = Panel(weapons, "Grenade", 414, 0, 88, 76, new Color(0.03f, 0.09f, 0.18f, 0.95f));
            Text(grenade, "Grenade Count", "◉ 2", 0, 0, 88, 76, 20, White, TextAnchor.MiddleCenter);
            
            var medkit = Panel(weapons, "Medkit", 510, 0, 88, 76, new Color(0.03f, 0.09f, 0.18f, 0.95f));
            Text(medkit, "Medkit Count", "✚ 1", 0, 0, 88, 76, 20, White, TextAnchor.MiddleCenter);

            // ==========================================
            // BOTTOM CENTER: HEALTH & ARMOR
            // ==========================================
            var status = Panel(root.transform, "Player Status", 480, 850, 680, 60, new Color(0.01f, 0.04f, 0.08f, 0.7f));
            Text(status, "Health Label", "✚ HP 100", 10, 2, 112, 27, 16, White, TextAnchor.MiddleLeft);
            var hp = Bar(status, "Health Fill", 130, 10, 420, 10, new Color(0.1f, 0.7f, 1f), 1);
            Text(status, "Armor Label", "⬟ ARMOR 100", 10, 30, 112, 27, 16, White, TextAnchor.MiddleLeft);
            var armor = Bar(status, "Armor Fill", 130, 38, 420, 10, new Color(0.1f, 0.48f, 0.95f), 1);

            // Binding Data
            var runtime = root.GetComponent<TacticalHUDRuntime>();
            runtime.healthFill = hp;
            runtime.armorFill = armor;
            runtime.healthLabel = FindText(status.transform, "Health Label");
            runtime.armorLabel = FindText(status.transform, "Armor Label");
            runtime.ammoLabel = FindText(primary.transform, "Primary Ammo");
            runtime.grenadeLabel = FindText(grenade.transform, "Grenade Count");
            runtime.medkitLabel = FindText(medkit.transform, "Medkit Count");
            runtime.timerLabel = FindText(timer.transform, "Timer Text");
            runtime.missionLabel = FindText(mission.transform, "Mission Objective");

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Selection.activeGameObject = root;
            EditorUtility.DisplayDialog("LeakOS HUD", "HUD Berhasil Dibuat!", "OK");
        }

        static void RemoveHUD()
        {
            var old = GameObject.Find(RootName);
            if (old) Undo.DestroyObjectImmediate(old);
        }

        // --- HELPER FUNCTIONS ---

        static GameObject Panel(Transform parent, string name, float x, float y, float w, float h, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(x, -y);
            rt.sizeDelta = new Vector2(w, h);
            var img = go.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
            Outline(go, new Color(0.1f, 0.5f, 1f, 0.4f), 1.5f);
            return go;
        }

        static Image Box(Transform parent, string name, float x, float y, float w, float h, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0,1);
            rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = new Vector2(x,-y);
            rt.sizeDelta = new Vector2(w,h);
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        static Text Text(Transform parent, string name, string value, float x, float y, float w, float h,
            int size, Color color, TextAnchor align)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0,1);
            rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = new Vector2(x,-y);
            rt.sizeDelta = new Vector2(w,h);
            var t = go.GetComponent<Text>();
            t.text = value;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.fontSize = size;
            t.color = color;
            t.alignment = align;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            return t;
        }

        static GameObject CircleButton(Transform parent, string name, string label, float x, float y, float w, float h, int fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0,1);
            rt.pivot = new Vector2(0.5f,0.5f);
            rt.anchoredPosition = new Vector2(x,-y);
            rt.sizeDelta = new Vector2(w,h);
            
            var img = go.GetComponent<Image>();
            img.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
            img.color = new Color(0.02f, 0.08f, 0.15f, 0.8f);
            img.raycastTarget = true;
            
            Outline(go, new Color(0.3f, 0.6f, 1f, 0.8f), 2);
            
            if (!string.IsNullOrEmpty(label))
                Text(go.transform, "Label", label, 0, 0, w, h, fontSize, White, TextAnchor.MiddleCenter);
                
            return go;
        }

        static Image Bar(Transform parent, string name, float x, float y, float w, float h, Color color, float fill)
        {
            var bg = Box(parent, name + " Background", x, y, w, h, new Color(0.01f, 0.05f, 0.1f, 0.9f));
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(bg.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0,0);
            rt.anchorMax = new Vector2(fill,1);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var image = go.GetComponent<Image>();
            image.color = color;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillAmount = fill;
            return image;
        }

        static void Outline(GameObject go, Color color, float width)
        {
            var outline = go.GetComponent<Outline>();
            if (!outline) outline = go.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(width, -width);
            outline.useGraphicAlpha = true;
        }

        static Text FindText(Transform root, string objectName)
        {
            foreach (var t in root.GetComponentsInChildren<Text>(true))
                if (t.name == objectName) return t;
            return null;
        }
    }

    // =====================================================================
    // BAGIAN 2: RUNTIME LOGIC (Untuk update HP, Ammo, dll)
    // =====================================================================
    public class TacticalHUDRuntime : MonoBehaviour
    {
        [Header("Player values")]
        [Range(0, 100)] public float health = 100;
        [Range(0, 100)] public float armor = 100;
        public int magazine = 30;
        public int reserveAmmo = 120;
        public int grenades = 2;
        public int medkits = 1;

        [Header("HUD references")]
        public Image healthFill;
        public Image armorFill;
        public Text healthLabel;
        public Text armorLabel;
        public Text ammoLabel;
        public Text grenadeLabel;
        public Text medkitLabel;
        public Text timerLabel;
        public Text missionLabel;

        void Start() { Refresh(); }

        public void SetHealth(float value) { health = Mathf.Clamp(value, 0, 100); Refresh(); }
        public void SetArmor(float value) { armor = Mathf.Clamp(value, 0, 100); Refresh(); }
        public void SetAmmo(int inMagazine, int inReserve)
        {
            magazine = Mathf.Max(0, inMagazine);
            reserveAmmo = Mathf.Max(0, inReserve);
            Refresh();
        }
        public void SetConsumables(int grenadeCount, int medkitCount)
        {
            grenades = Mathf.Max(0, grenadeCount);
            medkits = Mathf.Max(0, medkitCount);
            Refresh();
        }
        public void SetMission(string value) { if (missionLabel) missionLabel.text = value; }

        public void Refresh()
        {
            if (healthFill) healthFill.fillAmount = health / 100f;
            if (armorFill) armorFill.fillAmount = armor / 100f;
            if (healthLabel) healthLabel.text = "✚ HP " + Mathf.CeilToInt(health);
            if (armorLabel) armorLabel.text = "⬟ ARMOR " + Mathf.CeilToInt(armor);
            if (ammoLabel) ammoLabel.text = magazine + " / " + reserveAmmo;
            if (grenadeLabel) grenadeLabel.text = "◉ " + grenades;
            if (medkitLabel) medkitLabel.text = "✚ " + medkits;
        }
    }

    // =====================================================================
    // BAGIAN 3: JOYSTICK LOGIC
    // =====================================================================
    public class HUDJoystickInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform area;
        public RectTransform knob;
        public float radius = 65f;
        public Vector2 Value { get; private set; }
        public System.Action<Vector2> OnValueChanged;

        public void OnPointerDown(PointerEventData eventData) { OnDrag(eventData); }

        public void OnDrag(PointerEventData eventData)
        {
            if (!area) area = transform as RectTransform;
            Vector2 local;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(area, eventData.position, eventData.pressEventCamera, out local)) return;

            Value = Vector2.ClampMagnitude(local / Mathf.Max(1f, radius), 1f);
            if (knob) knob.anchoredPosition = Value * radius;
            OnValueChanged?.Invoke(Value);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector2.zero;
            if (knob) knob.anchoredPosition = Vector2.zero;
            OnValueChanged?.Invoke(Value);
        }
    }
}
