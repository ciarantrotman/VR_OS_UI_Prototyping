using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace VR_Prototyping.Scripts.Keyboard
{
    public class KeyboardKey : SelectableObject
    {
        private KeyboardManager keyboardManager { get; set; }
        public int index { get; set; }

        [BoxGroup ("Key Settings")] public KeyboardManager.KeyboardKeyValues keyValue;
        
        protected override void InitialiseOverride()
        {
            SetupKey();
            selectEnd.AddListener(KeyStroke);
        }

        private void SetupKey()
        {
            keyboardManager = GetComponentInParent<KeyboardManager>();
            player = keyboardManager.controllerTransforms.Player();
            name = "Key_" + (char)keyValue;
            buttonText.SetText(""+(char)keyValue);
            buttonBack.shadowCastingMode = ShadowCastingMode.Off;
            toolTip = true;
            toolTipText = keyValue.ToString();
        }
        
        private void KeyStroke()
        {
            Debug.Log(index + " pressed!");
            keyboardManager.Keystroke(index);
        }
    }
}
