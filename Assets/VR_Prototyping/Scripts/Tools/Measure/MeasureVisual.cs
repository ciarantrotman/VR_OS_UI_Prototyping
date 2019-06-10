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
            measureTool.MeasureVisual = this;
            SetColor(measureTool.MeasureTape.tapeColor);
        }

        public void SetColor(Color color)
        {
            meshRenderer.material.color = color;
        }
    }
}
