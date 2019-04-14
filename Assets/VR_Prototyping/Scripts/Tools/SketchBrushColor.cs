using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchBrushColor : DirectDial
    {
        private SketchTool sketchTool;
        private MeshRenderer dialCapMeshRenderer;

        private void Start()
        {
            sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            c = sketchTool.controller;
            
            SetupDial();

            dialCapMeshRenderer = dialCap.GetComponent<MeshRenderer>();
            dialCapMeshRenderer.material = sketchTool.sketchMaterial;
        }

        private void LateUpdate()
        {
            SetColor(dialValue);
        }

        private void SetColor(float colorValue)
        {
            var color = Color.HSVToRGB(colorValue, 1, 1, true);
            sketchTool.SetColor(color);
            dialCapMeshRenderer.sharedMaterial.color = color;
        }
    }
}
