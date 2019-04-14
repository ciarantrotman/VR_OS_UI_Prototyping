using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchTool : BaseTool
    {        
        [BoxGroup("Sketch Tool Settings")] [Range(.001f, .05f)] public float minWidth;
        [BoxGroup("Sketch Tool Settings")] [Range(.05f, .1f)] public float maxWidth;
        [BoxGroup("Sketch Tool Settings")] [Space(5)] public Material sketchMaterial;
        [BoxGroup("Sketch Tool Settings")] [Space(5)] public bool sketchTrail;
        [BoxGroup("Sketch Tool Settings")] [Indent] [ShowIf("sketchTrail")] public AnimationCurve trailWidth;
        
        public SketchBrushVisual SketchVisual { private get; set; }

        private List<LineRenderer> sketches = new List<LineRenderer>();

        private Color brushColor;
        private float brushWidth;

        private int sketchCount;
        private int position = 1;
        
        private GameObject sketchObject;
        private LineRenderer sketchLr;

        private void LateUpdate()
        {
            if(!active) return;
            
            if (cTrigger && !pTrigger)
            {
                SketchStart();
            }

            if (cTrigger && pTrigger)
            {
                SketchStay();
            }

            if (!cTrigger && pTrigger)
            {
                SketchEnd();
            }

            pTrigger = cTrigger;
            
            if(SketchVisual == null) return;
            
            SketchVisual.SetVisual(brushColor, brushWidth);
        }
        
        private void SketchStart()
        {
            sketchCount++;
            sketchObject = new GameObject("Sketch_" + sketchCount);
            Set.Transforms(sketchObject.transform, dominant.transform);
            sketchLr = sketchObject.AddComponent<LineRenderer>();
            Setup.LineRender(sketchLr, sketchMaterial, brushWidth, true);
            sketchLr.material.color = brushColor;
            sketches.Add(sketchLr);
            position = 0;
            sketchTrail = false;
        }

        private void SketchStay()
        {
            sketchLr.positionCount = position + 1;
            sketchLr.SetPosition(position, dominant.transform.position);
            position++;
        }

        private void SketchEnd()
        {
            sketchLr.BakeMesh(new Mesh(), true);
            sketchObject = null;
            sketchLr = null;
            sketchTrail = true;
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
                Set.LineRenderWidth(sketchLr, brushWidth, brushWidth);
            }
        }
    }
}
