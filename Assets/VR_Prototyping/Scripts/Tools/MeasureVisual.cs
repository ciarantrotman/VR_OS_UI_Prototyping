using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureVisual : MonoBehaviour
    {
        private MeasureTool measureTool;
        private MeshRenderer meshRenderer;       
        
        private void Start()
        {
            measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            measureTool.MeasureVisual = this;
            meshRenderer = transform.GetComponent<MeshRenderer>();
            meshRenderer.material = measureTool.tapeMaterial;
        }
    }
}
