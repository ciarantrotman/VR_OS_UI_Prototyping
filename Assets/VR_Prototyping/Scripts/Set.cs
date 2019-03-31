﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace VR_Prototyping.Scripts
{
    public static class Set
    {
        private static readonly int LeftHand = Shader.PropertyToID("_LeftHand");
        private static readonly int RightHand = Shader.PropertyToID("_RightHand");

        public static void Tag(string tag)
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");

            var found = false;
            for (var i = 0; i < tagsProp.arraySize; i++)
            {
                var t = tagsProp.GetArrayElementAtIndex(i);
                if (!t.stringValue.Equals(tag)) continue;
                found = true; break;
            }

            Debug.LogWarning(tag + " is already a tag!");
            
            if (found) return;
            
            Debug.Log(tag + " added to tags!");
            tagsProp.InsertArrayElementAtIndex(0);
            var n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tag;
        }
        
        public static void Position(Transform a, Transform b)
        {
            if (a == null || b == null) return;
            a.transform.position = b.transform.position;
        }
        public static void Rotation(Transform a, Transform b)
        {
            if (a == null || b == null) return;
            a.transform.rotation = b.transform.rotation;
        }
        
        public static void AddForceRotation(Rigidbody rb, Transform a, Transform b, float force)
        {
            if (a == null || b == null || rb == null) return;
            
            var r = Quaternion.FromToRotation(a.forward, b.forward);
            rb.AddTorque(r.eulerAngles * force, ForceMode.Force);
        }
        
        public static void SplitPosition(Transform xz, Transform y, Transform c)
        {
            if (xz == null || y == null || c == null) return;
            var position = xz.position;
            c.transform.position = new Vector3(position.x, y.position.y, position.z);
        }
        
        public static void SplitRotation(Transform controller, Transform target, bool follow)
        {
            if (controller == null || target == null) return;
            var c = controller.eulerAngles;
            target.transform.eulerAngles = new Vector3(0, c.y, 0);
            
            if(!follow) return;
            Position(target, controller);
        }
        
        public static void TransformLerpPosition(Transform a, Transform b, float l)
        {
            if (a == null || b == null) return;
            a.position = Vector3.Lerp(a.position, b.position, l);
        }
        
        public static void VectorLerpPosition(Transform a, Vector3 b, float l)
        {
            if (a == null) return;
            a.position = Vector3.Lerp(a.position, b, l);
        }
        
        public static void Transforms(Transform a, Transform b)
        {
            if (a == null || b == null) return;
            Position(a, b);
            Rotation(a, b);
        }

        public static float Midpoint(Transform a, Transform b)
        {
            if (a == null || b == null) return 0f;
            return Vector3.Distance(a.position, b.position) *.5f;
        }

        public static void ForwardVector(Transform a, Transform b)
        {
            if (a == null || b == null) return;

            a.forward = b.forward;
        }

        public static void MidpointPosition(Transform target, Transform a, Transform b, bool lookAt)
        {
            if (a == null || b == null) return;
            var posA = a.position;
            var posB = b.position;

            target.position = Vector3.Lerp(posA, posB, .5f);
            
            if (!lookAt) return;
            
            target.LookAt(b);
        }

        public static void AddForcePosition(Rigidbody rb, Transform a, Transform b, bool debug)
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
        public static Vector3 Velocity(List<Vector3> list)
        {
            return (list[list.Count - 1] - list[0]) / Time.deltaTime;
        }
        
        public static Vector3 AngularVelocity(List<Quaternion> list)
        {
            var rot = Quaternion.FromToRotation(list[list.Count - 1].eulerAngles, list[0].eulerAngles);
            return rot.eulerAngles / Time.deltaTime;
        }
        
        public static void RigidBody(Rigidbody rb, float force, float drag, bool stop, bool gravity)
        {
            rb.mass = force;
            rb.drag = drag;
            rb.angularDrag = drag;
            rb.velocity = stop? Vector3.zero : rb.velocity;
            rb.useGravity = gravity;
        }

        public static void ReactiveMaterial(Renderer r,  Transform leftHand, Transform rightHand)
        {
            var material = r.material;
            material.SetVector(LeftHand, leftHand.position);
            material.SetVector(RightHand, rightHand.position);
        }

        public static void LineRenderWidth(LineRenderer lr, float start, float end)
        {
            lr.startWidth = start;
            lr.endWidth = end;
        }

        public static Vector3 LocalScale(Vector3 originalScale, float factor)
        {
            return new Vector3(
                originalScale.x + originalScale.x * factor,
                originalScale.y + originalScale.y * factor,
                originalScale.z + originalScale.z * factor);
        }

        public static Vector3 LocalPosition(Vector3 originalPos, float factor)
        {
            return new Vector3(
                originalPos.x,
                originalPos.y,
                originalPos.z + factor);
        }

        public static void LocalDepth(Transform a, float z, bool lerp, float speed)
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

        public static void VisualState(Transform t, SelectableObject s, Vector3 scale, Vector3 pos, TMP_FontAsset font, Color color)
        {
            s.buttonText.font = font;
            s.Renderer.material.color = color;
        }

        public static Vector3 Offset(Transform a, Transform b)
        {
            var x = a.position;
            var y = b.position;
            var xN = new Vector3(x.x, 0, x.z);
            var yN = new Vector3(y.x, 0, y.z);
            
            return yN - xN;
        }
        
        public static float Divergence(Transform a, Transform b)
        {           
            return Vector3.Angle(a.forward, b.forward);
        }
        
        public static float MagnifiedDepth(GameObject conP, GameObject conO, GameObject objO, GameObject objP, float snapDistance, float max, bool limit)
        {
            var depth = conP.transform.localPosition.z / conO.transform.localPosition.z;
            var distance = Vector3.Distance(objO.transform.position, objP.transform.position);
				
            if (distance >= max && limit) return max;
            if (distance < snapDistance) return objO.transform.localPosition.z * Mathf.Pow(depth, 2);														
            return objO.transform.localPosition.z * Mathf.Pow(depth, 2.5f);
        }
    }
}
