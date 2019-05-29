using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MemoNode : MonoBehaviour
    {
        public MemoTool MemoTool { get; set; }
        public ControllerTransforms Controller { get; set; }
        private Color _nodeColor;
        private int _nodeIndex;
        public AudioSource AudioSource { get; private set; }

        [Button]
        public void PlayAudio()
        {
            AudioSource.Play();
        }
        
        public void Initialise(Color color, int index, MemoTool memoTool)
        {
            name = "Memo_" + index;
            MemoTool = memoTool;
            Controller = MemoTool.controller;
            SetColor(color);
            SetIndex(index);
            SetupAudio();
        }

        private void SetupAudio()
        {
            AudioSource = gameObject.AddComponent<AudioSource>();
        }

        private void SetColor(Color color)
        {
            _nodeColor = color;
            GetComponentInChildren<MeshRenderer>().material.color = color;
        }

        private void SetIndex(int index)
        {
            _nodeIndex = index;
            GetComponentInChildren<TextMeshPro>().SetText(index.ToString());
        }
    }
}
