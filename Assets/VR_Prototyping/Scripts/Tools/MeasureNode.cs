using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureNode : MonoBehaviour
    {
        public MeasureTool MeasureTool { get; set; }
        public MeasureTape MeasureTape { private get; set; }
        public bool LockNode { private get; set; }
        public ControllerTransforms C { private get; set; }      
        public TextMeshPro Text { get; private set; }      
        public float Distance { get; set; }      
        
        private const float DirectDistance = .05f;
        
        private bool rGrabP;
        private bool lGrabP;

        private void Awake()
        {
            LockNode = true;
            Text = GetComponentInChildren<TextMeshPro>(); 
        }

        private void FixedUpdate()
        {
            transform.LookAwayFrom(C.CameraTransform(), Vector3.up);
            
            if(LockNode) return;
            
            DirectGrabCheck(C.RightControllerTransform(), C.RightGrab(), rGrabP);
            DirectGrabCheck(C.LeftControllerTransform(), C.LeftGrab(), lGrabP);

            rGrabP = C.RightGrab();
            lGrabP = C.LeftGrab();
        }
        
        private void DirectGrabCheck(Transform controller, bool grab, bool pGrab)
        {
            
            if (!(Vector3.Distance(transform.position, controller.position) < DirectDistance)) return;
            if(MeasureTool.MeasureNode != null && MeasureTool.MeasureNode != this) return;
            
            if (grab && !pGrab)
            {
                Debug.Log("Node Start");
                MeasureTool.MeasureNode = this;
                MeasureTool.MeasureTape = MeasureTape;
            }

            if (grab && pGrab)
            {
                Debug.Log("Node Stay");
                MeasureTape.AdjustTape();
                Set.TransformLerpPosition(transform, controller, .5f);
            }

            if (!grab && pGrab)
            {
                Debug.Log("Node End");
                MeasureTool.MeasureNode = null;
            }
        }
    }
}
