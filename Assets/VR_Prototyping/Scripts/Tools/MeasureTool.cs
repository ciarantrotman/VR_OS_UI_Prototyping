using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureTool : BaseTool
    {
        public enum NodeLockingType
        {
            RELATIVE,
            GLOBAL
        }
        [BoxGroup("Tape Tool Settings")] public NodeLockingType nodeLockingType;
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
        
        public MeasureVisual MeasureVisual { get; set; }
        
        public bool Insertion { get; set; }
        public bool Grabbing { get; set; }
        public bool Placing { get; set; }
        
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
            
            MeasureText.SetText(MeasureTape.TapeDistance(), MeasureTape.TapeName);

            intersectionPointPrefab.SetActive(Insertion);
        }

        protected override void ToolStart()
        {
            if (Insertion) return;
            Placing = true;
            InsertNode(MeasureTape, dominant.transform.position, MeasureTape.measureNodes.Count);
        }

        protected override void ToolStay()
        {
            if (Insertion) return;
            _node.transform.Position(dominant.transform);
            MeasureTape.TapeLr.SetPosition(MeasureTape.TapeLr.positionCount - 1, dominant.transform.position);
            MeasureTape.AdjustTape(); 
        }

        protected override void ToolEnd()
        {
            Placing = false;
            if (Insertion) return;
            ReleaseNode();
        }

        protected override void ToolInactive()
        {
            
        }

        public void NewTape()
        {
            _tapeCount++;
            var color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1, true);
            
            _tapeObject = new GameObject("Tape_" + _tapeCount);
            _tapeObject.transform.Transforms(dominant.transform);
            MeasureTape = _tapeObject.AddComponent<MeasureTape>();
            MeasureTape.Controller = controller;
            MeasureTape.MeasureTool = this;

            MeasureTape.TapeLr = _tapeObject.AddComponent<LineRenderer>();
            MeasureTape.TapeLr.SetupLineRender(tapeMaterial, tapeWidth, true);
            MeasureTape.SetColor(color);
            if (_tapeCount > 1)
            {
                MeasureVisual.SetColor(color);
            }
            MeasureTape.TapeLr.positionCount = 0;

            MeasureTape.TapeName = _tapeCount.ToString();
        }

        public void InsertNode(MeasureTape tape, Vector3 position, int index)
        {
            _node = Instantiate(tapeNodePrefab, position, Quaternion.identity, _tapeObject.transform);
            MeasureNode = _node.GetComponent<MeasureNode>();
            FocusMeasureTape = tape;
            FocusMeasureNode = MeasureNode;
            FocusMeasureTape.Controller = controller;
            MeasureNode.Initialise(this, controller, tape);
            MeasureNode.SetColor(FocusMeasureTape.tapeColor);
            tape.measureNodes.Insert(index, MeasureNode);
            tape.RefactorNodes();
            AddLineRenderNode(tape.TapeLr, position);
            tape.AdjustTape();
        }

        private void ReleaseNode()
        {
            return;
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
            FocusMeasureTape.RefactorNodes();
            ReleaseNode();
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

        public void SetColor(Color color)
        {
            if (FocusMeasureTape == null) return;
            FocusMeasureTape.SetColor(color);
        }
    }
}
