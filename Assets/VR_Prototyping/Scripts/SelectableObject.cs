using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VR_Prototyping.Plugins.QuickOutline.Scripts;
using Object = UnityEngine.Object;

namespace VR_Prototyping.Scripts
{
	[DisallowMultipleComponent]
	public class SelectableObject : MonoBehaviour
	{
		#region Inspector and Variables
		private ObjectSelection c;
		private Manipulation f;
		private Outline outline;
		
		private Vector3 defaultPosition;
		private Vector3 defaultLocalPosition;
		private Vector3 defaultLocalScale;
		private Vector3 scaleMax;
		private Vector3 scaleMin;

		private bool active;
		
		private bool pGrabR;
		private bool pGrabL;

		private bool pDualGrab;
		
		private RotationLock rotLock;
		private float gazeAngle;
		private float manualRef;
		
		private Rigidbody rb;
		public float AngleL { get; private set; }
		public float AngleR { get; private set; }
		public Renderer Renderer { get; private set; }

		public Transform ResetState { get; private set; }
		
		public enum RotationLock
		{
			FreeRotation
		}
		private enum ButtonTrigger
		{
			OnButtonDown,
			OnButtonUp				
		}
		
		private readonly List<Vector3> positions = new List<Vector3>();
		private readonly List<Vector3> rotations = new List<Vector3>();
		private const float Sensitivity = 10f;
	
		[BoxGroup("Script Setup")] [SerializeField] [Required] private GameObject player;
		[Header("Define Object Behaviour")]
		[BoxGroup("Script Setup")] [HideIf("button")] [SerializeField] private bool grab;
		[BoxGroup("Script Setup")] [HideIf("grab")] [SerializeField] private bool button;
		[BoxGroup("Script Setup")] [ShowIf("button")] [SerializeField] [Indent] private bool startsActive;
		[BoxGroup("Script Setup")] [ShowIf("button")] [SerializeField] [Indent] private bool menu;
		[BoxGroup("Script Setup")] [ShowIf("button")] [ShowIf("menu")] [Indent(2)] public GameObject menuItems;
		
		[Header("Grab Settings")]
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [Range(0, 1f)] public float moveForce = .15f;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [Range(0, 10f)] public float latency = 4.5f;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")]  [SerializeField] private bool gravity;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("grab")] public bool directGrab = true;
		[Header("Rotation Settings")]
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [Space(5)] [SerializeField] public bool freeRotationEnabled;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [HideIf("freeRotationEnabled")] [Indent] [SerializeField] public RotationLock rotationLock;
		[Header("Scaling Settings")]
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [Space(5)] [SerializeField] public bool scalingEnabled;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("scalingEnabled")] [Indent] [Range(.01f, 1f)] public float minScaleFactor;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("scalingEnabled")] [Indent] [Range(1f, 10f)] public float maxScaleFactor;
		[Header("Visual Effects")]
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [Space(5)] [SerializeField] private bool genericGrabEffect;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [Indent] [SerializeField] private bool grabOutline;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("grabOutline")] [Indent(2)] [Range(1f, 10f)] [SerializeField] private float grabOutlineWidth;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("grabOutline")] [Indent(2)] [SerializeField] private Color grabOutlineColor = new Color(0,0,0,255);
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("grabOutline")] [Indent(2)] [SerializeField] private Outline.Mode grabOutlineMode;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("directGrab")] [Indent] [SerializeField] private bool touchOutline;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("directGrab")] [ShowIf("touchOutline")] [Indent(2)] [Range(1f, 10f)] [SerializeField] private float touchOutlineWidth;
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("directGrab")] [ShowIf("touchOutline")] [Indent(2)] [SerializeField] private Color touchOutlineColor = new Color(0,0,0,255);
		[BoxGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("directGrab")] [ShowIf("touchOutline")] [Indent(2)] [SerializeField] private Outline.Mode touchOutlineMode;
		
		[BoxGroup("Button Settings")] [ShowIf("button")] [Required] public TextMeshPro buttonText;
		[BoxGroup("Button Settings")] [ShowIf("button")] [Required] public Renderer buttonBack;
		[Header("Visual Effects")]
		[BoxGroup("Button Settings")] [ShowIf("button")] [Space(10)] [SerializeField] private bool genericSelectState;
		[BoxGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Range(0, 1f)] [SerializeField] private float selectOffset;
		[BoxGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Range(0, 1f)] [SerializeField] private float selectScale;
		[BoxGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Range(0, 1f)] [SerializeField] private float selectEffectDuration = .1f;
		[BoxGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Required] [SerializeField] private TMP_FontAsset activeFont;
		[BoxGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Required] [SerializeField] private TMP_FontAsset inactiveFont;
		[BoxGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [SerializeField] private Color activeColor = new Color(0,0,0,255);
		[BoxGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [SerializeField] private Color inactiveColor = new Color(0,0,0,255);
		[BoxGroup("Button Settings")] [ShowIf("button")] [Space(5)] private ButtonTrigger buttonTrigger;
		[BoxGroup("Button Settings")] [ShowIf("button")] [SerializeField] [Space(10)] private UnityEvent selectStart;
		[BoxGroup("Button Settings")] [ShowIf("button")] [SerializeField] private UnityEvent selectStay;
		[BoxGroup("Button Settings")] [ShowIf("button")] [SerializeField] private UnityEvent selectEnd;

		[BoxGroup("Hover Settings")] [SerializeField] private bool reactiveMat;
		[BoxGroup("Hover Settings")] [ShowIf("reactiveMat")] [SerializeField] [Indent] [Range(0, 1f)] private float clippingDistance;
		[BoxGroup("Hover Settings")] [SerializeField] private bool hover;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] public bool toolTip;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("toolTip")] [Indent] public string toolTipText = "Enter hover state text here.";
		[Header("Visual Effects")]
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [Space(5)] [Indent] [SerializeField] private bool genericHoverEffect;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [Indent(2)] [Range(0, 1f)] [SerializeField] private float hoverOffset;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [Indent(2)] [Range(0, 1f)] [SerializeField] private float hoverScale;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [Indent(2)] [Range(0, 1f)] [SerializeField] private float hoverEffectDuration;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [Indent] [SerializeField] private bool hoverOutline;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [ShowIf("hoverOutline")] [Indent(2)] [Range(1f, 10f)] [SerializeField] private float hoverOutlineWidth;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [ShowIf("hoverOutline")] [Indent(2)] [SerializeField] private Color hoverOutlineColor = new Color(0,0,0,255);
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [ShowIf("hoverOutline")] [Indent(2)] [SerializeField] private Outline.Mode hoverOutlineMode;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [HideIf("genericHoverEffect")] [Space(10)] [SerializeField] private UnityEvent hoverStart;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [HideIf("genericHoverEffect")] [SerializeField] private UnityEvent hoverStay;
		[BoxGroup("Hover Settings")] [ShowIf("hover")] [HideIf("genericHoverEffect")] [SerializeField] private UnityEvent hoverEnd;
		
		private static readonly int Threshold = Shader.PropertyToID("_ClipThreshold");
		#endregion
		private void Start ()
		{
			InitialiseSelectableObject();
			SetupScaling();
		}

		private void SetupScaling()
		{
			defaultLocalScale = transform.localScale;
			scaleMin = Set.ScaledScale(defaultLocalScale, minScaleFactor);
			scaleMax = Set.ScaledScale(defaultLocalScale, maxScaleFactor);
		}
		
		private void OnEnable()
		{
			InitialiseSelectableObject();
		}
		private void OnDisable()
		{
			var g = gameObject;
			ToggleList(g, c.gazeList);
			ToggleList(g, c.lHandList);
			ToggleList(g, c.rHandList);
		}
		private void InitialiseSelectableObject()
		{
			AssignComponents();
			SetupRigidBody();
			SetupManipulation();
			SetupOutline();
			ToggleList(gameObject, c.gazeList);
			
			if(!button) return;
			SetState(startsActive);
			active = startsActive;
			ResetState = transform;
		}
		private void AssignComponents()
		{
			c = player.GetComponent<ObjectSelection>();
			f = player.GetComponent<Manipulation>();
			Renderer = GetComponent<Renderer>();
		}
		private void SetupRigidBody()
		{
			rb = Setup.AddOrGetRigidbody(transform);
			rb.freezeRotation = true;
			rb.useGravity = !button && gravity;
		}
		private void SetupManipulation()
		{
			if (freeRotationEnabled)
			{
				rotationLock = RotationLock.FreeRotation;
			}
		}

		private void SetupOutline()
		{
			outline = Setup.AddOrGetOutline(transform);
			outline.enabled = false;
			outline.precomputeOutline = true;
		}
		private static void ToggleList(GameObject g, ICollection<GameObject> l)
		{
			if (l.Contains(g))
			{
				l.Remove(g);
			}
			else if (!l.Contains(g))
			{
				l.Add(g);
			}
		}
		private void Update()
		{		
			GetAngles();
			ReactiveMaterial();

			var o = gameObject;
			CheckGaze(o, gazeAngle, c.gaze, c.gazeList, c.lHandList, c.rHandList, c.globalList);
			ManageList(o, c.lHandList, CheckHand(o, c.gazeList, c.manual, AngleL,f.disableRightGrab, button), c.disableLeftHand, WithinRange(c.setSelectionRange, transform, c.Controller.LeftControllerTransform(), c.selectionRange));
			ManageList(o, c.rHandList, CheckHand(o, c.gazeList, c.manual, AngleR,f.disableLeftGrab, button), c.disableRightHand, WithinRange(c.setSelectionRange, transform, c.Controller.RightControllerTransform(), c.selectionRange));
		}

		private void OnTriggerEnter(Collider col)
		{
			if(!directGrab) return;
			
			switch (col.gameObject.name)
			{
				case Manipulation.RTag when !c.rTouch && !c.Controller.RightGrab():
					c.rTouch = true;
					c.rLr.enabled = false;
					c.rFocusObject = gameObject;
					break;
				case Manipulation.LTag when !c.lTouch && !c.Controller.LeftGrab():
					c.lTouch = true;
					c.lLr.enabled = false;
					c.lFocusObject = gameObject;
					break;
				default:
					return;
			}
			
			if (!touchOutline) return;
			Set.Outline(outline, touchOutlineMode, touchOutlineWidth, touchOutlineColor);
			outline.enabled = true;
		}
		
		private void OnTriggerStay(Collider col)
		{
			if(!directGrab) return;
			
			switch (col.gameObject.name)
			{
				case Manipulation.RTag when c.Controller.RightGrab() && !pGrabR && c.rFocusObject == gameObject:
					Manipulation.DirectGrabStart(rb, transform, f.cR.transform);
					break;
				case Manipulation.RTag when !c.Controller.RightGrab() && pGrabR && c.rFocusObject == gameObject:
					Manipulation.DirectGrabEnd(rb, transform, gravity, positions, rotations, moveForce, c.rLr);
					break;
				case Manipulation.LTag when c.Controller.LeftGrab() && !pGrabL && c.lFocusObject == gameObject:
					Manipulation.DirectGrabStart(rb, transform, f.cL.transform);
					c.rTouch = false;
					c.rFocusObject = null;
					break;
				case Manipulation.LTag when !c.Controller.LeftGrab() && pGrabL && c.lFocusObject == gameObject:
					Manipulation.DirectGrabEnd(rb, transform, gravity, positions, rotations, moveForce, c.lLr);
					c.lTouch = false;
					c.lFocusObject = null;
					break;
				default:
					Check.PositionTracking(positions, transform.position, Sensitivity);
					Check.RotationTracking(rotations, transform.forward, Sensitivity);
					break;
			}

			pGrabR = c.Controller.RightGrab();
			pGrabL = c.Controller.LeftGrab();
		}
		
		private void OnTriggerExit(Collider col)
		{
			if(!directGrab) return;
			
			switch (col.gameObject.name)
			{
				case Manipulation.RTag:
					c.rLr.enabled = true;
					c.rTouch = false;
					c.rFocusObject = null;
					break;
				case Manipulation.LTag:
					c.lLr.enabled = true;
					c.lTouch = false;
					c.lFocusObject = null;
					break;
				default:
					return;
			}
			
			outline.enabled = false;
		}

		private void ReactiveMaterial()
		{
			if (!reactiveMat) return;
			
			Renderer.material.SetFloat(Threshold, clippingDistance);
			Set.ReactiveMaterial(Renderer, c.Controller.LeftControllerTransform(), c.Controller.RightControllerTransform());
		}
		private void GetAngles()
		{
			var position = transform.position;
			gazeAngle = Vector3.Angle(position - c.Controller.CameraPosition(), c.Controller.CameraForwardVector());
			AngleL = Vector3.Angle(position - c.Controller.LeftControllerTransform().position, c.Controller.LeftForwardVector());
			AngleR = Vector3.Angle(position - c.Controller.RightControllerTransform().position, c.Controller.RightForwardVector());
		}
		private static void CheckGaze(GameObject o, float a, float c, ICollection<GameObject> g, ICollection<GameObject> l, ICollection<GameObject> r, ICollection<GameObject> global)
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
		private static bool CheckHand(GameObject g, ICollection<GameObject> gaze, float m, float c, bool b, bool button)
		{
			if (b && !button) return false;
			if (!gaze.Contains(g)) return false;
			return m > c / 2;
		}

		private static bool WithinRange(bool enabled, Transform self, Transform user, float range)
		{
			if (!enabled) return true;
			return Vector3.Distance(self.position, user.position) <= range;
		}

		private static void ManageList(GameObject g, ICollection<GameObject> l, bool b, bool d, bool r)
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

		private void SetState(bool a)
		{
			switch (a)
			{
				case true:
					Set.VisualState(transform, this, Set.LocalScale(defaultLocalScale, selectScale), Set.LocalPosition(defaultLocalPosition, selectOffset), activeFont, activeColor);
					break;
				case false:
					Set.VisualState(transform, this, defaultLocalScale, defaultLocalPosition, inactiveFont, inactiveColor);
					break;
				default:
					throw new ArgumentException();
			}
		}
		
		public void GrabStart(Transform con)
		{
			if (!grab) return;
			Set.RigidBody(rb, moveForce, latency,false, gravity);
			f.OnStart(con);
			
			if(!grabOutline) return;
			Set.Outline(outline, grabOutlineMode, grabOutlineWidth, grabOutlineColor);
			outline.enabled = true;
		}	
		public void GrabStay(Transform con)
		{
			if (!grab) return;
			
			f.OnStay(con);
			
			switch (f.manipulationType)
			{
				case Manipulation.ManipulationType.Lerp:
					if (DualGrab())
					{
						Set.TransformLerpPosition(transform, f.mP.transform, .1f);
						break;
					}
					if (c.Controller.RightGrab() && c.rSelectableObject == this)
					{
						Set.TransformLerpPosition(transform, f.tSr.transform, .1f);
						break;
					}
					if (c.Controller.LeftGrab() && c.lSelectableObject == this)
					{
						Set.TransformLerpPosition(transform, f.tSl.transform, .1f);
					}
					break;
				case Manipulation.ManipulationType.Physics:
					if (DualGrab() && !pDualGrab)
					{
						f.DualGrabStart(transform, freeRotationEnabled, scalingEnabled, maxScaleFactor, minScaleFactor);
					}
					if (DualGrab() && pDualGrab)
					{
						Set.AddForcePosition(rb, transform, f.mP.transform, c.Controller.debugActive);
						f.DualGrabStay(rb, transform, freeRotationEnabled, scalingEnabled, scaleMax, scaleMin);
						break;
					}
					if (!DualGrab() && pDualGrab)
					{
						f.DualGrabEnd();
					}
					if (c.Controller.RightGrab() && c.rSelectableObject == this)
					{
						Set.AddForcePosition(rb, transform, f.tSr.transform, c.Controller.debugActive);
						break;
					}
					if (c.Controller.LeftGrab() && c.lSelectableObject == this)
					{
						Set.AddForcePosition(rb, transform, f.tSl.transform, c.Controller.debugActive);
					}
					break;
				default:
					throw new ArgumentException();
			}
			
			pDualGrab = DualGrab();
		}
		private bool DualGrab()
		{
			return c.Controller.LeftGrab() && c.Controller.RightGrab() && c.lSelectableObject == this && c.rSelectableObject == this;
		}
		public void GrabEnd(Transform con)
		{
			c.gazeList.Clear();
			
			Set.RigidBody(rb, moveForce, latency,false, gravity);
			outline.enabled = false;
			
			f.OnEnd(con);
		}
		public void HoverStart()
		{
			hoverStart.Invoke();
			
			if (!genericHoverEffect || !hover) return;

			if (hoverOutline)
			{
				Set.Outline(outline, hoverOutlineMode, hoverOutlineWidth, hoverOutlineColor);
				outline.enabled = true;
			}
			
			var t = transform;
			defaultLocalScale = t.localScale;
			defaultLocalPosition = t.localPosition;

			t.DOScale(Set.LocalScale(defaultLocalScale, hoverScale), hoverEffectDuration);
			
			if (rb.velocity != Vector3.zero || hoverOffset <= 0) return;
			t.DOLocalMove(Set.LocalPosition(defaultLocalPosition, hoverOffset), hoverEffectDuration);
		}
		public void HoverStay()
		{
			hoverStay.Invoke();
		}
		public void HoverEnd()
		{
			hoverEnd.Invoke();
			
			outline.enabled = false;
			
			if (!genericHoverEffect || !hover) return;
			
			var t = transform;
			
			t.DOScale(defaultLocalScale, hoverEffectDuration);
			
			if (rb.velocity != Vector3.zero || hoverOffset <= 0) return;
			t.DOLocalMove(defaultLocalPosition, hoverEffectDuration);
		}
		public void SelectStart()
		{
			selectStart.Invoke();

			if (!genericSelectState || !button) return;
			switch (buttonTrigger)
			{
				case ButtonTrigger.OnButtonDown:
					active = !active;
					SetState(active);
					break;
				case ButtonTrigger.OnButtonUp:
					break;
				default:
					throw new ArgumentException();
			}
		}
		public void SelectStay()
		{
			selectStay.Invoke();
		}
		public void SelectEnd()
		{
			selectEnd.Invoke();
			
			if (!genericSelectState || !button) return;
			switch (buttonTrigger)
			{
				case ButtonTrigger.OnButtonDown:
					break;
				case ButtonTrigger.OnButtonUp:
					active = !active;
					SetState(active);
					break;
				default:
					throw new ArgumentException();
			}
		}
	}
}
