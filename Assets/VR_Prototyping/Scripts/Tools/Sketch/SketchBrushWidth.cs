using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchBrushWidth : DirectSlider
    {
        private SketchTool sketchTool;

        private void Start()
        {
            sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            controller = sketchTool.controller;
            
            SetupSlider();
        }

        private void LateUpdate()
        {
            sketchTool.SetWidth(sliderValue);
        }
    }
}
