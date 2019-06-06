using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchBrushColor : DirectDial
    {
        private SketchTool sketchTool;
        private MeshRenderer dialCapMeshRenderer; 
        
        private  LineRenderer colorGuideCircle;
        
        [BoxGroup][SerializeField] private Gradient colorGradient;
        
        private static readonly int MatColor = Shader.PropertyToID("_Color");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private void Start()
        {
            sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            c = sketchTool.controller;
            
            SetupDial();

            dialCapMeshRenderer = dialCap.GetComponent<MeshRenderer>();
            dialCapMeshRenderer.material = sketchTool.sketchMaterial;
            
            SetupColorGradient();
        }

        private void SetupColorGradient()
        {
            var colorGuide = new GameObject();
            colorGuide.transform.parent = transform;
            colorGuideCircle = LineRender(colorGuide.transform, activeCircleLineRendererWidth);
            colorGuideCircle.CircleLineRenderer(dialRadius * .75f, Draw.Orientation.Right, circleQuality);
            colorGuideCircle.colorGradient = colorGradient;
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

            spokeLr.material.SetColor(MatColor, color);
            spokeLr.material.SetColor(EmissionColor, color);
        }
    }
}