using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools.Measure
{
    public class MeasureTool : BaseTool
    {
        public enum NodeLockingType
        {
            RELATIVE,
            RELATIVE_VERTICAL,
            GLOBAL
        }
        [BoxGroup("Tape Tool Settings")] [Required] public GameObject tapeNodePrefab;
        [BoxGroup("Tape Tool Settings")] [Space(10)] public bool axisSnapping;
        [BoxGroup("Tape Tool Settings")] [ShowIf("axisSnapping")] [Indent] [Required] public GameObject snapObject;
        [BoxGroup("Tape Tool Settings")] [ShowIf("axisSnapping")] [Indent] public NodeLockingType nodeLockingType;
        [BoxGroup("Tape Tool Settings")] [ShowIf("axisSnapping")] [Indent] [Range(.01f, .5f)] public float snapTolerance;
        [BoxGroup("Tape Tool Settings")] [Space(10)] public bool nodeInsertion;
        [BoxGroup("Tape Tool Settings")] [ShowIf("nodeInsertion")] [Indent] [Required] public GameObject intersectionPointPrefab;
        [BoxGroup("Tape Tool Settings")] [ShowIf("nodeInsertion")] [Indent] [Range(.00001f, .1f)] public float tolerance = .01f;
        [BoxGroup("Tape Tool Settings")] [ShowIf("nodeInsertion")] [Indent] [Range(0f, .1f)] public float insertionThreshold = .1f;
        [BoxGroup("Tape Tool Settings")] [Space(10)] [Range(.001f, .05f)] public float tapeWidth;
        [BoxGroup("Tape Tool Settings")] [Range(.001f, .05f)] public float nodeGrabDistance = .1f;
        [BoxGroup("Tape Tool Settings")] [Space(10)] public Material tapeMaterial;
        [BoxGroup("Tape Tool Settings")] [Indent] public Color tapeColor = new Color(0,0,0,255);
        [BoxGroup("Tape Tool Settings")] [Space(10)] public float nodeTextFocusHeight = .2f;
        [BoxGroup("Tape Tool Settings")] public float nodeTextStandardHeight = .15f;

        public MeasureText MeasureText { get; set; }
        public MeasureTape MeasureTape { get; set; }
        public MeasureTape FocusMeasureTape { get; set; }
        public MeasureNode MeasureNode  { get; set; }
        public MeasureNode FocusMeasureNode  { get; set; }
        public MeasureNode PreviousMeasureNode  { get; set; }
        
        public MeasureVisual MeasureVisual { get; set; }
        
        public bool Insertion { get; set; }
        public bool Grabbing { get; set; }
        public bool Placing { get; private set; }
        
        private int tapeCount;

        private bool validNode;
        
        private GameObject tapeObject;
        private GameObject node;

        protected override void Initialise()
        {
            NewTape();
            intersectionPointPrefab = Instantiate(intersectionPointPrefab, transform);
            intersectionPointPrefab.name = "Measure/Intersection";
        }

        protected override void ToolUpdate()
        {
            intersectionPointPrefab.SetActive(Insertion);
            if (MeasureText == null) return;
            MeasureText.transform.LookAwayFrom(controller.CameraTransform(), Vector3.up);
            MeasureText.SetText(MeasureTape.TapeDistance(), MeasureTape.TapeName);
        }

        protected override void ToolStart()
        {
            if (Insertion || !ValidNodePlacement()) return;
            Placing = true;
            InsertNode(MeasureTape, dominant.transform.position, MeasureTape.measureNodes.Count);
        }

        protected override void ToolStay()
        {
            if (Insertion || !validNode) return;
//            node.transform.Position(dominant.transform);
            NodeSnap();
        }

        protected override void ToolEnd()
        {
            Placing = false;
            if (Insertion || !validNode) return;
            NodeSnap();
            ReleaseNode();
        }

        protected override void ToolInactive()
        {
            
        }

        private void NodeSnap()
        {
            if (!axisSnapping || PreviousMeasureNode == null)
            {
                SetNodePosition(dominant.transform);
                return;
            }

            if (PreviousMeasureNode.XSnap)
            {
                SetNodePosition(PreviousMeasureNode.X.transform);
                return;
            }
            if (PreviousMeasureNode.YSnap)
            {
                SetNodePosition(PreviousMeasureNode.Y.transform);
                return;
            }
            if (PreviousMeasureNode.ZSnap)
            {
                SetNodePosition(PreviousMeasureNode.Z.transform);
                return;
            }
            
            SetNodePosition(dominant.transform);
        }

        private void SetNodePosition(Transform target)
        {
            node.transform.LerpTransform(target, .5f);
            MeasureTape.TapeLr.SetPosition(MeasureTape.TapeLr.positionCount - 1, node.transform.position);
            MeasureTape.AdjustTape(); 
        }

        public void NewTape()
        {
            tapeCount++;
            Color color = Color.HSVToRGB(Random.Range(0f, 1f), 1, 1, true);
            
            tapeObject = new GameObject("Tape_" + tapeCount);
            tapeObject.transform.Transforms(dominant.transform);
            MeasureTape = tapeObject.AddComponent<MeasureTape>();
            MeasureTape.Controller = controller;
            MeasureTape.MeasureTool = this;

            PreviousMeasureNode = null;

            MeasureTape.TapeLr = tapeObject.AddComponent<LineRenderer>();
            MeasureTape.TapeLr.SetupLineRender(controller.doubleSidedLineRenderMat, tapeWidth, true);
            
            MeasureTape.SetColor(color);
            
            if (tapeCount > 1)
            {
                MeasureVisual.SetColor(color);
            }
            
            MeasureTape.TapeLr.positionCount = 0;
            MeasureTape.TapeName = tapeCount.ToString();
        }

        private bool ValidNodePlacement()
        {
            validNode = true;
            return validNode;
            
            if (PreviousMeasureNode == null)
            {
                validNode = true;
                return validNode;
            }

            bool valid = Vector3.Distance(dominant.transform.position, PreviousMeasureNode.transform.position) >= tolerance;

            validNode = valid;
            
            Debug.Log(validNode);
            
            return validNode;
        }
        public void InsertNode(MeasureTape tape, Vector3 position, int index)
        {
            node = Instantiate(tapeNodePrefab, position, Quaternion.identity, tapeObject.transform);
            
            MeasureNode = node.GetComponent<MeasureNode>();
            FocusMeasureNode = MeasureNode;
            
            FocusMeasureTape = tape;
            FocusMeasureTape.Controller = controller;
            
            MeasureNode.Initialise(this, controller, tape);
            MeasureNode.SetColor(FocusMeasureTape.tapeColor);
            
            tape.measureNodes.Insert(index, MeasureNode);
            
            tape.RefactorNodes();
            
            AddLineRenderNode(tape.TapeLr, position);
            tape.AdjustTape();

            if (index == 0) return;
            PreviousMeasureNode = tape.measureNodes[index - 1];
        }

        private void ReleaseNode()
        {

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
            int positionCount = lr.positionCount;
            positionCount++;
            lr.positionCount = positionCount;
            lr.SetPosition(positionCount - 1, position);
        }
        
        private static void RemoveLineRenderNode(LineRenderer lr)
        {
            int positionCount = lr.positionCount;
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
