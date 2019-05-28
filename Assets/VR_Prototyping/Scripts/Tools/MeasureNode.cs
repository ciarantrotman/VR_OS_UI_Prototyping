using TMPro;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureNode : MonoBehaviour
    {
        public MeasureTool MeasureTool { get; set; }
        public MeasureTape MeasureTape { private get; set; }
        public bool LockNode { get; set; }
        public ControllerTransforms Controller { private get; set; }      
        public TextMeshPro Text { get; private set; }
        public float Distance { get; set; }
        public int NodeIndex { get; set; }

        private MeshRenderer _renderer;
        

        private const float DirectDistance = .05f;
        
        private bool rGrabP;
        private bool lGrabP;

        public void Initialise(MeasureTool tool, ControllerTransforms c, MeasureTape tape)
        {
            MeasureTool = tool;
            Controller = c;
            MeasureTape = tape;
            Text = GetComponentInChildren<TextMeshPro>();
            _renderer = GetComponent<MeshRenderer>();
        }

        private void FixedUpdate()
        {
            transform.LookAwayFrom(Controller.CameraTransform(), Vector3.up);
            
            DirectGrabCheck(Controller.RightTransform(), Controller.RightGrab(), rGrabP);
            DirectGrabCheck(Controller.LeftTransform(), Controller.LeftGrab(), lGrabP);

            rGrabP = Controller.RightGrab();
            lGrabP = Controller.LeftGrab();

            switch (MeasureTool.FocusMeasureNode == this)
            {
                case true:
                    NodeInFocus();
                    break;
                case false:
                    NodeOutFocus();
                    break;
            }
        }
        
        private void DirectGrabCheck(Transform controller, bool grab, bool pGrab)
        {
            if (!(Vector3.Distance(transform.position, controller.position) < DirectDistance)) return;
            if(MeasureTool.MeasureNode != null && MeasureTool.MeasureNode != this) return;

            if (grab && !pGrab)
            {
                MeasureTool.MeasureNode = this;
                MeasureTool.FocusMeasureNode = this;
                MeasureTool.MeasureTape = MeasureTape;
                MeasureTool.FocusMeasureTape = MeasureTape;
                MeasureTool.MeasureVisual.SetColor(MeasureTape.tapeColor);
                return;
            }

            if (grab)
            {
                if (LockNode) return;
                MeasureTape.AdjustTape();
                Set.TransformLerpPosition(transform, controller, .85f);
                return;
            }

            MeasureTool.MeasureNode = null;
        }

        public void DeleteNode()
        {
            MeasureTape.measureNodes.RemoveAt(NodeIndex);
            Destroy(transform.gameObject);
        }

        private void NodeInFocus()
        {
            Text.fontSize = MeasureTool.nodeTextFocusHeight;
        }

        private void NodeOutFocus()
        {
            Text.fontSize = MeasureTool.nodeTextStandardHeight;
        }

        public void SetColor(Color color)
        {
            _renderer.material.color = color;
        }
    }
}
