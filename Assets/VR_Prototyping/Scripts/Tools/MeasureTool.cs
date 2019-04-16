
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
        private MeasureTape measureTape;
        private MeasureNode measureNode;
        
        private int tapeCount;
        
        private GameObject tapeObject;
        private GameObject node;

        protected override void Initialise()
        {
            NewTape();
        }

        protected override void ToolStart()
        {
            var position = dominant.transform.position;
            var positionCount = measureTape.TapeLr.positionCount;
            positionCount++;
            measureTape.TapeLr.positionCount = positionCount;
            measureTape.TapeLr.SetPosition(positionCount - 1, position);
            CreateNode();
        }

        protected override void ToolStay()
        {            
            Set.Transforms(node.transform, dominant.transform);
            measureTape.TapeLr.SetPosition(measureTape.TapeLr.positionCount - 1, dominant.transform.position);
            measureTape.AdjustTape();
            MeasureText.SetText(measureNode.Distance, measureTape.Distance, tapeCount);
        }

        protected override void ToolEnd()
        {
            MeasureText.SetText(measureNode.Distance, measureTape.Distance, tapeCount);
            ReleaseNode();
        }

        protected override void ToolInactive()
        {
            if(measureTape == null || measureNode == null) return;
            MeasureText.SetText(measureNode.Distance, measureTape.Distance, tapeCount);
        }

        public void NewTape()
        {
            tapeCount++;
            
            tapeObject = new GameObject("Tape_" + tapeCount);
            Set.Transforms(tapeObject.transform, dominant.transform);
            measureTape = tapeObject.AddComponent<MeasureTape>();
            measureTape.MeasureTool = this;
            
            measureTape.TapeLr = tapeObject.AddComponent<LineRenderer>();
            Setup.LineRender(measureTape.TapeLr, tapeMaterial, tapeWidth, true);
            measureTape.TapeLr.positionCount = 0;
            measureTape.TapeLr.material.color = tapeColor;
        }

        private void CreateNode()
        {
            node = Instantiate(tapeNodePrefab, tapeObject.transform, true);
            node.transform.position = dominant.transform.position;
            measureNode = node.GetComponent<MeasureNode>();
            measureNode.C = controller;
            measureNode.MeasureTape = measureTape;
            measureNode.LockNode = false;
            measureTape.measureNodes.Add(measureNode);
        }

        private void ReleaseNode()
        {
            node = null;
            measureNode = null;
        }
    }
}
