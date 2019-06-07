using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.UI_Blocks;

namespace VR_Prototyping.Scripts.Tools
{
    public class SketchBrushColor : DirectDial
    {
        private SketchTool sketchTool;
        private MeshRenderer dialCapMeshRenderer; 
        private MeshRenderer dialHandleMeshRenderer; 
        private  LineRenderer colorGuideCircle;

        private void Start()
        {
            sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            controller = sketchTool.controller;
            
            SetupDial();

            dialCapMeshRenderer = SetupRenderer(dialCap);
            dialHandleMeshRenderer = SetupRenderer(dialHandle);
            spokeLr.material = sketchTool.sketchMaterial;
            
            SetupColorGradient();
        }

        private MeshRenderer SetupRenderer(GameObject target)
        {
            MeshRenderer meshRenderer= target.GetComponent<MeshRenderer>();
            meshRenderer.material = sketchTool.sketchMaterial;
            return meshRenderer;
        }

        private void SetupColorGradient()
        {
            if (!sketchTool.gradientCircle) return;
            
            GameObject colorGuide = new GameObject();
            colorGuide.transform.parent = transform;
            colorGuide.transform.localPosition = Vector3.zero;
            colorGuideCircle = LineRender(colorGuide.transform, activeCircleLineRendererWidth);
            colorGuideCircle.CircleLineRenderer(dialRadius * .75f, Draw.Orientation.Right, circleQuality);
            colorGuideCircle.colorGradient = sketchTool.colorGradient;
        }
        
        private void LateUpdate()
        {
            SetColor(dialValue);
        }

        private void SetColor(float colorValue)
        {
            Color color = Color.HSVToRGB(colorValue, 1, 1, true);
            sketchTool.SetColor(color);
            
            dialCapMeshRenderer.sharedMaterial.color = color;
            dialHandleMeshRenderer.sharedMaterial.color = color;
            spokeLr.sharedMaterial.color = color;
        }
    }
}