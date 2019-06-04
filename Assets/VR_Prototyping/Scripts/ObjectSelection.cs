using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VR_Prototyping.Scripts
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(ControllerTransforms))]
	public class ObjectSelection : MonoBehaviour
	{
		#region 01 Inspector and Variables
		public ControllerTransforms Controller { get; private set; }
		private enum SelectionType
		{
			Fusion,
			Fuzzy,
			RayCast
		}
		private static bool TypeCheck(SelectionType type)
		{
			return type == SelectionType.Fusion;
		}
		private GameObject lTarget;
		private GameObject rTarget;
		private GameObject lDefault;
		private GameObject rDefault;
		private SelectableObject pLSelectableObject;
		private SelectableObject pRSelectableObject;

		[HideInInspector] public bool rTouch;
		[HideInInspector] public bool lTouch;
		[HideInInspector] public GameObject lMidPoint;
		[HideInInspector] public GameObject rMidPoint;
		[HideInInspector] public GameObject lFocusObject;
		[HideInInspector] public GameObject rFocusObject;
		[HideInInspector] public SelectableObject rSelectableObject;
		[HideInInspector] public SelectableObject lSelectableObject;
		[HideInInspector] public LineRenderer lLr;
		[HideInInspector] public LineRenderer rLr;
		[HideInInspector] public bool disableSelection;
		[HideInInspector] public bool lSelectPrevious;
		[HideInInspector] public bool rSelectPrevious;
		[HideInInspector] public bool lGrabPrevious;
		[HideInInspector] public bool rGrabPrevious;
		
		[ValidateInput("TypeCheck", "Recommended Selection Type is Fusion", InfoMessageType.Warning)]
		[BoxGroup("Selection Settings")] [SerializeField] private SelectionType selectionType;
		[BoxGroup("Selection Settings")] [HideIf("selectionType", SelectionType.RayCast)] [Indent] [Range(0f, 180f)] public float gaze = 60f;
		[BoxGroup("Selection Settings")] [HideIf("selectionType", SelectionType.RayCast)] [Indent] [Range(0f, 180f)] public float manual = 25f;
		[BoxGroup("Selection Settings")] [Space(10)]public bool setSelectionRange;		
		[BoxGroup("Selection Settings")] [ShowIf("setSelectionRange")] [Indent] [Range(0f, 250f)] public float selectionRange = 25f;		
		[BoxGroup("Selection Settings")] public bool disableLeftHand;
		[BoxGroup("Selection Settings")] public bool disableRightHand;
		
		[TabGroup("Object Lists")] public List<GameObject> globalList;
		[TabGroup("Object Lists")] public List<GameObject> gazeList;
		[TabGroup("Object Lists")] public List<GameObject> rHandList;
		[TabGroup("Object Lists")] public List<GameObject> lHandList;

		[TabGroup("Aesthetics")] [Range(3f, 30f)] public int lineRenderQuality = 15;
		[TabGroup("Aesthetics")] [Range(.1f, 2.5f)] public float defaultOffset = 1f;
		
		#endregion
		private void Start ()
		{
			Controller = GetComponent<ControllerTransforms>();
			
			SetupGameObjects();
			
			lLr = Controller.LeftTransform().gameObject.AddComponent<LineRenderer>();
			rLr = Controller.RightTransform().gameObject.AddComponent<LineRenderer>();
			
			lLr.SetupLineRender(Controller.lineRenderMat, .005f, true);
			rLr.SetupLineRender(Controller.lineRenderMat, .005f, true);
		}
		private void SetupGameObjects()
		{
			lMidPoint = new GameObject("MidPoint/Left");
			rMidPoint = new GameObject("MidPoint/Right");
			lTarget = new GameObject("TargetLineRender/Left");
			rTarget = new GameObject("Target/LineRender/Right");
			lDefault = new GameObject("Target/LineRender/Left/Default");
			rDefault = new GameObject("Target/LineRender/Right/Default");
			
			lTarget.transform.SetParent(transform);
			rTarget.transform.SetParent(transform);
						
			lDefault.transform.SetupLineRenderObjects(Controller.LeftTransform(), defaultOffset);
			rDefault.transform.SetupLineRenderObjects(Controller.RightTransform(), defaultOffset);
			
			lMidPoint.transform.SetupLineRenderObjects(Controller.LeftTransform(), 0f);
			rMidPoint.transform.SetupLineRenderObjects(Controller.RightTransform(), 0f);
		}

		private void FixedUpdate()
		{
			SortLists();

			switch (selectionType)
			{
				case SelectionType.Fuzzy:
					lFocusObject = lHandList.FuzzyFindFocusObject(lFocusObject, lTarget, lDefault, Controller.LeftGrab() || lTouch);
					rFocusObject = rHandList.FuzzyFindFocusObject(rFocusObject, rTarget, rDefault, Controller.RightGrab() || rTouch);
					break;
				case SelectionType.RayCast:
					lFocusObject = lHandList.RayCastFindFocusObject(lFocusObject, lTarget, lDefault, Controller.LeftTransform(), setSelectionRange ? selectionRange : float.PositiveInfinity, Controller.LeftGrab() || lTouch);
					rFocusObject = rHandList.RayCastFindFocusObject(rFocusObject, rTarget, rDefault, Controller.RightTransform(), setSelectionRange ? selectionRange : float.PositiveInfinity, Controller.RightGrab() || rTouch);
					break;
				case SelectionType.Fusion:
					lFocusObject = lHandList.FusionFindFocusObject(lFocusObject, lTarget, lDefault, Controller.LeftTransform(), setSelectionRange ? selectionRange : float.PositiveInfinity, Controller.LeftGrab() || lTouch);
					rFocusObject = rHandList.FusionFindFocusObject(rFocusObject, rTarget, rDefault, Controller.RightTransform(), setSelectionRange ? selectionRange : float.PositiveInfinity, Controller.RightGrab() || rTouch);
					break;
				default:
					lFocusObject = null;
					rFocusObject = null;
					break;
			}
			
			lLr.DrawLineRenderer(lFocusObject, lMidPoint, Controller.LeftTransform(), lTarget, lineRenderQuality, Controller.LeftGrab());
			rLr.DrawLineRenderer(rFocusObject, rMidPoint, Controller.RightTransform() ,rTarget, lineRenderQuality, Controller.RightGrab());
			
			lFocusObject.Manipulation(rFocusObject, lSelectableObject, pLSelectableObject, Controller.LeftGrab(), lGrabPrevious, Controller.LeftTransform(), lTouch, rTouch);
			rFocusObject.Manipulation(lFocusObject, rSelectableObject, pRSelectableObject, Controller.RightGrab(), rGrabPrevious, Controller.RightTransform(), rTouch, lTouch);
			
			lSelectableObject = lFocusObject.FindSelectableObject(lSelectableObject, Controller.LeftGrab());
			rSelectableObject = rFocusObject.FindSelectableObject(rSelectableObject, Controller.RightGrab());
			
			lGrabPrevious = Controller.LeftGrab();
			rGrabPrevious = Controller.RightGrab();
		}

		private void LateUpdate()
		{
			lFocusObject.Selection(lSelectableObject, Controller.LeftSelect(), lSelectPrevious);
			rFocusObject.Selection(rSelectableObject, Controller.RightSelect(), rSelectPrevious);
			
			lSelectableObject.Hover(pLSelectableObject);
			rSelectableObject.Hover(pRSelectableObject);
		
			lSelectPrevious = Controller.LeftSelect();
			rSelectPrevious = Controller.RightSelect();

			pLSelectableObject = lSelectableObject;
			pRSelectableObject = rSelectableObject;
		}

		private void SortLists()
		{
			lHandList.Sort(SortBy.FocusObjectL);
			rHandList.Sort(SortBy.FocusObjectR);
		}
	}
}
