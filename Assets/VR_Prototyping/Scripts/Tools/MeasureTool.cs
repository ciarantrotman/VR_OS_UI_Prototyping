
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureTool : BaseTool
    {        
        [BoxGroup("Tape Tool Settings")] [Required] public GameObject tapeNodePrefab;
        [BoxGroup("Tape Tool Settings")] [Range(.001f, .05f)] public float tapeWidth;
        [BoxGroup("Tape Tool Settings")] [Space(5)] public Material tapeMaterial;
        [BoxGroup("Tape Tool Settings")] [Space(5)] public Color tapeColor = new Color(0,0,0,255);
        
        public MeasureVisual MeasureVisual { private get; set; }
        public MeasureText MeasureText { private get; set; }
        
        private int tapeCount;
        
        private GameObject tapeObject;
        private GameObject node;
        private LineRenderer tapeLr;

        private Vector3 startPos = Vector3.zero;
        private float totalLength;

        protected override void Initialise()
        {
            NewTape();
        }

        protected override void ToolStart()
        {
            var position = dominant.transform.position;
            startPos = position;
            var positionCount = tapeLr.positionCount;
            positionCount++;
            tapeLr.positionCount = positionCount;
            tapeLr.SetPosition(positionCount - 1, position);
            CreateNode();
        }

        protected override void ToolStay()
        {            
            MeasureText.SetText(CurrentDistance(), totalLength, tapeCount);
            Set.Transforms(node.transform, dominant.transform);
            tapeLr.SetPosition(tapeLr.positionCount - 1, dominant.transform.position);
        }

        protected override void ToolEnd()
        {
            totalLength = totalLength + CurrentDistance();
            MeasureText.SetText(CurrentDistance(), totalLength, tapeCount);
            ReleaseNode();
        }

        protected override void ToolInactive()
        {
            
        }

        private float CurrentDistance()
        {
            return Vector3.Distance(startPos, dominant.transform.position);
        }

        public void NewTape()
        {
            tapeCount++;
            
            tapeObject = new GameObject("Tape_" + tapeCount);
            Set.Transforms(tapeObject.transform, dominant.transform);
            
            tapeLr = tapeObject.AddComponent<LineRenderer>();
            Setup.LineRender(tapeLr, tapeMaterial, tapeWidth, true);
            tapeLr.positionCount = 0;
            tapeLr.material.color = tapeColor;
            
            totalLength = 0f;
        }

        private void CreateNode()
        {
            node = Instantiate(tapeNodePrefab, tapeObject.transform, true);
            node.transform.position = dominant.transform.position;
        }

        private void ReleaseNode()
        {
            node = null;
        }
    }
}
