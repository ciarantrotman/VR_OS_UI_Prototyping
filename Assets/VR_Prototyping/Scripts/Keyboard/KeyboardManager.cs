using System.Collections.Generic;
using UnityEngine;

namespace VR_Prototyping.Scripts.Keyboard
{
    public class KeyboardManager : MonoBehaviour
    {
        public KeyboardTarget keyboardTarget;
        public ControllerTransforms controllerTransforms;
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
            Space = ' ', 
            Comma = ',', 
            Period = '.', 
            QuestionMark = '?', 
            ExclamationMark = '!',
            One = '1', 
            Two = '2', 
            Three = '3', 
            Four = '4', 
            Five = '5', 
            Six = '6', 
            Seven = '7', 
            Eight = '8', 
            Nine = '9', 
            Zero = '0',
            Back, 
            Enter
        }

        public List<KeyboardKey> keyboardKeys = new List<KeyboardKey>(); 

        private void Start()
        {
            var index = 0;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<KeyboardKey>() == null) continue;
                var key = child.GetComponent<KeyboardKey>();
                key.index = index;
                keyboardKeys.Add(key);
                index++;
            }
        }

        public void Keystroke(int index)
        {
            if (keyboardTarget == null) return;
            keyboardTarget.SetText((char)keyboardKeys[index].keyValue);
        }
    }
}
