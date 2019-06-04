using UnityEngine;
using VR_Prototyping.Plugins.QuickOutline.Scripts;

namespace VR_Prototyping.Scripts
{
    public static class Setup
    {
        public static LineRenderer AddOrGetLineRenderer(this Transform a)
        {
            return !a.GetComponent<LineRenderer>() ? a.gameObject.AddComponent<LineRenderer>() : a.gameObject.GetComponent<LineRenderer>();
        }
        public static void SetupLineRender(this LineRenderer lr, Material m, float w, bool e)
        {
            lr.material = m;
            lr.castShadows = false;
            lr.receiveShadows = false;
            lr.startWidth = w;
            lr.endWidth = w;
            lr.numCapVertices = 32;
            lr.useWorldSpace = true;
            lr.enabled = e;
        }
        
        public static void SetupTrailRender(this TrailRenderer tr, Material m, float time, AnimationCurve widthCurve, bool e)
        {
            tr.material = m;
            tr.minVertexDistance = .01f;
            tr.time = time;
            tr.widthCurve = widthCurve;
            tr.numCapVertices = 32;
            tr.enabled = e;
        }

        public static Rigidbody AddOrGetRigidbody(this Transform a)
        {
            return !a.GetComponent<Rigidbody>() ? a.gameObject.AddComponent<Rigidbody>() : a.gameObject.GetComponent<Rigidbody>();
        }
        
        public static Outline AddOrGetOutline(this Transform a)
        {
            return !a.GetComponent<Outline>() ? a.gameObject.AddComponent<Outline>() : a.gameObject.GetComponent<Outline>();
        }
        
        public static void SetupLineRenderObjects(this Transform m, Transform p, float offset)
        {
            m.position = p.position;
            m.parent = p;
            m.localRotation = new Quaternion(0,0,0,0);
            var position = m.position;
            m.localPosition = new Vector3(position.x, position.y, position.z + offset);
        }

        public static void SetupSphereCollider(this SphereCollider sc, bool trigger, float radius)
        {
            sc.isTrigger = trigger;
            sc.radius = radius;
        }
    }
}
