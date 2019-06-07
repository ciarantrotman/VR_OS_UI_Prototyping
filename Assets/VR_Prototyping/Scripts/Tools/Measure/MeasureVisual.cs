using UnityEngine;
using VR_Prototyping.Scripts.Tools.Measure;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureVisual : MonoBehaviour
    {
        private MeasureTool _measureTool;
        private MeshRenderer _meshRenderer;       
        
        private void Start()
        {
            _measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            _meshRenderer = transform.GetComponent<MeshRenderer>();
            _meshRenderer.material = _measureTool.tapeMaterial;
            _meshRenderer.material.color = _measureTool.tapeColor;
            _measureTool.MeasureVisual = this;
            SetColor(_measureTool.MeasureTape.tapeColor);
        }

        public void SetColor(Color color)
        {
            _meshRenderer.material.color = color;
        }
    }
}
