using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MemoNode : MonoBehaviour
    {
        private MemoTool MemoTool { get; set; }
        public AudioSource AudioSource { get; private set; }
        public ControllerTransforms Controller { get; set; }
        private Color _nodeColor;
        private float _memoLength;
        
        private int _nodeIndex;
        
        [Button]
        public void PlayAudioDebug()
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
            AudioSource.spatialize = true;
            AudioSource.spatializePostEffects = true;
        }

        private void SetColor(Color color)
        {
            _nodeColor = color;
            GetComponentInChildren<MeshRenderer>().material.color = color;
            GetComponentInChildren<MeshFilter>().ReverseNormals();
        }

        private void SetIndex(int index)
        {
            _nodeIndex = index;
        }
        
        private void SetLength(float length)
        {
            _memoLength = length;
        }

        public void ReleaseMemo(float length)
        {
            SetLength(length);
            GetComponentInChildren<TextMeshPro>().SetText(_nodeIndex + " - {0:1} ", length);
        }

        public void PlayMemo()
        {
            Debug.Log("PLAY");
            MemoTool.PlayAudio(_nodeIndex);
        }
        
        private void FixedUpdate()
        {
            var x = transform;
            x.LookAwayFrom(Controller.CameraTransform(), Vector3.up);
            x.eulerAngles = new Vector3(0, x.eulerAngles.y, 0);
        }
    }
}
