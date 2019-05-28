using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureTool : BaseTool
    {        
        [BoxGroup("Tape Tool Settings")] [Required] public GameObject tapeNodePrefab;
        [BoxGroup("Tape Tool Settings")] [Required] public GameObject intersectionPointPrefab;
        [BoxGroup("Tape Tool Settings")] [Range(.00001f, .1f)] public float tolerance = .01f;
        [BoxGroup("Tape Tool Settings")] [Range(.001f, .05f)] public float tapeWidth;
        [BoxGroup("Tape Tool Settings")] [Range(.001f, .05f)] public float nodeGrabDistance = .1f;
        [BoxGroup("Tape Tool Settings")] [Space(5)] public Material tapeMaterial;
        [BoxGroup("Tape Tool Settings")] [Space(5)] public Color tapeColor = new Color(0,0,0,255);
        [BoxGroup("Tape Tool Settings")] [Space(5)] public float nodeTextFocusHeight = .2f;
        [BoxGroup("Tape Tool Settings")] public float nodeTextStandardHeight = .15f;

        public MeasureText MeasureText { get; set; }
        public MeasureTape MeasureTape { get; set; }
        public MeasureTape FocusMeasureTape { get; set; }
        public MeasureNode MeasureNode  { get; set; }
        public MeasureNode FocusMeasureNode  { get; set; }
        
        public bool Insertion { get; set; }
        
        private int _tapeCount;
        
        private GameObject _tapeObject;
        private GameObject _node;

        protected override void Initialise()
        {
            NewTape();
            intersectionPointPrefab = Instantiate(intersectionPointPrefab, transform);
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
            if (Insertion) return;
            InsertNode(MeasureTape, dominant.transform.position, MeasureTape.measureNodes.Count);
        }

        protected override void ToolStay()
        {
            if (Insertion) return;
            Set.Transforms(_node.transform, dominant.transform);
            MeasureTape.TapeLr.SetPosition(MeasureTape.TapeLr.positionCount - 1, dominant.transform.position);
            MeasureTape.AdjustTape(); 
        }

        protected override void ToolEnd()
        {
            if (Insertion) return;
            ReleaseNode();
        }

        protected override void ToolInactive()
        {
            
        }

        public void NewTape()
        {
            _tapeCount++;
            
            _tapeObject = new GameObject("Tape_" + _tapeCount);
            Set.Transforms(_tapeObject.transform, dominant.transform);
            MeasureTape = _tapeObject.AddComponent<MeasureTape>();
            MeasureTape.Controller = controller;
            MeasureTape.MeasureTool = this;

            MeasureTape.TapeLr = _tapeObject.AddComponent<LineRenderer>();
            Setup.LineRender(MeasureTape.TapeLr, tapeMaterial, tapeWidth, true);
            MeasureTape.TapeLr.positionCount = 0;
            MeasureTape.TapeLr.material.color = tapeColor;

            MeasureTape.TapeName = _tapeCount.ToString();
        }

        public void InsertNode(MeasureTape tape, Vector3 position, int index)
        {
            _node = Instantiate(tapeNodePrefab, position, Quaternion.identity, _tapeObject.transform);
            MeasureNode = _node.GetComponent<MeasureNode>();
            FocusMeasureTape = tape;
            FocusMeasureNode = MeasureNode;
            MeasureNode.Initialise(this, controller, tape);
            tape.measureNodes.Insert(index, MeasureNode);
            tape.RefactorNodes();
            AddLineRenderNode(tape.TapeLr, position);
            tape.AdjustTape();
        }

        private void ReleaseNode()
        {
            _node = null;
            MeasureNode = null;
        }

        public void DeleteNode()
        {
            if (FocusMeasureNode == null) return;
            
            ReleaseNode();
            FocusMeasureNode.DeleteNode();
            FocusMeasureTape.RefactorNodes();
            RemoveLineRenderNode(FocusMeasureTape.TapeLr);
            FocusMeasureTape.AdjustTape();
        }
        
        public void LockNode()
        {
            if (FocusMeasureNode == null) return;
            
            FocusMeasureNode.LockNode = !FocusMeasureNode.LockNode;
            MeasureTape.AdjustTape();
        }

        private static void AddLineRenderNode(LineRenderer lr, Vector3 position)
        {
            var positionCount = lr.positionCount;
            positionCount++;
            lr.positionCount = positionCount;
            lr.SetPosition(positionCount - 1, position);
        }
        
        private static void RemoveLineRenderNode(LineRenderer lr)
        {
            var positionCount = lr.positionCount;
            positionCount--;
            lr.positionCount = positionCount;
        }
    }
}
