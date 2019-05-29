using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MemoNode : MonoBehaviour
    {
        private MemoTool MemoTool { get; set; }
        public AudioSource AudioSource { get; private set; }
        public ControllerTransforms Controller { private get; set; }
        private Color _nodeColor;
        
        private int _nodeIndex;
        
        private float _memoLength;

        private bool _rGrabP;
        private bool _lGrabP;
        
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
            Set.ReverseNormals(GetComponentInChildren<MeshFilter>());
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

        
        private void FixedUpdate()
        {
            transform.LookAwayFrom(Controller.CameraTransform(), Vector3.up);
            
            DirectGrabCheck(Controller.RightTransform(), Controller.RightGrab(), _rGrabP);
            DirectGrabCheck(Controller.LeftTransform(), Controller.LeftGrab(), _lGrabP);

            _rGrabP = Controller.RightGrab();
            _lGrabP = Controller.LeftGrab();
        }
        
        private void DirectGrabCheck(Transform controller, bool grab, bool pGrab)
        {
            
            if (!(Vector3.Distance(transform.position, controller.position) < MemoTool.triggerDistance)) return;

            if (grab && !pGrab)
            {
                MemoTool.PlayAudio(_nodeIndex);
                return;
            }

            if (grab)
            {
                // on hold goes here
                return;
            }
            
            // on release goes here
        }
    }
}
