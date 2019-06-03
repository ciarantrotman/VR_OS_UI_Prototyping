using System;
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

        public string CheckText()
        {
            return textField.text;
        }

    public void DeleteText()
        {
            text = text.Substring(0, text.Length - 1);
            textField.SetText(text);
        }
    
    public void ClearText()
        {
            text = String.Empty;
            textField.SetText(text);
        }
    }
}
