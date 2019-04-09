using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts
{
    public class TestScale : MonoBehaviour
    {
        [FoldoutGroup("References")]public Transform a;
        [FoldoutGroup("References")]public Transform r;
        [FoldoutGroup("References")]public Transform l;
        [FoldoutGroup("References")]public Transform maxVis;
        [FoldoutGroup("References")]public Transform minVis;
        [FoldoutGroup("References")]public Transform startVis;

        [BoxGroup("Controls")]public float min;
        [BoxGroup("Controls")]public float max;

        private bool scaling;
    
        private Vector3 startScale;
        private Vector3 initialScale;
        private Vector3 scaleMin;
        private Vector3 scaleMax;
        private float startDistance;
        private float initialScaleFactor;
        private float minDistance;
        private float maxDistance;
        
        [Button]
        public void Toggle()
        {
            scaling = !scaling;

            if (!scaling) return;
            
            initialScale = a.localScale;
            initialScaleFactor = Mathf.InverseLerp(scaleMin.x, scaleMax.x, initialScale.x);
            Debug.Log(initialScaleFactor);

            startVis.localScale = initialScale;
            
            startDistance = Vector3.Distance(r.position,l.position);
            minDistance = startDistance * min;
            maxDistance = startDistance * max;
        }

        private void Start()
        {
            startScale = a.localScale;
            scaleMin = Set.ScaledScale(startScale, min);
            scaleMax = Set.ScaledScale(startScale, max);
            
            maxVis.localScale = scaleMax;
            minVis.localScale = scaleMin;
        }

        private void Update()
        {
            
            Debug.DrawRay(l.position, Vector3.up * maxDistance, Color.red);
            Debug.DrawRay(r.position, Vector3.up * minDistance, Color.blue);
            
            if (!scaling) return;

            var scaleFactor = Mathf.InverseLerp(minDistance, maxDistance, Vector3.Distance(l.position, r.position));// * initialScaleFactor);
            scaleFactor = scaleFactor <= 0 ? 0 : scaleFactor;
            scaleFactor = scaleFactor >= 1 ? 1 : scaleFactor;
            
            Debug.DrawRay(startVis.position, Vector3.up * scaleFactor, Color.red);

            a.transform.localScale = Vector3.Lerp(scaleMin, scaleMax, scaleFactor);
        }
    }
}
