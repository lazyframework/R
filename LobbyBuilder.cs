using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EclipseLobby.Editor
{
    public static class LobbyBuilder
    {
        const string Root = "Assets/EclipseLobby";
        const float W = 1672f, H = 941f;
        static Font font;

        [MenuItem("Tools/Project Eclipse/Build Lobby Scene")]
        public static void Build()
        {
            // Gambar referensi (LobbyReference.jpg) tidak lagi diperlukan.
            // Teks "PROJECT ECLIPSE" bawaan gambar juga sudah tidak ada.
            
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

#if UNITY_2022_2_OR_NEWER
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
#else
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
#endif
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Setup Camera
            var cameraObject = new GameObject("Lobby Camera", typeof(Camera), typeof(AudioListener));
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.orthographic = true;
            cameraObject.tag = "MainCamera";

            // Setup Canvas
            var canvasObject = new GameObject("Eclipse Lobby Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(W, H);
            scaler.matchWidthOrHeight = .5f;

            var controller = canvasObject.AddComponent<LobbyController>();
            
            // Setup Frame
            var frame = Rect("Reference Frame - aspect preserved", canvasObject.transform);
            var fitter = frame.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = W / H;

            // Setup Background (Warna Solid Pengganti Gambar)
            var bg = Rect("Solid Background", frame);
            Stretch(bg);
            var image = bg.gameObject.AddComponent<Image>(); 
            // Warna biru gelap ala Sci-Fi
            image.color = new Color(0.02f, 0.08f, 0.15f, 1f); 
            image.raycastTarget = false;

            // Setup Hotspots (Area Tombol Transparan)
            // Posisi disesuaikan dengan tata letak asli
            Hotspot(frame, controller, "PROFILE", 30, 120, 390, 168);
            Hotspot(frame, controller, "CRYSTALS", 1043, 24, 165, 50);
            Hotspot(frame, controller, "COINS", 1218, 24, 174, 50);
            Hotspot(frame, controller, "NOTIFICATIONS", 1413, 14, 75, 66);
            Hotspot(frame, controller, "FRIENDS", 1494, 14, 75, 66);
            Hotspot(frame, controller, "SETTINGS", 1578, 14, 75, 66);
            Hotspot(frame, controller, "NEW AGENT - VYRA", 30, 369, 377, 154);
            Hotspot(frame, controller, "NOTICE", 30, 536, 377, 113);
            Hotspot(frame, controller, "DAILY MISSION", 1315, 178, 330, 125);
            Hotspot(frame, controller, "SEASON RANK", 1315, 319, 330, 117);
            Hotspot(frame, controller, "RESEARCH FACILITY", 1315, 451, 330, 212);
            Hotspot(frame, controller, "OPERATIONS", 29, 787, 220, 115);
            Hotspot(frame, controller, "AGENTS", 255, 789, 176, 110);
            Hotspot(frame, controller, "ARMORY", 445, 789, 176, 110);
            Hotspot(frame, controller, "INVENTORY", 635, 789, 185, 110);
            Hotspot(frame, controller, "STORE", 835, 789, 181, 110);
            Hotspot(frame, controller, "SETTINGS", 1032, 789, 180, 110);
            Hotspot(frame, controller, "DEPLOY", 1260, 758, 391, 152);

            // Setup Popup
            BuildPopup(canvasObject.transform, controller);

            // Setup Event System
            var es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            es.AddComponent<StandaloneInputModule>();
#endif

            // Simpan Prefab dan Scene
            Directory.CreateDirectory(Root + "/Scenes");
            Directory.CreateDirectory(Root + "/Prefabs");
            PrefabUtility.SaveAsPrefabAsset(canvasObject, Root + "/Prefabs/EclipseLobby.prefab");
            EditorSceneManager.SaveScene(scene, Root + "/Scenes/EclipseLobby.unity");
            AssetDatabase.SaveAssets();
            
            Selection.activeGameObject = canvasObject;
            EditorUtility.DisplayDialog("Lobby ready", "Scene and Canvas prefab generated successfully without artwork dependency.\n\nPress Play to test. The blue transparent boxes are the clickable hotspots.", "OK");
        }

        static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        static void Stretch(RectTransform r)
        {
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
        }

        static void Region(RectTransform r, float x, float y, float w, float h)
        {
            r.anchorMin = new Vector2(x/W, 1f-(y+h)/H);
            r.anchorMax = new Vector2((x+w)/W, 1f-y/H);
            r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
        }

        static void Hotspot(Transform parent, LobbyController controller, string label, float x, float y, float w, float h)
        {
            var r = Rect("Button - " + label, parent); Region(r,x,y,w,h);
            var im = r.gameObject.AddComponent<Image>(); 
            // Memberikan warna biru transparan agar batas tombol terlihat saat testing
            im.color = new Color(0.1f, 0.4f, 0.8f, 0.2f); 
            
            var button = r.gameObject.AddComponent<Button>(); button.targetGraphic = im;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.2f, 0.6f, 1f, 0.5f);
            colors.selectedColor = new Color(0.2f, 0.6f, 1f, 0.5f);
            colors.pressedColor = new Color(0.1f, 0.8f, 1f, 0.8f);
            colors.fadeDuration = .12f; button.colors = colors;
            
            // Tambahkan Teks Label di tengah tombol agar jelas
            var txtObj = Rect("Text", r);
            Stretch(txtObj);
            var text = txtObj.gameObject.AddComponent<Text>();
            text.font = font;
            text.text = label;
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;

            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(button.onClick, controller.Open, label);
        }

        static Text Label(Transform parent, string name, string text, float x, float y, float w, float h, int size)
        {
            var r = Rect(name, parent); Region(r,x,y,w,h);
            var t = r.gameObject.AddComponent<Text>(); t.font=font; t.text=text;
            t.fontSize=size; t.color=Color.white; t.alignment=TextAnchor.MiddleLeft;
            t.horizontalOverflow=HorizontalWrapMode.Wrap; t.verticalOverflow=VerticalWrapMode.Truncate;
            t.raycastTarget=false; return t;
        }

        static void BuildPopup(Transform parent, LobbyController controller)
        {
            var overlay = Rect("Popup Overlay", parent); Stretch(overlay);
            overlay.gameObject.AddComponent<Image>().color = new Color(0,0,.025f,.84f);
            var panel = Rect("Popup Panel", overlay);
            panel.anchorMin=panel.anchorMax=new Vector2(.5f,.5f);
            panel.sizeDelta=new Vector2(850,460);
            panel.gameObject.AddComponent<Image>().color=new Color(.025f,.065f,.14f,.99f);
            var outline=panel.gameObject.AddComponent<Outline>(); outline.effectColor=new Color(.15f,.6f,1f); outline.effectDistance=new Vector2(2,-2);
            
            controller.popupTitle=Label(panel,"Title","LOBBY",100,70,1450,130,32);
            controller.popupBody=Label(panel,"Body","",100,230,1450,460,22);
            
            var close=Rect("Close",panel); Region(close,1190,730,370,135);
            close.gameObject.AddComponent<Image>().color=new Color(.04f,.35f,.8f);
            var b=close.gameObject.AddComponent<Button>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(b.onClick,controller.ClosePopup);
            var text=Label(close,"Label","CLOSE",0,0,W,H,22); text.alignment=TextAnchor.MiddleCenter;
            
            controller.popup=overlay.gameObject; overlay.gameObject.SetActive(false);
        }
    }
}
