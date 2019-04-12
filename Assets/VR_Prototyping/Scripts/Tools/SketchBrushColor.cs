using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchBrushColor : DirectDial
    {
        private SketchTool sketchTool;

        private void Start()
        {
            sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            c = sketchTool.controller;
            
            SetupDial();
        }

        private void LateUpdate()
        {
            SetColor(dialValue);
        }

        private void SetColor(float colorValue)
        {
            sketchTool.SetColor(Color.HSVToRGB(colorValue, 1, 1, true));
        }
    }
}
