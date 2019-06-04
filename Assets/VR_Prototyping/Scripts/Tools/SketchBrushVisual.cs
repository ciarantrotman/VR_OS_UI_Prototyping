using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SketchBrushVisual : MonoBehaviour
    {
        private SketchTool sketchTool;
        private MeshRenderer meshRenderer;
        private TrailRenderer trail;

        private void Start()
        {
            sketchTool = transform.parent.transform.GetComponentInParent<SketchTool>();
            sketchTool.SketchVisual = this;
            meshRenderer = transform.GetComponent<MeshRenderer>();
            meshRenderer.material = sketchTool.sketchMaterial;
            
            trail = gameObject.AddComponent<TrailRenderer>();
            Setup.SetupTrailRender(trail, sketchTool.sketchMaterial, .2f, sketchTool.trailWidth, true);
        }

        public void SetVisual(Color c, float w)
        {
            transform.localScale = new Vector3(w,w,w);
            meshRenderer.material.color = c;

            trail.enabled = sketchTool.sketchTrail;
            Set.TrailRender(trail, w, 0, c);
        }
    }
}
