using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace VR_Prototyping.Scripts.Keyboard
{
    public class KeyboardKey : SelectableObject
    {
        public KeyboardManager keyboardManager { private get; set; }
        public int index { private get; set; }

        [BoxGroup ("Key Settings")] public KeyboardManager.KeyboardKeyValues keyValue;

        private void Awake()
        {
            selectEnd.AddListener(KeyStroke);
        }

        protected override void InitialiseOverride()
        {
            SetupKey();
        }

        private void SetupKey()
        {
            //keyboardManager = GetComponentInParent<KeyboardManager>();
            //player = keyboardManager.controllerTransforms.Player();
            name = "Key_" + (char)keyValue;
            buttonText.SetText(""+(char)keyValue);
            buttonBack.shadowCastingMode = ShadowCastingMode.Off;
            toolTip = true;
            toolTipText = keyValue.ToString();
            Debug.Log(keyboardManager.controllerTransforms.name);
        }
        
        private void KeyStroke()
        {
            keyboardManager.Keystroke(index, keyValue);
        }
    }
}
