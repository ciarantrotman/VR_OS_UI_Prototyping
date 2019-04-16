using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureNode : MonoBehaviour
    {
        public MeasureTape MeasureTape { private get; set; }
        public bool LockNode { private get; set; }
        public ControllerTransforms C { private get; set; }      
        public TextMeshPro Text { get; private set; }      
        public float Distance { get; set; }      
        
        
        private const float DirectDistance = .05f;

        private void Awake()
        {
            LockNode = true;
            Text = GetComponentInChildren<TextMeshPro>();
        }

        private void FixedUpdate()
        {
            if(LockNode) return;
            
            DirectSliderCheck(C.RightControllerTransform(), C.RightGrab());
            DirectSliderCheck(C.LeftControllerTransform(), C.LeftGrab());
            
            transform.LookAwayFrom(C.CameraTransform(), Vector3.up);
        }
        
        private void DirectSliderCheck(Transform controller, bool grab)
        {
            if (!(Vector3.Distance(transform.position, controller.position) < DirectDistance) || !grab) return;
            
            MeasureTape.AdjustTape();
            Set.TransformLerpPosition(transform, controller, .5f);
        }
    }
}
