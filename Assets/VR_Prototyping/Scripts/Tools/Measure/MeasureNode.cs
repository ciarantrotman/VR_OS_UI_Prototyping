using System.Runtime.CompilerServices;
using LeapInternal;
using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools.Measure
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

        private GameObject dominantFollow;
        
        public GameObject X {get; private set;}
        public GameObject Y {get; private set;}
        public GameObject Z {get; private set;}
        
        private float xDistance;
        private float yDistance;
        private float zDistance;
        
        public bool XSnap { get; private set; }
        public bool YSnap { get; private set; }
        public bool ZSnap { get; private set; }
        

        
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

        private void SetupAxis()
        {
            dominantFollow = new GameObject("Dominant_Follow");
            dominantFollow.transform.SetParent(transform);
            dominantFollow.transform.localPosition = Vector3.zero;

            X = Instantiate(MeasureTool.snapObject, transform, true);
            X.name = "Axis_X";
            X.transform.localPosition = Vector3.zero;
            
            Y = Instantiate(MeasureTool.snapObject, transform, true);
            Y.name = "Axis_Y";
            Y.transform.localPosition = Vector3.zero;
            
            Z = Instantiate(MeasureTool.snapObject, transform, true);
            Z.name = "Axis_Z";
            Z.transform.localPosition = Vector3.zero;
        }

        private void FixedUpdate()
        {
            Text.transform.LookAwayFrom(Controller.CameraTransform(), Vector3.up);
            
            DirectGrabCheck(Controller.RightTransform(), Controller.RightGrab(), rGrabP);
            DirectGrabCheck(Controller.LeftTransform(), Controller.LeftGrab(), lGrabP);

            rGrabP = Controller.RightGrab();
            lGrabP = Controller.LeftGrab();

            if (previousNode != null)
            {
                NodeEvents();
            }
            previousNode = MeasureTool.FocusMeasureNode;
        }

        private void NodeEvents()
        {
            if (previousNode != this && CurrentNode())
            {
                NodeStart();
            }
            else if (previousNode == this && CurrentNode())
            {
                NodeStay();
            }
            else if (previousNode == this  && !CurrentNode())
            {
                NodeStay();
            }
            else if (previousNode != this && !CurrentNode())
            {
                NodeEnd();
            }
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
            X.SetActive(true);
            Y.SetActive(true);
            Z.SetActive(true);
            
            Text.fontSize = MeasureTool.nodeTextFocusHeight;
        }
        private void NodeStay()
        {
            SnappingTransforms();

            XSnap = CheckSnap(xDistance, yDistance, zDistance, MeasureTool.snapTolerance);
            YSnap = CheckSnap(yDistance, xDistance, zDistance, MeasureTool.snapTolerance);
            ZSnap = CheckSnap(zDistance, xDistance, yDistance, MeasureTool.snapTolerance);
        }

        private void NodeEnd()
        {
            X.SetActive(false);
            Y.SetActive(false);
            Z.SetActive(false);
            Text.fontSize = MeasureTool.nodeTextStandardHeight;
        }

        private void NodeInactiveStay()
        {
            
        }
        
        private void SnappingTransforms()
        {
            dominantFollow.transform.Transforms(MeasureTool.dominant.transform);
            
            X.transform.LockAxis(dominantFollow.transform, Set.Axis.X);
            Y.transform.LockAxis(dominantFollow.transform, Set.Axis.Y);
            Z.transform.LockAxis(dominantFollow.transform, Set.Axis.Z);

            xDistance = SnapDistance(X);
            yDistance = SnapDistance(Y);
            zDistance = SnapDistance(Z);
        }

        private float SnapDistance(GameObject axis)
        {
            return Vector3.Distance(axis.transform.position, dominantFollow.transform.position);
        }

        private static bool CheckSnap(float a, float b, float c, float threshold)
        {
            return a < threshold && a < b && a < c;
        }

        public void SetColor(Color color)
        {
            meshRenderer.material.color = color;
        }
    }
}
