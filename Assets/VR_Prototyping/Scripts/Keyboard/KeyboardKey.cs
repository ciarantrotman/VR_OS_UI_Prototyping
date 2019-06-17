using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace VR_Prototyping.Scripts.Keyboard
{
    public class KeyboardKey : SelectableObject
    {
        public KeyboardManager KeyboardManager { private get; set; }
        public int Index { private get; set; }
        
        private float borderDepth;

        [BoxGroup("Key Settings")] [SerializeField] private Transform border;
        [BoxGroup("Key Settings")] [Space(10)] [SerializeField] [Range(0f, .01f)] private float restDepth;
        [BoxGroup("Key Settings")] [SerializeField] [Range(.01f, .05f)] private float hoverDepth;
        [BoxGroup("Key Settings")] [SerializeField] [Range(.1f, 1f)] private float hoverDuration;
        [BoxGroup("Key Settings")] [Space(10)] public KeyboardManager.KeyboardKeyValues keyValue;

        private void Awake()
        {
            selectStart.AddListener(KeyStroke);
        }

        public void SetupKey(KeyboardManager manager, GameObject p)
        {
            KeyboardManager = manager;
            player = p;
            name = "Key_" + (char)keyValue;
            buttonText.SetText(""+(char)keyValue);
            buttonBack.shadowCastingMode = ShadowCastingMode.Off;
            toolTipText = keyValue.ToString();
            
            hoverStart.AddListener(HoverBorderStart);
            hoverEnd.AddListener(HoverBorderEnd);
        }
        
        private void KeyStroke()
        {
            KeyboardManager.Keystroke(Index, keyValue);
        }
        
        protected override void ObjectUpdate()
        {
            border.localPosition = new Vector3(0, 0, borderDepth);
        }

        private void HoverBorderStart()
        {
            borderDepth = border.localPosition.z;
            DOTween.To(()=> borderDepth, x=> borderDepth = x, -hoverDepth, hoverDuration);
        }

        private void HoverBorderEnd()
        {
            borderDepth = border.localPosition.z;
            DOTween.To(()=> borderDepth, x=> borderDepth = x, -restDepth, hoverDuration);
        }
    }
}
