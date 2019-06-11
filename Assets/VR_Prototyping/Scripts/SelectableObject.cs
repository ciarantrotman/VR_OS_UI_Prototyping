using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VR_Prototyping.Interfaces;
using VR_Prototyping.Plugins.QuickOutline.Scripts;
using Object = UnityEngine.Object;

namespace VR_Prototyping.Scripts
{
	[DisallowMultipleComponent]
	public abstract class SelectableObject : MonoBehaviour, ISelectableObject
	{
		#region Inspector and Variables

		private ObjectSelection objectSelection;
		private Manipulation manipulation;
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
			FREE_ROTATION
		}
		private enum ButtonTrigger
		{
			ON_BUTTON_DOWN,
			ON_BUTTON_UP				
		}
		
		private readonly List<Vector3> positions = new List<Vector3>();
		private readonly List<Vector3> rotations = new List<Vector3>();
		private const float Sensitivity = 10f;
	
		[BoxGroup("Script Setup")] public bool instantiated;
		[BoxGroup("Script Setup")] [Required] [HideIf("instantiated")] public GameObject player;
		[Header("Define Object Behaviour")]
		[BoxGroup("Script Setup")] [HideIf("button")] [SerializeField] private bool grab;
		[BoxGroup("Script Setup")] [HideIf("grab")] [SerializeField] private bool button;
		[BoxGroup("Script Setup")] [ShowIf("button")] [SerializeField] [Indent] private bool startsActive;
		[BoxGroup("Script Setup")] [ShowIf("button")] [SerializeField] [Indent] private bool menu;
		[BoxGroup("Script Setup")] [ShowIf("button")] [ShowIf("menu")] [Indent(2)] public GameObject menuItems;
		
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
		[FoldoutGroup("Button Settings")] [ShowIf("button")] [Space(5)] [SerializeField] private bool genericSelectState;
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
			InitialiseSelectableObject();
			Initialise();
		}
		protected virtual void Initialise()
		{
			
		}
		public void OnEnable()
		{
			InitialiseSelectableObject();
		}
		public void OnDisable()
		{
			GameObject g = gameObject;
			ToggleList(g, objectSelection.globalList, false);
			ToggleList(g, objectSelection.gazeList, false);
			ToggleList(g, objectSelection.lHandList, false);
			ToggleList(g, objectSelection.rHandList, false);
		}
		private void InitialiseSelectableObject()
		{
			InitialiseOverride();
			SetupScaling();
			AssignComponents();
			SetupRigidBody();
			SetupManipulation();
			SetupOutline();
			ToggleList(gameObject, objectSelection.globalList, true);
			ToggleList(gameObject, objectSelection.gazeList, true);
			
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
			objectSelection = player.GetComponent<ObjectSelection>();
			manipulation = player.GetComponent<Manipulation>();
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
				rotationLock = RotationLock.FREE_ROTATION;
			}
		}
		private void SetupOutline()
		{
			outline = transform.AddOrGetOutline();
			outline.enabled = false;
			outline.precomputeOutline = true;
		}
		private void SetupScaling()
		{
			defaultLocalScale = transform.localScale;
			scaleMin = defaultLocalScale.ScaledScale(minScaleFactor);
			scaleMax = defaultLocalScale.ScaledScale(maxScaleFactor);
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
			o.CheckGaze(gazeAngle, objectSelection.gaze, objectSelection.gazeList, objectSelection.lHandList, objectSelection.rHandList, objectSelection.globalList);
			o.ManageList(objectSelection.lHandList, o.CheckHand(objectSelection.gazeList, objectSelection.manual, AngleL,manipulation.disableRightGrab, button), objectSelection.disableLeftHand, WithinRange(objectSelection.setSelectionRange, transform, objectSelection.Controller.LeftTransform(), objectSelection.selectionRange));
			o.ManageList(objectSelection.rHandList, o.CheckHand(objectSelection.gazeList, objectSelection.manual, AngleR,manipulation.disableLeftGrab, button), objectSelection.disableRightHand, WithinRange(objectSelection.setSelectionRange, transform, objectSelection.Controller.RightTransform(), objectSelection.selectionRange));
		}
		private void ReactiveMaterial()
		{
			if (!reactiveMat) return;
			
			Renderer.material.SetFloat(Threshold, clippingDistance);
			Renderer.ReactiveMaterial(objectSelection.Controller.LeftTransform(), objectSelection.Controller.RightTransform());
		}
		private void OnTriggerEnter(Collider col)
		{
			if(!directGrab) return;
			
			switch (col.gameObject.name)
			{
				case Manipulation.RTag when !objectSelection.rTouch && !objectSelection.Controller.RightGrab():
					objectSelection.rTouch = true;
					objectSelection.rLr.enabled = false;
					objectSelection.rFocusObject = gameObject;
					break;
				case Manipulation.LTag when !objectSelection.lTouch && !objectSelection.Controller.LeftGrab():
					objectSelection.lTouch = true;
					objectSelection.lLr.enabled = false;
					objectSelection.lFocusObject = gameObject;
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
				case Manipulation.RTag when objectSelection.Controller.RightGrab() && !pGrabR && objectSelection.rFocusObject == gameObject:
					Manipulation.DirectGrabStart(rb, transform, manipulation.cR.transform);
					break;
				case Manipulation.RTag when !objectSelection.Controller.RightGrab() && pGrabR && objectSelection.rFocusObject == gameObject:
					Manipulation.DirectGrabEnd(rb, transform, gravity, positions, rotations, moveForce, objectSelection.rLr);
					break;
				case Manipulation.LTag when objectSelection.Controller.LeftGrab() && !pGrabL && objectSelection.lFocusObject == gameObject:
					Manipulation.DirectGrabStart(rb, transform, manipulation.cL.transform);
					objectSelection.rTouch = false;
					objectSelection.rFocusObject = null;
					break;
				case Manipulation.LTag when !objectSelection.Controller.LeftGrab() && pGrabL && objectSelection.lFocusObject == gameObject:
					Manipulation.DirectGrabEnd(rb, transform, gravity, positions, rotations, moveForce, objectSelection.lLr);
					objectSelection.lTouch = false;
					objectSelection.lFocusObject = null;
					break;
				default:
					positions.PositionTracking(transform.position, Sensitivity);
					rotations.RotationTracking(transform.forward, Sensitivity);
					break;
			}

			pGrabR = objectSelection.Controller.RightGrab();
			pGrabL = objectSelection.Controller.LeftGrab();
		}
		private void OnTriggerExit(Collider col)
		{
			if(!directGrab) return;
			
			switch (col.gameObject.name)
			{
				case Manipulation.RTag:
					objectSelection.rLr.enabled = true;
					objectSelection.rTouch = false;
					objectSelection.rFocusObject = null;
					break;
				case Manipulation.LTag:
					objectSelection.lLr.enabled = true;
					objectSelection.lTouch = false;
					objectSelection.lFocusObject = null;
					break;
				default:
					return;
			}
			
			outline.enabled = false;
		}
		private void GetAngles()
		{
			Vector3 position = transform.position;
			gazeAngle = Vector3.Angle(position - objectSelection.Controller.CameraPosition(), objectSelection.Controller.CameraForwardVector());
			AngleL = Vector3.Angle(position - objectSelection.Controller.LeftTransform().position, objectSelection.Controller.LeftForwardVector());
			AngleR = Vector3.Angle(position - objectSelection.Controller.RightTransform().position, objectSelection.Controller.RightForwardVector());
		}
		private static bool WithinRange(bool enabled, Transform self, Transform user, float range)
		{
			if (!enabled) return true;
			return Vector3.Distance(self.position, user.position) <= range;
		}
		public void SetState(bool state)
		{
			if (!genericSelectState) return;
			
			switch (state)
			{
				case true:
					transform.VisualState(this, defaultLocalScale.LocalScale(selectScale), defaultLocalPosition.LocalPosition(selectOffset), activeFont, activeColor);
					break;
				case false:
					transform.VisualState(this, defaultLocalScale, defaultLocalPosition, inactiveFont, inactiveColor);
					break;
			}
		}	
		public void GrabStart(Transform con)
		{
			if (!grab) return;
			rb.RigidBody(moveForce, latency,false, gravity);
			manipulation.OnStart(con);
			
			if(!grabOutline) return;
			outline.Outline(grabOutlineMode, grabOutlineWidth, grabOutlineColor);
			outline.enabled = true;
		}	
		public void GrabStay(Transform con)
		{
			if (!grab) return;
			
			manipulation.OnStay(con);
			
			switch (manipulation.manipulationType)
			{
				case Manipulation.ManipulationType.Lerp:
					if (DualGrab())
					{
						transform.TransformLerpPosition(manipulation.mP.transform, .1f);
						break;
					}
					if (objectSelection.Controller.RightGrab() && objectSelection.rSelectableObject == this)
					{
						transform.TransformLerpPosition(manipulation.tSr.transform, .1f);
						break;
					}
					if (objectSelection.Controller.LeftGrab() && objectSelection.lSelectableObject == this)
					{
						transform.TransformLerpPosition(manipulation.tSl.transform, .1f);
					}
					break;
				case Manipulation.ManipulationType.Physics:
					if (DualGrab() && !pDualGrab)
					{
						manipulation.DualGrabStart(transform, freeRotationEnabled, scalingEnabled, maxScaleFactor, minScaleFactor, scaleMax, scaleMin);
					}
					if (DualGrab() && pDualGrab)
					{
						rb.AddForcePosition(transform, manipulation.mP.transform, objectSelection.Controller.debugActive);
						manipulation.DualGrabStay(rb, transform, freeRotationEnabled, scalingEnabled, scaleMin, scaleMax);
						break;
					}
					if (!DualGrab() && pDualGrab)
					{
						defaultLocalScale = transform.localScale;
						manipulation.DualGrabEnd();
					}
					if (objectSelection.Controller.RightGrab() && objectSelection.rSelectableObject == this)
					{
						rb.AddForcePosition(transform, manipulation.tSr.transform, objectSelection.Controller.debugActive);
						break;
					}
					if (objectSelection.Controller.LeftGrab() && objectSelection.lSelectableObject == this)
					{
						rb.AddForcePosition(transform, manipulation.tSl.transform, objectSelection.Controller.debugActive);
					}
					break;
				default:
					throw new ArgumentException();
			}
			pDualGrab = DualGrab();
		}
		private bool DualGrab()
		{
			return objectSelection.Controller.LeftGrab() && objectSelection.Controller.RightGrab() && objectSelection.lSelectableObject == this && objectSelection.rSelectableObject == this;
		}
		public void GrabEnd(Transform con)
		{
			objectSelection.gazeList.Clear();
			
			rb.RigidBody(moveForce, latency,false, gravity);
			outline.enabled = false;
			
			manipulation.OnEnd(con);
		}

		void ISelectableObject.GrabStart()
		{
			throw new NotImplementedException();
		}
		void ISelectableObject.GrabStay()
		{
			throw new NotImplementedException();
		}
		void ISelectableObject.GrabEnd()
		{
			throw new NotImplementedException();
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
				case ButtonTrigger.ON_BUTTON_DOWN:
					active = !active;
					SetState(active);
					break;
				case ButtonTrigger.ON_BUTTON_UP:
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
				case ButtonTrigger.ON_BUTTON_DOWN:
					break;
				case ButtonTrigger.ON_BUTTON_UP:
					active = !active;
					SetState(active);
					break;
				default:
					throw new ArgumentException();
			}
		}
	}
}
