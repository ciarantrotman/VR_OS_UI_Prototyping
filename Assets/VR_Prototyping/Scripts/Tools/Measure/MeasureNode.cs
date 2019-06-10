﻿using System.Runtime.CompilerServices;
using LeapInternal;
using TMPro;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools.Measure
{
    public class MeasureNode : MonoBehaviour
    {
        private MeasureTool MeasureTool { get; set; }
        private MeasureTape MeasureTape { get; set; }
        public bool LockNode { get; set; }
        private ControllerTransforms Controller { get; set; }      
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
        
        private LineRenderer xLr;
        private LineRenderer yLr;
        private LineRenderer zLr;
        
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
            xLr = X.transform.AddOrGetLineRenderer();
            xLr.SetupLineRender(Controller.lineRenderMat, .001f, true);
            X.SetActive(false);
            
            Y = Instantiate(MeasureTool.snapObject, transform, true);
            Y.name = "Axis_Y";
            Y.transform.localPosition = Vector3.zero;
            yLr = Y.transform.AddOrGetLineRenderer();
            yLr.SetupLineRender(Controller.lineRenderMat, .001f, true);
            Y.SetActive(false);
            
            Z = Instantiate(MeasureTool.snapObject, transform, true);
            Z.name = "Axis_Z";
            Z.transform.localPosition = Vector3.zero;
            zLr = Z.transform.AddOrGetLineRenderer();
            zLr.SetupLineRender(Controller.lineRenderMat, .001f, true);
            Z.SetActive(false);
        }

        private void FixedUpdate()
        {
            Text.transform.LookAwayFrom(Controller.CameraTransform(), Vector3.up);
            
            if (!MeasureTool.Active) return;
            
            DirectGrabCheck(Controller.RightTransform(), Controller.RightGrab(), rGrabP);
            DirectGrabCheck(Controller.LeftTransform(), Controller.LeftGrab(), lGrabP);

            rGrabP = Controller.RightGrab();
            lGrabP = Controller.LeftGrab();
            
            NodeEvents();
        }

        private void NodeEvents()
        {
            switch (CurrentNode())
            {
                case true when !PreviousNode(): // spawned but not released
                    NodeStay();
                    break;
                case true when PreviousNode(): // spawned and released
                    NodeStay();
                    break;
                case false when PreviousNode(): // released and no new node
                    NodeStay();
                    break;
                case false when !PreviousNode(): // new node and released
                    NodeEnd();
                    break;
                default:
                    return;
            }
        }

        private bool PreviousNode()
        {
            return MeasureTool.PreviousMeasureNode == this;
        }
        private bool CurrentNode()
        {
            return MeasureTool.FocusMeasureNode == this;
        }
        
        private void DirectGrabCheck(Transform controller, bool grab, bool pGrab)
        {
            if (!MeasureTool.grabNode) return;
            if (!MeasureTool.Grabbing)
            {
                if (!(Vector3.Distance(transform.position, controller.position) < DirectDistance))
                {
                    return;
                }
            }
            
            if (MeasureTool.FocusMeasureNode != null && MeasureTool.FocusMeasureNode != this && MeasureTool.Grabbing) return;

            if (grab && !pGrab)
            {
                MeasureTool.GrabNode(this, MeasureTape);
                MeasureTool.DeactivateAllTapes();

                if (NodeIndex == 0) return;
                previousNode = MeasureTape.measureNodes[NodeIndex - 1];
                MeasureTool.PreviousMeasureNode = previousNode;
                previousNode.NodeStart();
                return;
            }

            if (grab)
            {
                if (LockNode) return;
                MeasureTool.Grabbing = true;
                switch (NodeIndex)
                {
                    case 0:
                        transform.TransformLerpPosition(controller, .85f);
                        break;
                    default:
                        MeasureTool.NodeSnap(controller, this,  previousNode, MeasureTape);
                        break;
                }
                MeasureTape.AdjustTape();
                return;
            }

            if (!MeasureTool.Grabbing) return;
            MeasureTool.Grabbing = false;
            MeasureTool.MeasureNode = null;
            MeasureTool.PreviousMeasureNode = MeasureTape.measureNodes[MeasureTape.measureNodes.Count - 1];
            MeasureTool.PreviousMeasureNode.NodeStart();
            if (previousNode != null) previousNode.NodeEnd();
        }

        public void DeleteNode()
        {
            MeasureTape.measureNodes.RemoveAt(NodeIndex);
            Destroy(transform.gameObject);
        }

        public void NodeStart()
        {
            X.SetActive(true);
            Y.SetActive(true);
            Z.SetActive(true);
            Text.fontSize = MeasureTool.nodeTextFocusHeight;
        }
        
        public void NodeStay()
        {
            SnappingTransforms();

            XSnap = CheckSnap(xDistance, yDistance, zDistance, MeasureTool.snapTolerance);
            YSnap = CheckSnap(yDistance, xDistance, zDistance, MeasureTool.snapTolerance);
            ZSnap = CheckSnap(zDistance, xDistance, yDistance, MeasureTool.snapTolerance);
            
            xLr.LineRender(transform, X.transform);
            yLr.LineRender(transform, Y.transform);
            zLr.LineRender(transform, Z.transform);
            
            xLr.LineRenderWidth(XSnap ? MeasureTool.activeWidth : MeasureTool.inactiveWidth, XSnap ? MeasureTool.activeWidth : MeasureTool.inactiveWidth);
            yLr.LineRenderWidth(YSnap ? MeasureTool.activeWidth : MeasureTool.inactiveWidth, YSnap ? MeasureTool.activeWidth : MeasureTool.inactiveWidth);
            zLr.LineRenderWidth(ZSnap ? MeasureTool.activeWidth : MeasureTool.inactiveWidth, ZSnap ? MeasureTool.activeWidth : MeasureTool.inactiveWidth);
        }

        public void NodeEnd()
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
