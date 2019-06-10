using UnityEngine;

namespace VR_Prototyping.Scripts.Tools.Measure
{
    public class MeasureVisual : MonoBehaviour
    {
        private MeasureTool measureTool;
        private MeshRenderer meshRenderer;       
        
        private void Start()
        {
            measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            meshRenderer = transform.GetComponent<MeshRenderer>();
            meshRenderer.material = measureTool.tapeMaterial;
            meshRenderer.material.color = measureTool.tapeColor;
            measureTool.MeasureVisual = this;
            SetColor(measureTool.MeasureTape.tapeColor);
        }

        public void SetColor(Color color)
        {
            meshRenderer.material.color = color;
        }
    }
}
