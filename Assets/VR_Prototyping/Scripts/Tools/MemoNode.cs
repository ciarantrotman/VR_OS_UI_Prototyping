using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MemoNode : MonoBehaviour
    {
        public MemoTool memoTool { get; set; }
        private Color nodeColor;
        private int nodeIndex;
        public AudioSource audioSource { get; private set; }

        [Button]
        public void PlayAudio()
        {
            audioSource.Play();
        }
        
        public void Initialise(Color color, int index)
        {
            name = "Memo_" + index;
            
            SetColor(color);
            SetIndex(index);
            SetupAudio();
        }

        private void SetupAudio()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void SetColor(Color color)
        {
            nodeColor = color;
            GetComponentInChildren<MeshRenderer>().material.color = color;
        }

        private void SetIndex(int index)
        {
            nodeIndex = index;
        }
    }
}
