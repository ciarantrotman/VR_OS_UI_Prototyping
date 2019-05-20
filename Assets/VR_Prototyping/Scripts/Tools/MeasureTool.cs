
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureTool : BaseTool
    {        
        [BoxGroup("Tape Tool Settings")] [Required] public GameObject tapeNodePrefab;
        [BoxGroup("Tape Tool Settings")] [Range(.001f, .05f)] public float tapeWidth;
        [BoxGroup("Tape Tool Settings")] [Range(.001f, .05f)] public float nodeGrabDistance = .1f;
        [BoxGroup("Tape Tool Settings")] [Space(5)] public Material tapeMaterial;
        [BoxGroup("Tape Tool Settings")] [Space(5)] public Color tapeColor = new Color(0,0,0,255);

        public MeasureText MeasureText { get; set; }
        public MeasureTape MeasureTape { get; set; }
        public MeasureNode MeasureNode  { get; set; }
        
        private int tapeCount;
        
        private GameObject tapeObject;
        private GameObject node;

        protected override void Initialise()
        {
            NewTape();
        }

        protected override void ToolUpdate()
        {
            if (MeasureText == null) return;
            MeasureText.transform.LookAwayFrom(controller.CameraTransform(), Vector3.up);
            
            if (MeasureText == null || MeasureNode == null || MeasureTape == null) return;
            MeasureText.SetText(MeasureTape.TapeDistance(), MeasureTape.TapeName);
        }

        protected override void ToolStart()
        {
            var position = dominant.transform.position;
            var positionCount = MeasureTape.TapeLr.positionCount;
            positionCount++;
            MeasureTape.TapeLr.positionCount = positionCount;
            MeasureTape.TapeLr.SetPosition(positionCount - 1, position);
            CreateNode();
        }

        protected override void ToolStay()
        {            
            Set.Transforms(node.transform, dominant.transform);
            MeasureTape.TapeLr.SetPosition(MeasureTape.TapeLr.positionCount - 1, dominant.transform.position);
            MeasureTape.AdjustTape(); 
        }

        protected override void ToolEnd()
        {
            ReleaseNode();
        }

        protected override void ToolInactive()
        {
            
        }

        public void NewTape()
        {
            tapeCount++;
            
            tapeObject = new GameObject("Tape_" + tapeCount);
            Set.Transforms(tapeObject.transform, dominant.transform);
            MeasureTape = tapeObject.AddComponent<MeasureTape>();
            MeasureTape.MeasureTool = this;
            
            MeasureTape.TapeLr = tapeObject.AddComponent<LineRenderer>();
            Setup.LineRender(MeasureTape.TapeLr, tapeMaterial, tapeWidth, true);
            MeasureTape.TapeLr.positionCount = 0;
            MeasureTape.TapeLr.material.color = tapeColor;

            MeasureTape.TapeName = tapeCount.ToString();
        }

        private void CreateNode()
        {
            node = Instantiate(tapeNodePrefab, tapeObject.transform, true);
            node.transform.position = dominant.transform.position;
            MeasureNode = node.GetComponent<MeasureNode>();
            MeasureNode.MeasureTool = this;
            MeasureNode.C = controller;
            MeasureNode.MeasureTape = MeasureTape;
            MeasureNode.LockNode = false;
            MeasureTape.measureNodes.Add(MeasureNode);
        }

        private void ReleaseNode()
        {
            node = null;
            MeasureNode = null;
        }
    }
}
