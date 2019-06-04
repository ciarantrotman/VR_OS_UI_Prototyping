using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Keyboard
{
    public class KeyboardTarget : MonoBehaviour
    {
        private string _text;
        private TextMeshPro _textField;

        private void Awake()
        {
            _textField = GetComponentInChildren<TextMeshPro>();
        }

        public void SetText(char newText)
        {
            _text += newText;
            _textField.SetText(_text);
        }

        public string CheckText()
        {
            return _text;
        }

    public void DeleteText()
        {
            _text = _text.Substring(0, _text.Length - 1);
            _textField.SetText(_text);
        }
    
    public void ClearText()
        {
            _text = "";
            _textField.SetText(_text);
        }
    }
}
