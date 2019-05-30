using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Keyboard
{
    public class KeyboardTarget : MonoBehaviour
    {
        private string text { get; set; }
        private TextMeshPro textField;

        private void Awake()
        {
            textField = GetComponentInChildren<TextMeshPro>();
        }

        public void SetText(char newText)
        {
            text = text + newText;
            textField.SetText(text);
        }
    }
}
