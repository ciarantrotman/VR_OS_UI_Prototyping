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
	
		[FoldoutGroup("Script Setup")] public bool instantiated;
		[FoldoutGroup("Script Setup")] [Required] [HideIf("instantiated")] public GameObject player;
		[Header("Define Object Behaviour")]
		[FoldoutGroup("Script Setup")] [HideIf("button")] [SerializeField] private bool grab;
		[FoldoutGroup("Script Setup")] [HideIf("grab")] [SerializeField] private bool button;
		[FoldoutGroup("Script Setup")] [ShowIf("button")] [SerializeField] [Indent] private bool startsActive;
		[FoldoutGroup("Script Setup")] [ShowIf("button")] [SerializeField] [Indent] private bool menu;
		[FoldoutGroup("Script Setup")] [ShowIf("button")] [ShowIf("menu")] [Indent(2)] public GameObject menuItems;
		
		[Header("Grab Settings")]
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [Range(0, 1f)] public float moveForce = .15f;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [Range(0, 10f)] public float latency = 4.5f;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")]  [SerializeField] private bool gravity;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("grab")] public bool directGrab = true;
		[Header("Rotation Settings")]
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [Space(5)] [SerializeField] public bool freeRotationEnabled;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [HideIf("freeRotationEnabled")] [Indent] [SerializeField] public RotationLock rotationLock;
		[Header("Scaling Settings")]
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [Space(5)] [SerializeField] public bool scalingEnabled;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [HideInPlayMode] [ShowIf("scalingEnabled")] [Indent] [Range(.01f, 1f)] public float minScaleFactor;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [HideInPlayMode] [ShowIf("scalingEnabled")] [Indent] [Range(1f, 10f)] public float maxScaleFactor;
		[Header("Visual Effects")]
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [Space(5)] [SerializeField] private bool genericGrabEffect;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [Indent] [SerializeField] private bool grabOutline;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("grabOutline")] [Indent(2)] [Range(1f, 10f)] [SerializeField] private float grabOutlineWidth;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("grabOutline")] [Indent(2)] [SerializeField] private Color grabOutlineColor = new Color(0,0,0,255);
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("grabOutline")] [Indent(2)] [SerializeField] private Outline.Mode grabOutlineMode;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("directGrab")] [Indent] [SerializeField] private bool touchOutline;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("directGrab")] [ShowIf("touchOutline")] [Indent(2)] [Range(1f, 10f)] [SerializeField] private float touchOutlineWidth;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("directGrab")] [ShowIf("touchOutline")] [Indent(2)] [SerializeField] private Color touchOutlineColor = new Color(0,0,0,255);
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [ShowIf("genericGrabEffect")] [ShowIf("directGrab")] [ShowIf("touchOutline")] [Indent(2)] [SerializeField] private Outline.Mode touchOutlineMode;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [HideIf("genericGrabEffect")] [ShowIf("directGrab")] [Indent] public UnityEvent grabStart;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [HideIf("genericGrabEffect")] [ShowIf("directGrab")] [Indent] public UnityEvent grabStay;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [HideIf("genericGrabEffect")] [ShowIf("directGrab")] [Indent] public UnityEvent grabEnd;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [HideIf("genericGrabEffect")] [ShowIf("directGrab")] [Indent] public UnityEvent dualGrabStart;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [HideIf("genericGrabEffect")] [ShowIf("directGrab")] [Indent] public UnityEvent dualGrabStay;
		[FoldoutGroup("Manipulation Settings")] [ShowIf("grab")] [HideIf("genericGrabEffect")] [ShowIf("directGrab")] [Indent] public UnityEvent dualGrabEnd;
		
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [Required] public TextMeshPro buttonText;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [Required] public MeshRenderer buttonBack;
		[Header("Visual Effects")]
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [Space(10)] [SerializeField] private bool genericSelectState;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Range(0, 1f)] [SerializeField] private float selectOffset;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Range(0, 1f)] [SerializeField] private float selectScale;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Range(0, 1f)] [SerializeField] private float selectEffectDuration = .1f;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Required] [SerializeField] private TMP_FontAsset activeFont;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [Required] [SerializeField] private TMP_FontAsset inactiveFont;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [SerializeField] private Color activeColor = new Color(0,0,0,255);
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [ShowIf("genericSelectState")] [Indent] [SerializeField] private Color inactiveColor = new Color(0,0,0,255);
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [Space(5)] private ButtonTrigger buttonTrigger;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [Space(10)] public UnityEvent selectStart;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] public UnityEvent selectStay;
		[FoldoutGroup("Button Settings")] [ShowIf("button")] public UnityEvent selectEnd;

		[FoldoutGroup("Hover Settings")] [SerializeField] private bool reactiveMat;
		[FoldoutGroup("Hover Settings")] [ShowIf("reactiveMat")] [SerializeField] [Indent] [Range(0, 1f)] private float clippingDistance;
		[FoldoutGroup("Hover Settings")] [SerializeField] private bool hover;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] public bool toolTip;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("toolTip")] [Indent] public string toolTipText = "Enter hover state text here.";
		[Header("Visual Effects")]
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [Space(5)] [Indent] [SerializeField] private bool genericHoverEffect;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [Indent(2)] [Range(0, 1f)] [SerializeField] private float hoverOffset;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [Indent(2)] [Range(0, 1f)] [SerializeField] private float hoverScale;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [Indent(2)] [Range(0, 1f)] [SerializeField] private float hoverEffectDuration;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [Indent] [SerializeField] private bool hoverOutline;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [ShowIf("hoverOutline")] [Indent(2)] [Range(1f, 10f)] [SerializeField] private float hoverOutlineWidth;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [ShowIf("hoverOutline")] [Indent(2)] [SerializeField] private Color hoverOutlineColor = new Color(0,0,0,255);
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [ShowIf("genericHoverEffect")] [ShowIf("hoverOutline")] [Indent(2)] [SerializeField] private Outline.Mode hoverOutlineMode;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [HideIf("genericHoverEffect")] [Space(10)] [SerializeField] private UnityEvent hoverStart;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [HideIf("genericHoverEffect")] [SerializeField] private UnityEvent hoverStay;
		[FoldoutGroup("Hover Settings")] [ShowIf("hover")] [HideIf("genericHoverEffect")] [SerializeField] private UnityEvent hoverEnd;
		
		private static readonly int Threshold = Shader.PropertyToID("_ClipThreshold");

		#endregion
		private void Start ()
		{
			SetupScaling();
			InitialiseSelectableObject();
		}

		private void SetupScaling()
		{
			defaultLocalScale = transform.localScale;
			scaleMin = defaultLocalScale.ScaledScale(minScaleFactor);
			scaleMax = defaultLocalScale.ScaledScale(maxScaleFactor);
		}
		
		private void OnEnable()
		{
			InitialiseSelectableObject();
		}
		private void OnDisable()
		{
			var g = gameObject;
			ToggleList(g, c.globalList, false);
			ToggleList(g, c.gazeList, false);
			ToggleList(g, c.lHandList, false);
			ToggleList(g, c.rHandList, false);
		}
		private void InitialiseSelectableObject()
		{
			InitialiseOverride();
			
			AssignComponents();
			SetupRigidBody();
			SetupManipulation();
			SetupOutline();
			ToggleList(gameObject, c.globalList, true);
			ToggleList(gameObject, c.gazeList, true);
			
			if(!button) return;
			SetState(startsActive);
			active = startsActive;
			ResetState = transform;
		}

		protected virtual void InitialiseOverride()
		{
			
		}
		
		private void AssignComponents()
		{
			c = player.GetComponent<ObjectSelection>();
			f = player.GetComponent<Manipulation>();
			Renderer = GetComponent<Renderer>();
		}
		private void SetupRigidBody()
		{
			rb = transform.AddOrGetRigidbody();
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
			outline = transform.AddOrGetOutline();
			outline.enabled = false;
			outline.precomputeOutline = true;
		}
		private static void ToggleList(GameObject g, ICollection<GameObject> l, bool add)
		{
			switch (add)
			{
				case true when !l.Contains(g):
					l.Add(g);
					return;
				case false when l.Contains(g):
					l.Remove(g);
					return;
				default:
					return;
			}
		}
		private void Update()
		{					
			GetAngles();
			ReactiveMaterial();

			var o = gameObject;
			o.CheckGaze(gazeAngle, c.gaze, c.gazeList, c.lHandList, c.rHandList, c.globalList);
			o.ManageList(c.lHandList, o.CheckHand(c.gazeList, c.manual, AngleL,f.disableRightGrab, button), c.disableLeftHand, WithinRange(c.setSelectionRange, transform, c.Controller.LeftTransform(), c.selectionRange));
			o.ManageList(c.rHandList, o.CheckHand(c.gazeList, c.manual, AngleR,f.disableLeftGrab, button), c.disableRightHand, WithinRange(c.setSelectionRange, transform, c.Controller.RightTransform(), c.selectionRange));
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
			outline.Outline(touchOutlineMode, touchOutlineWidth, touchOutlineColor);
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
					positions.PositionTracking(transform.position, Sensitivity);
					rotations.RotationTracking(transform.forward, Sensitivity);
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
			Renderer.ReactiveMaterial(c.Controller.LeftTransform(), c.Controller.RightTransform());
		}
		private void GetAngles()
		{
			var position = transform.position;
			gazeAngle = Vector3.Angle(position - c.Controller.CameraPosition(), c.Controller.CameraForwardVector());
			AngleL = Vector3.Angle(position - c.Controller.LeftTransform().position, c.Controller.LeftForwardVector());
			AngleR = Vector3.Angle(position - c.Controller.RightTransform().position, c.Controller.RightForwardVector());
		}
		private static bool WithinRange(bool enabled, Transform self, Transform user, float range)
		{
			if (!enabled) return true;
			return Vector3.Distance(self.position, user.position) <= range;
		}
		public void SetState(bool a)
		{
			if (!genericSelectState) return;
			
			switch (a)
			{
				case true:
					transform.VisualState(this, defaultLocalScale.LocalScale(selectScale), defaultLocalPosition.LocalPosition(selectOffset), activeFont, activeColor);
					break;
				case false:
					transform.VisualState(this, defaultLocalScale, defaultLocalPosition, inactiveFont, inactiveColor);
					break;
				default:
					throw new ArgumentException();
			}
		}	
		public void GrabStart(Transform con)
		{
			if (!grab) return;
			rb.RigidBody(moveForce, latency,false, gravity);
			f.OnStart(con);
			
			if(!grabOutline) return;
			outline.Outline(grabOutlineMode, grabOutlineWidth, grabOutlineColor);
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
						transform.TransformLerpPosition(f.mP.transform, .1f);
						break;
					}
					if (c.Controller.RightGrab() && c.rSelectableObject == this)
					{
						transform.TransformLerpPosition(f.tSr.transform, .1f);
						break;
					}
					if (c.Controller.LeftGrab() && c.lSelectableObject == this)
					{
						transform.TransformLerpPosition(f.tSl.transform, .1f);
					}
					break;
				case Manipulation.ManipulationType.Physics:
					if (DualGrab() && !pDualGrab)
					{
						f.DualGrabStart(transform, freeRotationEnabled, scalingEnabled, maxScaleFactor, minScaleFactor, scaleMax, scaleMin);
					}
					if (DualGrab() && pDualGrab)
					{
						rb.AddForcePosition(transform, f.mP.transform, c.Controller.debugActive);
						f.DualGrabStay(rb, transform, freeRotationEnabled, scalingEnabled, scaleMin, scaleMax);
						break;
					}
					if (!DualGrab() && pDualGrab)
					{
						defaultLocalScale = transform.localScale;
						f.DualGrabEnd();
					}
					if (c.Controller.RightGrab() && c.rSelectableObject == this)
					{
						rb.AddForcePosition(transform, f.tSr.transform, c.Controller.debugActive);
						break;
					}
					if (c.Controller.LeftGrab() && c.lSelectableObject == this)
					{
						rb.AddForcePosition(transform, f.tSl.transform, c.Controller.debugActive);
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
			
			rb.RigidBody(moveForce, latency,false, gravity);
			outline.enabled = false;
			
			f.OnEnd(con);
		}
		public void HoverStart()
		{
			hoverStart.Invoke();
			
			if (!genericHoverEffect || !hover) return;

			if (hoverOutline)
			{
				outline.Outline(hoverOutlineMode, hoverOutlineWidth, hoverOutlineColor);
				outline.enabled = true;
			}
			
			var t = transform;
			defaultLocalScale = t.localScale;
			defaultLocalPosition = t.localPosition;

			t.DOScale(defaultLocalScale.LocalScale(hoverScale), hoverEffectDuration);
			
			if (rb.velocity != Vector3.zero || hoverOffset <= 0) return;
			t.DOLocalMove(defaultLocalPosition.LocalPosition(hoverOffset), hoverEffectDuration);
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
