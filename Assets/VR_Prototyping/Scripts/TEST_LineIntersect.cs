using UnityEngine;

namespace VR_Prototyping.Scripts
{
    public class TEST_LineIntersect : MonoBehaviour
    {
        public ControllerTransforms c;
        public Transform a;
        public Transform b;

        private LineRenderer lr;
        private LineRenderer jlr;

        public float tolerance;

        private void Start()
        {
            lr = Setup.AddOrGetLineRenderer(transform);
            jlr = Setup.AddOrGetLineRenderer(a);
            Setup.LineRender(lr, c.lineRenderMat, .01f, true);
            Setup.LineRender(jlr, c.lineRenderMat, .0075f, true);
        }

        private void Update()
        {
            Debug.DrawLine(a.position, b.position, Color.cyan);
            Debug.DrawRay(c.RightControllerTransform().transform.position, c.RightControllerTransform().transform.forward, Color.red);
            lr.SetPosition(0, c.RightControllerTransform().transform.position);
            lr.SetPosition(1, Intersection.Line(c.RightControllerTransform().position, c.RightControllerTransform().forward, a.position,(a.position - b.position), tolerance));
            jlr.SetPosition(0, a.position);
            jlr.SetPosition(1, b.position);
        }
    }
}
