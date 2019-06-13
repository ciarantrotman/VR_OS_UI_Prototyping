using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace VR_Prototyping.Scripts.Keyboard
{
    public class KeyboardKey : SelectableObject
    {
        public KeyboardManager KeyboardManager { private get; set; }
        public int Index { private get; set; }

        [BoxGroup ("Key Settings")] public KeyboardManager.KeyboardKeyValues keyValue;

        private void Awake()
        {
            selectEnd.AddListener(KeyStroke);
        }

        protected override void InitialiseOverride()
        {
            
        }

        public void SetupKey(KeyboardManager manager, GameObject p)
        {
            KeyboardManager = manager;
            player = p;
            name = "Key_" + (char)keyValue;
            buttonText.SetText(""+(char)keyValue);
            buttonBack.shadowCastingMode = ShadowCastingMode.Off;
            toolTipText = keyValue.ToString();
        }
        
        private void KeyStroke()
        {
            KeyboardManager.Keystroke(Index, keyValue);
        }
    }
}
