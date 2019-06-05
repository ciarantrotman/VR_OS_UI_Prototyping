using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchTool : BaseTool
    {        
        [BoxGroup("Sketch Tool Settings")] [Range(.001f, .05f)] public float erasingDistance;
        [BoxGroup("Sketch Tool Settings")] [Range(.001f, .05f)] public float minWidth;
        [BoxGroup("Sketch Tool Settings")] [Range(.05f, .1f)] public float maxWidth;
        [BoxGroup("Sketch Tool Settings")] [Space(5)] public Material sketchMaterial;
        [BoxGroup("Sketch Tool Settings")] [Space(5)] public bool sketchTrail;
        [BoxGroup("Sketch Tool Settings")] [Indent] [ShowIf("sketchTrail")] public AnimationCurve trailWidth;

        public bool Erasing;
        
        public SketchBrushVisual SketchVisual { private get; set; }

        private List<LineRenderer> sketches = new List<LineRenderer>();

        private Color brushColor;
        private float brushWidth;

        private int sketchCount;
        private int position = 1;
        
        private GameObject sketchObject;
        private LineRenderer sketchLr;

        protected override void ToolUpdate()
        {
            if (!Erasing) return;
            foreach (var line in sketches)
            {
                for (var i = 0; i < line.positionCount; i++)
                {
                    if (EraseDistance(line, i) && cTrigger)
                    {
                        DeleteTape(line);
                    }
                }
            }
        }

        private bool EraseDistance(LineRenderer lr, int index)
        {
            return Vector3.Distance(lr.GetPosition(index), dominant.transform.position) <= erasingDistance;
        }

        protected override void ToolStart()
        {
            if (Erasing) return;
            NewTape();
        }

        protected override void ToolStay()
        {
            if (Erasing) return;
            sketchLr.positionCount = position + 1;
            sketchLr.SetPosition(position, dominant.transform.position);
            position++;
        }

        protected override void ToolEnd()
        {
            if (Erasing) return;
            sketchLr.BakeMesh(new Mesh(), true);
            sketchObject = null;
            sketchLr = null;
            sketchTrail = true;
        }

        protected override void ToolInactive()
        {
            if(SketchVisual == null) return;

            SketchVisual.SetVisual(brushColor, brushWidth);
        }

        private void NewTape()
        {
            sketchCount++;
            sketchObject = new GameObject("Sketch_" + sketchCount);
            sketchObject.transform.Transforms(dominant.transform);
            sketchLr = sketchObject.AddComponent<LineRenderer>();
            sketchLr.SetupLineRender(sketchMaterial, brushWidth, true);
            sketchLr.material.color = brushColor;
            sketchObject.AddComponent<SketchColor>().Color = brushColor;
            sketches.Add(sketchLr);
            position = 0;
            sketchTrail = false;
        }

        private void DeleteTape(LineRenderer line)
        {
            sketches.Remove(line);
            Destroy(line.gameObject);
        }
        
        public void SetColor(Color color)
        {
            brushColor = color;
        }

        public void SetWidth(float widthPercentage)
        {
            brushWidth = Mathf.Lerp(minWidth, maxWidth, widthPercentage);
            if (sketchLr != null)
            {
                sketchLr.LineRenderWidth(brushWidth, brushWidth);
            }
        }
    }
}
