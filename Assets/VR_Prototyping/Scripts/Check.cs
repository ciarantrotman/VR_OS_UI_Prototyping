using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VR_Prototyping.Scripts
{
    public static class Check
    {
        public static void Manipulation(this Object focusObject, Object oppFocusObject, SelectableObject selectableObject, SelectableObject previous, bool grip, bool pGrip, Transform con, bool touch, bool oppTouch)
        {
            if (focusObject == null || selectableObject == null || touch) return;
         
            if (oppTouch && oppFocusObject  == focusObject) return;
            
            if (grip && !pGrip)
            {
                selectableObject.GrabStart(con);
            }
            if (grip && pGrip)
            {
                selectableObject.GrabStay(con);
            }
            if (!grip && pGrip)
            {
                previous.GrabEnd(con);
            }
        }
        
        public static void Selection(this Object focusObject, SelectableObject button, bool select, bool pSelect)
        {
            if (focusObject == null || button == null) return;
            if (select && !pSelect)
            {
                button.SelectStart();
            }
            if (select && pSelect)
            {
                button.SelectStay();
            }
            if (!select && pSelect)
            {
                button.SelectEnd();
            }
        }

        public static void Hover(this SelectableObject current, SelectableObject previous)
        {
            if (current != previous && current != null)
            {
                current.HoverStart();
            }
            if (current == previous && current != null)
            {
                current.HoverStay();
            }
            if (current != previous && previous != null)
            {
                previous.HoverEnd();
            }
        }

        public static void JoystickTracking(this List<Vector2> list, Vector2 current, float sensitivity)
        {
            list.Add(current);
            CullList(list, sensitivity);
        }
        
        public static void PositionTracking(this List<Vector3> list, Vector3 current, float sensitivity)
        {
            list.Add(current);
            CullList(list, sensitivity);
        }
        
        public static void RotationTracking(this List<Vector3> list, Vector3 current, float sensitivity)
        {
            list.Add(current);
            CullList(list, sensitivity);
        }

        
        private static void CullList(this IList list, float sensitivity)
        {
            if (list.Count > sensitivity)
            {
                list.RemoveAt(0);
            }
        }
        
        public static void GestureDetection(this Locomotion l, Vector2 current, Vector2 previous, float rot, float speed, float triggerValue, float toleranceValue, GameObject visual, LineRenderer lr, bool currentTouch, bool previousTouch, bool disabled, bool locomotionActive)
        {
            if (disabled) return;
            
            var trigger = Mathf.Abs(current.x) > triggerValue || Mathf.Abs(current.y) > triggerValue;
            var tolerance = Mathf.Abs(previous.x) - 0 <= toleranceValue && Mathf.Abs(previous.y) - 0 <= toleranceValue;
            var triggerEnd = Mathf.Abs(current.x) - 0 <= toleranceValue && Mathf.Abs(current.y) - 0 <= toleranceValue;
            var toleranceEnd = Mathf.Abs(previous.x) > triggerValue || Mathf.Abs(previous.y) > triggerValue;

            var latch = trigger && toleranceEnd;
            
            if ((trigger && tolerance && !locomotionActive && !latch) || (currentTouch && !previousTouch && !locomotionActive))
            {
                if (current.x > triggerValue)
                {
                    l.RotateUser(rot, speed);
                    
                }
                else if (current.x < -triggerValue)
                {
                    l.RotateUser(-rot, speed);
                }
                else if (current.y < -triggerValue)
                {
                    l.RotateUser(180f, speed);
                }
            }
            else if ((currentTouch && previousTouch && !locomotionActive) || (trigger && toleranceEnd && !locomotionActive))
            {
                l.LocomotionStart(visual, lr);
            }
            else if ((triggerEnd && toleranceEnd && locomotionActive) || (!currentTouch && previousTouch && locomotionActive))
            {
                l.LocomotionEnd(visual, visual.transform.position, visual.transform.eulerAngles, lr);
            }
        }
        
        public static void Target(this GameObject visual, GameObject parent, Transform normal, Vector2 pos, GameObject target, bool advanced)
        {
            visual.transform.LookAt(RotationTarget(pos, target, advanced));
            
            parent.transform.forward = normal.forward;
        }
        
        private static Transform RotationTarget(this Vector2 pos, GameObject target, bool advanced)
        {
            target.transform.localPosition = advanced? Vector3.Lerp(target.transform.localPosition, new Vector3(pos.x, 0, pos.y), .1f) : Vector3.forward;
            return target.transform;
        }
        
        public static float ControllerAngle(this GameObject follow, GameObject proxy, GameObject normal, Transform controller, Transform head, bool debug)
        {
            proxy.transform.Position(controller);
            proxy.transform.ForwardVector(controller);
            proxy.transform.SplitRotation(normal.transform, true);
            head.SplitPosition(controller, follow.transform);
            follow.transform.LookAt(proxy.transform);
            
            var normalDown = -normal.transform.up;
            var proxyForward = proxy.transform.forward;
            var position = proxy.transform.position;
            
            if (!debug) return Vector3.Angle(normalDown, proxyForward);
            
            Debug.DrawLine(follow.transform.position, position, Color.red);
            Debug.DrawRay(normal.transform.position, normalDown, Color.blue);
            Debug.DrawRay(position, proxyForward, Color.blue);

            return Vector3.Angle(normalDown, proxyForward);
        }
        
        public static float CalculateDepth(this float angle, float maxAngle, float minAngle, float max, float min, Transform proxy)
        {
            var a = angle;

            a = a > maxAngle ? maxAngle : a;
            a = a < minAngle ? minAngle : a;
            
            var proportion = Mathf.InverseLerp(maxAngle, minAngle, a);
            return Mathf.SmoothStep(max, min, proportion);
        }
        
        public static void TargetLocation(this GameObject target, GameObject hitPoint, Vector3 lastValidPosition)
        {
            var t = target.transform;
            var position = t.position;
            var up = t.up;
            hitPoint.transform.position = Vector3.Lerp(hitPoint.transform.position, Physics.Raycast(position, -up, out var hit) ? hit.point : lastValidPosition, .25f);
            hitPoint.transform.up = Physics.Raycast(position, -up, out var h) ? h.normal : Vector3.up;
        }

        public static GameObject RayCastFindFocusObject(this List<GameObject> objects, GameObject current, GameObject target, GameObject inactive, Transform controller, float distance, bool disable)
        {
            if (disable) return current == null ? null : current;
            
            var position = controller.position;
            var forward = controller.forward;

            if (Physics.Raycast(position, forward, out var hit, distance) && objects.Contains(hit.transform.gameObject))
            {
                target.transform.SetParent(hit.transform);
                target.transform.VectorLerpPosition(hit.point, .15f);
                return hit.transform.gameObject;
            }

            target.transform.SetParent(null);
            target.transform.TransformLerpPosition(inactive.transform, .1f);
            return null;
        }
        
        public static GameObject FuzzyFindFocusObject(this List<GameObject> objects, GameObject current, GameObject target, GameObject inactive, bool disable)
        {
            if (disable) return current == null ? null : current;
            
            target.transform.TransformLerpPosition(objects.Count > 0 ? objects[0].gameObject.transform: inactive.transform, .1f);
            return objects.Count > 0 ? objects[0].gameObject : null;
        }
        
        public static GameObject FusionFindFocusObject(this List<GameObject> objects, GameObject current, GameObject target,GameObject inactive, Transform controller, float distance, bool disable)
        {
            if (disable) return current == null ? null : current;
            
            var position = controller.position;
            var forward = controller.forward;
            
            if (Physics.Raycast(position, forward, out var hit, distance) && objects.Contains(hit.transform.gameObject))
            {
                target.transform.SetParent(hit.transform);
                target.transform.VectorLerpPosition(hit.point, .25f);
                return hit.transform.gameObject;
            }

            if (objects.Count > 0)
            {
                target.transform.SetParent(objects[0].gameObject.transform);
                target.transform.TransformLerpPosition(objects[0].gameObject.transform, .25f);
                return objects[0].gameObject;
            }
            
            target.transform.SetParent(null);
            target.transform.TransformLerpPosition(inactive.transform, .2f);
            return null;
        }

        public static SelectableObject FindSelectableObject(this GameObject focusObject, SelectableObject current, bool disable)
        {
            if (disable) return current == null ? null : current;
            
            if (focusObject == null) return null;
            return focusObject.GetComponent<SelectableObject>() != null ? focusObject.GetComponent<SelectableObject>() : null;
        }
        
        public static void DrawLineRenderer(this LineRenderer lr, GameObject focus, GameObject midpoint, Transform controller, GameObject target, int quality, bool grab)
        {
            midpoint.transform.localPosition = new Vector3(0, 0, controller.Midpoint(target.transform));
            lr.LineRenderWidth(.001f, focus != null ? .01f : 0f);
            
            Draw.BezierLineRenderer(lr, 
                controller.position,
                midpoint.transform.position, 
                target.transform.position,
                quality);
        }
        
        public static void GrabStart(this GameObject f, GameObject p, GameObject target, GameObject o, Transform con)
        {
            f.transform.LookAt(con);
            p.transform.position = con.position;
            p.transform.LookAt(target.transform);
            target.transform.SetParent(con);
            o.transform.position = con.position;
        }
        
        public static void FocusObjectFollow(this Transform focus, Transform con, Transform tar, Transform tarS, Transform objO, Transform conO, Transform objP, bool d)
        {
            if (focus.transform.gameObject == null || d) return;
			
            tar.Transforms(focus);
            tarS.Transforms(focus);
            objO.Transforms(focus);
            conO.Position(con);
            objP.Position(con);
        }

        public static bool IsCollinear(this Vector3 a, Vector3 b, Vector3 x, float tolerance)
        {
            return Math.Abs(Vector3.Distance(a, x) + Vector3.Distance(b, x) - Vector3.Distance(a, b)) < tolerance;
        }
        
        public static void CheckGaze(this GameObject o, float a, float c, ICollection<GameObject> g, ICollection<GameObject> l, ICollection<GameObject> r, ICollection<GameObject> global)
        {
            if (!global.Contains(o))
            {
                g.Remove(o);
                l.Remove(o);
                r.Remove(o);
            }
				
            if (a < c/2 && !g.Contains(o))
            {
                g.Add(o);
            }
			
            else if (a > c/2)
            {
                g.Remove(o);
                l.Remove(o);
                r.Remove(o);
            }
        }
        public static bool CheckHand(this GameObject g, ICollection<GameObject> gaze, float m, float c, bool b, bool button)
        {
            if (b && !button) return false;
            if (!gaze.Contains(g)) return false;
            return m > c / 2;
        }
        public static void ManageList(this GameObject g, ICollection<GameObject> l, bool b, bool d, bool r)
        {
            if (d || !r) return;
			
            if (b && !l.Contains(g))
            {
                l.Add(g);
            }
            else if (!b && l.Contains(g))
            {
                l.Remove(g);
            }
        }
    }
}
