using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureTool : BaseTool
    {        
        [BoxGroup("Tape Tool Settings")] [Required] public GameObject tapeNodePrefab;
        [BoxGroup("Tape Tool Settings")] [Range(.001f, .05f)] public float tapeWidth;
        [BoxGroup("Tape Tool Settings")] [Space(5)] public Material tapeMaterial;
        
        public MeasureVisual MeasureVisual { private get; set; }
        public MeasureText MeasureText { private get; set; }
        
        private Color tapeColor;
        
        private int tapeCount;
        
        private int position = 1;
        
        private GameObject tapeObject;
        private GameObject node;
        private LineRenderer tapeLr;

        private Vector3 startPos;
        private float totalLength;

        protected override void ToolStart()
        {
            startPos = dominant.transform.position;
        }

        protected override void ToolStay()
        {            
            if (MeasureText == null || tapeLr == null || tapeLr.positionCount <= 2) return;
            MeasureText.SetText(CurrentDistance());
        }

        protected override void ToolEnd()
        {
            if (tapeCount == 0)
            {
                NewTape();
            }
            else
            {
                tapeLr.positionCount = position + 1;
                tapeLr.SetPosition(position, dominant.transform.position);
                position++;
                totalLength = totalLength + CurrentDistance();
                CreateNode();
            }
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
            tapeLr.material.color = tapeColor;
            totalLength = 0f;
        }

        private void CreateNode()
        {
            node = Instantiate(tapeNodePrefab, tapeObject.transform, true);
            node.transform.position = dominant.transform.position;
        }
    }
}
