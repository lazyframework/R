using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace EclipseLobby
{
    public class LobbyController : MonoBehaviour
    {
        [Header("Generated popup references")]
        public GameObject popup;
        public Text popupTitle;
        public Text popupBody;
        [Header("Connect your gameplay here")]
        public UnityEvent onDeploy = new UnityEvent();
        public UnityEvent onOperations = new UnityEvent();
        public UnityEvent onAgents = new UnityEvent();
        public UnityEvent onArmory = new UnityEvent();
        public UnityEvent onInventory = new UnityEvent();
        public UnityEvent onStore = new UnityEvent();
        public UnityEvent onSettings = new UnityEvent();

        public void Open(string section)
        {
            switch (section)
            {
                case "DEPLOY": onDeploy.Invoke(); break;
                case "OPERATIONS": onOperations.Invoke(); break;
                case "AGENTS": onAgents.Invoke(); break;
                case "ARMORY": onArmory.Invoke(); break;
                case "INVENTORY": onInventory.Invoke(); break;
                case "STORE": onStore.Invoke(); break;
                case "SETTINGS": onSettings.Invoke(); break;
            }
            if (popupTitle) popupTitle.text = section;
            if (popupBody) popupBody.text = Message(section);
            if (popup) popup.SetActive(true);
        }
        string Message(string section)
        {
            switch(section)
            {
                case "DEPLOY": return "READY TO DEPLOY\n\nDemo interface only. Connect On Deploy in the LobbyController Inspector to your gameplay scene or matchmaking service.";
                case "PROFILE": return "FamaLine\nLevel 12  |  EXP 720 / 2,000\nSilver III";
                case "DAILY MISSION": return "Play 3 matches\nProgress: 0 / 3\nReward: 100 crystals\n\nDemo data — no rewards are granted.";
                case "SEASON RANK": return "Bronze I\nSeason progress: 0 / 1000";
                case "NOTICE": return "Maintenance will be on\nNov 10, 08:00 (UTC+7).\n\nReference artwork text; not a live service announcement.";
                case "NEW AGENT - VYRA": return "VYRA\nNew agent preview.\n\nConnect this button to your agent details or store.";
                case "RESEARCH FACILITY": return "RESEARCH FACILITY\nNew map preview.\n\nMap artwork is included in the background; a playable 3D map is not included.";
                case "CRYSTALS": return "Crystals: 1,200\n\nDemo wallet. No purchase or payment processing is connected.";
                case "COINS": return "Coins: 15,340\n\nDemo wallet. No purchase or payment processing is connected.";
                default: return section + "\n\nThis is a working navigation placeholder. Edit Button > On Click or connect the matching LobbyController event to your own content.";
            }
        }
        public void ClosePopup() { if (popup) popup.SetActive(false); }
        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame) ClosePopup();
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Escape)) ClosePopup();
#endif
        }
    }
}
