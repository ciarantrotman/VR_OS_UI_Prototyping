using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchTool : BaseTool
    {
        [FoldoutGroup("Sketch Tool References")] [SerializeField] private DirectSlider widthSlider;
        
        [BoxGroup("Sketch Tool Settings")] [SerializeField] private float minWidth;
        [BoxGroup("Sketch Tool Settings")] [SerializeField] private float maxWidth;
        [BoxGroup("Sketch Tool Settings")] [Space(5)] [SerializeField] private Material sketchMaterial;

        public Color color { get; set; }
        public float width { get; set; }
        
        private int sketchCount;
        private int position;
        
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
                
            }

            pTrigger = cTrigger;
        }
        
        private void SketchStart()
        {
            sketchCount++;
            sketchObject = new GameObject("Sketch_" + sketchCount);
            sketchObject.transform.position = dominant.transform.position;
            sketchLr = sketchObject.AddComponent<LineRenderer>();
            Setup.LineRender(sketchLr, sketchMaterial, minWidth, true);
        }

        private void SketchStay()
        {
            sketchLr.positionCount = position + 1;
            sketchLr.SetPosition(position, dominant.transform.position);
            position++;
        }
    }
}
