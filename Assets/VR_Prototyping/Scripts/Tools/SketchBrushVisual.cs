using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SketchBrushVisual : MonoBehaviour
    {
        private SketchTool sketchTool;
        private MeshRenderer meshRenderer;

        private void Start()
        {
            sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            sketchTool.SketchVisual = this;
            meshRenderer = transform.GetComponent<MeshRenderer>();
        }

        public void SetVisual(Color c, float w)
        {
            transform.localScale = new Vector3(w,w,w);
            meshRenderer.material.color = c;
        }
    }
}
