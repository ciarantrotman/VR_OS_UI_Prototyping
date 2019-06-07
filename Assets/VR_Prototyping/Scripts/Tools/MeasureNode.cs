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

        private MeasureNode previousNode;

        private MeshRenderer meshRenderer;

        GameObject dominantFollow;
        GameObject x;
        GameObject y;
        GameObject z;
        
        private const float DirectDistance = .05f;
        
        private bool rGrabP;
        private bool lGrabP;

        public void Initialise(MeasureTool tool, ControllerTransforms c, MeasureTape tape)
        {
            MeasureTool = tool;
            Controller = c;
            MeasureTape = tape;
            Text = GetComponentInChildren<TextMeshPro>();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            SetupAxis();
        }

        void SetupAxis()
        {
            dominantFollow = new GameObject("Dominant_Follow");
            dominantFollow.transform.SetParent(transform);
            dominantFollow.transform.localPosition = Vector3.zero;

            x = Instantiate(MeasureTool.snapObject, transform, true);
            x.name = "Axis_X";
            x.transform.localPosition = Vector3.zero;
            
            y = Instantiate(MeasureTool.snapObject, transform, true);
            y.name = "Axis_Y";
            y.transform.localPosition = Vector3.zero;
            
            z = Instantiate(MeasureTool.snapObject, transform, true);
            z.name = "Axis_Z";
            z.transform.localPosition = Vector3.zero;
        }

        private void FixedUpdate()
        {
            Text.transform.LookAwayFrom(Controller.CameraTransform(), Vector3.up);
            
            DirectGrabCheck(Controller.RightTransform(), Controller.RightGrab(), rGrabP);
            DirectGrabCheck(Controller.LeftTransform(), Controller.LeftGrab(), lGrabP);

            rGrabP = Controller.RightGrab();
            lGrabP = Controller.LeftGrab();

            if (!previousNode == this && CurrentNode())
            {
                NodeStart();
            }
            else if (previousNode == this && CurrentNode())
            {
                NodeStay();
            }
            else if (previousNode && !CurrentNode())
            {
                NodeEnd();
            }
            else if (!previousNode && !CurrentNode())
            {
                NodeInactiveStay();
            }

            previousNode = MeasureTool.MeasureNode;
        }

        private bool CurrentNode()
        {
            return MeasureTool.FocusMeasureNode == this;
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
                MeasureTool.Grabbing = true;
                MeasureTape.AdjustTape();
                transform.TransformLerpPosition(controller, .85f);
                return;
            }

            MeasureTool.Grabbing = false;
            MeasureTool.MeasureNode = null;
        }

        public void DeleteNode()
        {
            MeasureTape.measureNodes.RemoveAt(NodeIndex);
            Destroy(transform.gameObject);
        }

        private void NodeStart()
        {
            Text.fontSize = MeasureTool.nodeTextFocusHeight;
        }
        private void NodeStay()
        {
            SnappingTransforms();
        }

        private void NodeEnd()
        {
            Text.fontSize = MeasureTool.nodeTextStandardHeight;
        }

        private void NodeInactiveStay()
        {
            
        }
        
        private void SnappingTransforms()
        {
            dominantFollow.transform.Transforms(MeasureTool.dominant.transform);
            
            x.transform.LockAxis(dominantFollow.transform, Set.Axis.X);
            y.transform.LockAxis(dominantFollow.transform, Set.Axis.Y);
            z.transform.LockAxis(dominantFollow.transform, Set.Axis.Z);
        }

        public void SetColor(Color color)
        {
            meshRenderer.material.color = color;
        }
    }
}
