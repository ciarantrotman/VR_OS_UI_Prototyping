using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using VR_Prototyping.Plugins.QuickOutline.Scripts;

namespace VR_Prototyping.Scripts
{
    public static class Set
    {
        private static readonly int LeftHand = Shader.PropertyToID("_LeftHand");
        private static readonly int RightHand = Shader.PropertyToID("_RightHand"); 
        
        public static void Position(this Transform a, Transform b)
        {
            if (a == null || b == null) return;
            a.transform.position = b.transform.position;
        }
        public static void Rotation(this Transform a, Transform b)
        {
            if (a == null || b == null) return;
            a.transform.rotation = b.transform.rotation;
        }

        public static void LocalTransformZero(Transform a)
        {
            var transform = a.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        
        public static void LookAtVertical(this Transform a, Transform b)
        {
            a.LookAwayFrom(b, Vector3.up);
            a.eulerAngles = new Vector3(0, a.eulerAngles.y,0);
        }
        
        public static void AddForceRotation(this Rigidbody rb, Transform a, Transform b, float force)
        {
            if (a == null || b == null || rb == null) return;
            
            var r = Quaternion.FromToRotation(a.forward, b.forward);
            rb.AddTorque(r.eulerAngles * force, ForceMode.Force);
        }
        
        public static void SplitPosition(this Transform xz, Transform y, Transform c)
        {
            if (xz == null || y == null || c == null) return;
            var position = xz.position;
            c.transform.position = new Vector3(position.x, y.position.y, position.z);
        }
        
        public static void SplitRotation(this Transform controller, Transform target, bool follow)
        {
            if (controller == null || target == null) return;
            var c = controller.eulerAngles;
            target.transform.eulerAngles = new Vector3(0, c.y, 0);
            
            if(!follow) return;
            Position(target, controller);
        }

        public static void TrailRender(this TrailRenderer tr, float start, float end, Color c)
        {
            tr.startWidth = start;
            tr.endWidth = end;
            tr.material.color = c;
        }
        
        public static void TransformLerpPosition(this Transform a, Transform b, float l)
        {
            if (a == null || b == null) return;
            a.position = Vector3.Lerp(a.position, b.position, l);
        }
        
        public static void TransformLerpRotation(this Transform a, Transform b, float l)
        {
            if (a == null || b == null) return;
            a.rotation = Quaternion.Lerp(a.rotation, b.rotation, l);
        }
        
        public static void VectorLerpPosition(this Transform a, Vector3 b, float l)
        {
            if (a == null) return;
            a.position = Vector3.Lerp(a.position, b, l);
        }
        
        public static void VectorLerpLocalPosition(this Transform a, Vector3 b, float l)
        {
            if (a == null) return;
            a.localPosition = Vector3.Lerp(a.localPosition, b, l);
        }
        
        public static void Transforms(this Transform a, Transform b)
        {
            if (a == null || b == null) return;
            Position(a, b);
            Rotation(a, b);
        }
        
        public static void LerpTransform(this Transform a, Transform b, float l)
        {
            if (a == null || b == null) return;
            TransformLerpPosition(a, b, l);
            TransformLerpRotation(a, b, l);
        }

        public static float Midpoint(this Transform a, Transform b)
        {
            if (a == null || b == null) return 0f;
            return Vector3.Distance(a.position, b.position) *.5f;
        }

        public static void ForwardVector(this Transform a, Transform b)
        {
            if (a == null || b == null) return;

            a.forward = b.forward;
        }

        public static void MidpointPosition(this Transform target, Transform a, Transform b, bool lookAt)
        {
            if (a == null || b == null) return;
            var posA = a.position;
            var posB = b.position;

            target.position = Vector3.Lerp(posA, posB, .5f);
            
            if (!lookAt) return;
            
            target.LookAt(b);
        }

        public static void AddForcePosition(this Rigidbody rb, Transform a, Transform b, bool debug)
        {
            if (a == null || b == null) return;
            
            var aPos = a.position;
            var bPos = b.position;
            var x = bPos - aPos;
            
            var d = Vector3.Distance(aPos, bPos);
            
            var p = Mathf.Pow(d, 1f);
            var y = p;
            
            if (debug)
            {
                Debug.DrawRay(aPos, -x * y, Color.cyan);   
                Debug.DrawRay(aPos, x * p, Color.yellow);
            }
            
            rb.AddForce(x, ForceMode.Force);
            
            if (!(d < 1f)) return;
            
            rb.AddForce(-x * d, ForceMode.Force);
        }
        public static Vector3 Velocity(this List<Vector3> list)
        {
            return (list[list.Count - 1] - list[0]) / Time.deltaTime;
        }
        
        public static Vector3 AngularVelocity(this List<Vector3> list)
        {
            return Vector3.Cross(list[list.Count - 1], list[0]);
        }
        
        public static void RigidBody(this Rigidbody rb, float force, float drag, bool stop, bool gravity)
        {
            rb.mass = force;
            rb.drag = drag;
            rb.angularDrag = drag;
            rb.velocity = stop? Vector3.zero : rb.velocity;
            rb.useGravity = gravity;
        }

        public static void ReactiveMaterial(this Renderer r,  Transform leftHand, Transform rightHand)
        {
            var material = r.material;
            material.SetVector(LeftHand, leftHand.position);
            material.SetVector(RightHand, rightHand.position);
        }

        public static void LineRenderWidth(this LineRenderer lr, float start, float end)
        {
            lr.startWidth = start;
            lr.endWidth = end;
        }

        public static Vector3 LocalScale(this Vector3 originalScale, float factor)
        {
            return new Vector3(
                originalScale.x + originalScale.x * factor,
                originalScale.y + originalScale.y * factor,
                originalScale.z + originalScale.z * factor);
        }

        public static Vector3 LocalPosition(this Vector3 originalPos, float factor)
        {
            return new Vector3(
                originalPos.x,
                originalPos.y,
                originalPos.z + factor);
        }

        public static void LocalDepth(this Transform a, float z, bool lerp, float speed)
        {
            if (a == null) return;
            var p = a.localPosition;

            switch (lerp)
            {
                case false:
                    a.localPosition = new Vector3(p.x,p.y, z);
                    break;
                case true:
                    Vector3.Lerp(a.localPosition, new Vector3(p.x, p.y, z), speed);
                    break;
                default:
                    throw new ArgumentException();
            }   
        }

        public static void VisualState(this Transform t, SelectableObject s, Vector3 scale, Vector3 pos, TMP_FontAsset font, Color color)
        {
            if (s.buttonText != null)
            {
                s.buttonText.font = font;
            }

            if (s.buttonBack != null)
            {
                s.buttonBack.material.color = color;
            }
        }

        public static Vector3 Offset(this Transform a, Transform b)
        {
            var x = a.position;
            var y = b.position;
            var xN = new Vector3(x.x, 0, x.z);
            var yN = new Vector3(y.x, 0, y.z);
            
            return yN - xN;
        }
        
        public static float Divergence(this Transform a, Transform b)
        {           
            return Vector3.Angle(a.forward, b.forward);
        }
        
        public static float MagnifiedDepth(this GameObject conP, GameObject conO, GameObject objO, GameObject objP, float snapDistance, float max, bool limit)
        {
            var depth = conP.transform.localPosition.z / conO.transform.localPosition.z;
            var distance = Vector3.Distance(objO.transform.position, objP.transform.position);
				
            if (distance >= max && limit) return max;
            if (distance < snapDistance) return objO.transform.localPosition.z * Mathf.Pow(depth, 2);														
            return objO.transform.localPosition.z * Mathf.Pow(depth, 2.5f);
        }

        public static void Outline(this Outline outline, Outline.Mode mode, float width, Color color)
        {
            outline.OutlineColor = color;
            outline.OutlineWidth = width;
            outline.OutlineMode = mode;
        }

        public static Vector3 ScaledScale(this Vector3 initialScale, float factor)
        {
            return new Vector3(
                initialScale.x * factor,
                initialScale.y * factor,
                initialScale.z * factor);
        }
        
        public static void LookAwayFrom(this Transform thisTransform, Transform transform, Vector3 upwards) 
        {
            thisTransform.rotation = Quaternion.LookRotation(thisTransform.position - transform.position, upwards);
        }

        public static void ReverseNormals(this MeshFilter filter)
        {
            var mesh = filter.mesh;
            var normals = mesh.normals;
            
            for (var i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (var m=0;m<mesh.subMeshCount;m++)
            {
                var triangles = mesh.GetTriangles(m);
                
                for (var i=0;i<triangles.Length;i+=3)
                {
                    var temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                
                mesh.SetTriangles(triangles, m);
            }
        }
        public static Vector3 LastValidPosition(this GameObject target, Vector3 lastValidPosition)
        {
            var t = target.transform;
            var position = t.position;
            var up = t.up;
            lastValidPosition = Physics.Raycast(position, -up, out var hit) ? hit.point : lastValidPosition;
            return lastValidPosition;
        }
    }
}
