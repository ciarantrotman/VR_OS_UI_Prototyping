using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VR_Prototyping.Scripts.Tools;

namespace VR_Prototyping.Scripts.Keyboard
{
    public class KeyboardManager : MonoBehaviour
    {
        public KeyboardTarget keyboardTarget { get; set; }
        public ToolMenu toolMenu { private get; set; }
        public ControllerTransforms controllerTransforms { get; set; }
        public enum KeyboardKeyValues
        {
            Q = 'Q',
            W = 'W',
            E = 'E',
            R = 'R',
            T = 'T',
            Y = 'Y',
            U = 'U',
            I = 'I',
            O = 'O',
            P = 'P',
            A = 'A',
            S = 'S',
            D = 'D',
            F = 'F',
            G = 'G',
            H = 'H',
            J = 'J',
            K = 'K',
            L = 'L',
            Z = 'Z',
            X = 'X',
            C = 'C',
            V = 'V',
            B = 'B',
            N = 'N',
            M = 'M',
            SPACE = ' ', 
            COMMA = ',', 
            PERIOD = '.', 
            QUESTION_MARK = '?', 
            EXCLAMATION_MARK = '!',
            ONE = '1', 
            TWO = '2', 
            THREE = '3', 
            FOUR = '4', 
            FIVE = '5', 
            SIX = '6', 
            SEVEN = '7', 
            EIGHT = '8', 
            NINE = '9', 
            ZERO = '0',
            BACK = '<', 
            ENTER = '¬'
        }
        public List<KeyboardKey> keyboardKeys = new List<KeyboardKey>();
        [HideInInspector] public UnityEvent enter;

        public void InitialiseKeyboard(ControllerTransforms c, ToolMenu t, Transform parent)
        {
            controllerTransforms = c;
            toolMenu = t;
            transform.SetParent(parent);
            
            var index = 0;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<KeyboardKey>() == null) continue;
                var key = child.GetComponent<KeyboardKey>();
                key.keyboardManager = this;
                key.player = controllerTransforms.Player();
                key.index = index;
                key.SetupKey(this, controllerTransforms.Player());
                keyboardKeys.Add(key);
                index++;
            }

            keyboardTarget = GetComponentInChildren<KeyboardTarget>();
            
            ToggleKeyboard(false);
        }
        
        public void ToggleKeyboard(bool state)
        {
            foreach (var key in keyboardKeys)
            {
                key.enabled = state;
                key.gameObject.SetActive(state);
                keyboardTarget.gameObject.SetActive(state);
            }
        }

        public void Keystroke(int index, KeyboardKeyValues key)
        {
            if (keyboardTarget == null) return;

            switch (key)
            {
                case KeyboardKeyValues.BACK:
                    keyboardTarget.DeleteText();
                    break;
                case KeyboardKeyValues.ENTER:
                    enter.Invoke();
                    break;
                default:
                    keyboardTarget.SetText((char)keyboardKeys[index].keyValue);
                    break;
            }
        }
    }
}
