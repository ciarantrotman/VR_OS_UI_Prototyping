using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Sirenix.OdinInspector;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace VR_Prototyping.Scripts
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(ObjectSelection))]
	public class Manipulation : MonoBehaviour
	{
		#region Inspector and Variables
		private GameObject player;
		
		private ObjectSelection c;
		
		private GameObject fM;
		[HideInInspector] public GameObject mP; // dual grab pos target
		
		[HideInInspector] public GameObject mRp; // dual grab rot target
		[HideInInspector] public GameObject mRc; // dual grab rot target
		
		[HideInInspector] public GameObject cR; // controller proxy
		private GameObject cPr; // controller proxy
		private GameObject oPr;	// object proxy
		private GameObject cOr; // controller original
		private GameObject oOr; // object original
		private GameObject tr;  // target
		private GameObject cFr; // follow
		[HideInInspector] public GameObject tSr;

		[HideInInspector] public GameObject cL; // controller proxy
		private GameObject cPl; // controller proxy
		private GameObject oPl;	// object proxy
		private GameObject cOl; // controller original
		private GameObject oOl; // object original
		private GameObject tl;  // target
		private GameObject cFl; // follow
		[HideInInspector] public GameObject tSl;
		
		private float initialDistance;
		private float m;
		private float z;
		
		private Quaternion pRot;
		private const float Scalar = 1f;

		[HideInInspector] public SphereCollider sCl;
		[HideInInspector] public SphereCollider sCr;

		public const string RTag = "Controller/Right";
		public const string LTag = "Controller/Left";
		
		public enum ManipulationType
		{
			Physics,
			Lerp		
		}
		
		[TabGroup("Grab Settings")] public ManipulationType manipulationType;
		[TabGroup("Grab Settings")] public bool directGrab;
		[TabGroup("Grab Settings")] [ShowIf("directGrab")] [Indent] [Range(0f, 1f)] [SerializeField] private float directGrabDistance = .5f;
		[TabGroup("Grab Settings")] public bool disableLeftGrab;
		[TabGroup("Grab Settings")] public bool disableRightGrab;
		[TabGroup("Rotation Settings")] public bool enableRotation;
		[TabGroup("Rotation Settings")] [ShowIf("enableRotation")] [Indent] [Range(1f, 10f)] public float rotationForce;
		
		[BoxGroup("Snapping")] [SerializeField] private bool distanceSnapping = true;
		[BoxGroup("Snapping")] [ShowIf("distanceSnapping")] [Indent] [Range(0f, 5f)] [SerializeField] private float snapDistance = 1f;
		[BoxGroup("Snapping")] [ShowIf("distanceSnapping")] [Indent] [SerializeField] private bool maximumDistance;
		
		#endregion
		private void Start () 
		{
			c = GetComponent<ObjectSelection>();
			player = c.gameObject;
			SetupGameObjects();
		}

		private void SetupGameObjects()
		{
			fM = new GameObject("Manipulation/Manipulation");
			mP = new GameObject("Manipulation/MidPoint/Position");
			mRc = new GameObject("Manipulation/MidPoint/Rotation");
			mRp = new GameObject("Manipulation/MidPoint/Rotation");
			
			tr = new GameObject("Manipulation/Target/Right");
			cFr = new GameObject("Manipulation/Controller/Follow/Right");
			cOr = new GameObject("Manipulation/Controller/Original/Right");
			cPr = new GameObject("Manipulation/Controller/Proxy/Right");
			tSr = new GameObject("Manipulation/Target/Scaled/Right");
			oPr = new GameObject("Manipulation/Object/Proxy/Right");
			oOr = new GameObject("Manipulation/Object/Original/Right");
			
			fM.transform.parent = player.transform;
			
			mP.transform.SetParent(fM.transform);
			mRc.transform.SetParent(fM.transform);
			mRp.transform.SetParent(fM.transform);
			
			tr.transform.SetParent(fM.transform);
			cFr.transform.SetParent(fM.transform);
			cOr.transform.SetParent(cFr.transform);
			cPr.transform.SetParent(cFr.transform);
			tSr.transform.SetParent(cPr.transform);
			oPr.transform.SetParent(cPr.transform);
			oOr.transform.SetParent(cPr.transform);
			
			tl = new GameObject("Manipulation/Target/Left");
			cFl = new GameObject("Manipulation/Controller/Follow/Left");
			cOl = new GameObject("Manipulation/Controller/Original/Left");
			cPl = new GameObject("Manipulation/Controller/Proxy/Left");
			tSl = new GameObject("Manipulation/Target/Scaled/Left");
			oPl = new GameObject("Manipulation/Object/Proxy/Left");
			oOl = new GameObject("Manipulation/Object/Original/Left");
			
			
			tl.transform.SetParent(fM.transform);
			cFl.transform.SetParent(fM.transform);
			cOl.transform.SetParent(cFl.transform);
			cPl.transform.SetParent(cFl.transform);
			tSl.transform.SetParent(cPl.transform);
			oPl.transform.SetParent(cPl.transform);
			oOl.transform.SetParent(cPl.transform);
			
			cR = new GameObject(RTag);
			cL = new GameObject(LTag);
			
			if(!directGrab) return;
			sCr = cR.AddComponent<SphereCollider>();
			sCl = cL.AddComponent<SphereCollider>();
			Setup.SphereCollider(sCr, true, directGrabDistance);
			Setup.SphereCollider(sCl, true, directGrabDistance);
		}
		private void Update()
		{				
			Set.SplitPosition(c.Controller.CameraTransform(), c.Controller.LeftControllerTransform(), cFl.transform);
			Set.SplitPosition(c.Controller.CameraTransform(), c.Controller.RightControllerTransform(), cFr.transform);
			Set.MidpointPosition(mP.transform, tSl.transform, tSr.transform, true);
			Set.Transforms(cR.transform, c.Controller.RightControllerTransform());
			Set.Transforms(cL.transform, c.Controller.LeftControllerTransform());
			
			FollowFocusObjects();
		}

		private void FollowFocusObjects()
		{
			if (c.lFocusObject != null)
			{
				Check.FocusObjectFollow(c.lFocusObject.transform, c.Controller.LeftControllerTransform(), tl.transform, tSl.transform, oOl.transform, cOl.transform, oPl.transform, c.Controller.LeftGrab());
			}

			if (c.rFocusObject != null)
			{
				Check.FocusObjectFollow(c.rFocusObject.transform, c.Controller.RightControllerTransform(), tr.transform, tSr.transform, oOr.transform, cOr.transform, oPr.transform, c.Controller.RightGrab());
			}
		}
		public void OnStart(Transform con)
		{
			switch (con == c.Controller.LeftControllerTransform())
			{
				case true:
					Check.GrabStart(cFl, cPl, tl, cOl, con);
					Set.Transforms(tl.transform, c.lFocusObject.transform);
					Set.Transforms(tSl.transform, tl.transform);
					Set.Position(oPl.transform, c.lFocusObject.transform);
					Set.Position(oOl.transform, c.lFocusObject.transform);
					break;
				case false:
					Check.GrabStart(cFr, cPr, tr, cOr, con);
					Set.Transforms(tr.transform, c.rFocusObject.transform);
					Set.Transforms(tSr.transform, tr.transform);
					Set.Position(oPr.transform, c.rFocusObject.transform);
					Set.Position(oOr.transform, c.rFocusObject.transform);
					break;
				default:
					throw new ArgumentException();
			}
		}

		public void OnStay(Transform con)
		{
			switch (con == c.Controller.LeftControllerTransform())
			{
				case true:
					ControllerFollowing(con, cFl, cPl, tl);
					tSl.transform.localPosition = new Vector3(0, 0, Set.MagnifiedDepth(cPl, cOl, oOl, tSl, snapDistance, c.selectionRange - c.selectionRange * .25f, maximumDistance));
					break;
				case false:
					ControllerFollowing(con, cFr, cPr, tr);
					tSr.transform.localPosition = new Vector3(0, 0, Set.MagnifiedDepth(cPr, cOr, oOr, tSr, snapDistance, c.selectionRange - c.selectionRange * .25f, maximumDistance));
					break;
				default:
					throw new ArgumentException();
			}
		}

		public void DualGrabStart(Transform target)
		{
			Set.MidpointPosition(mRp.transform, c.Controller.LeftControllerTransform(), c.Controller.RightControllerTransform(), true);
			pRot = mRp.transform.rotation;
			mRc.transform.rotation = target.rotation;
			mRc.transform.position = mRp.transform.position;
		}

		public void DualGrabStay(Rigidbody rb)
		{
			Set.MidpointPosition(mRp.transform, c.Controller.LeftControllerTransform(), c.Controller.RightControllerTransform(), true);

			var rotation = mRp.transform.rotation;
			
			Quaternion aRot = pRot * Quaternion.Inverse(rotation);
			mRc.transform.rotation = Quaternion.Inverse(Quaternion.LerpUnclamped(Quaternion.identity, aRot, Scalar)) * mRc.transform.rotation;
			mRc.transform.position = mRp.transform.position;

			rb.AddTorque(Vector3.Cross(rb.transform.forward, mRc.transform.forward) * rotationForce, ForceMode.Acceleration);
			rb.AddTorque(Vector3.Cross(rb.transform.right, mRc.transform.right) * rotationForce, ForceMode.Acceleration);
			rb.AddTorque(Vector3.Cross(rb.transform.up, mRc.transform.up) * rotationForce, ForceMode.Acceleration);

			pRot = rotation;
		}
		
		public static void DirectGrabStart(Rigidbody rigid, Transform target, Transform controller)
		{
			rigid.useGravity = false;
			rigid.velocity = Vector3.zero;
			target.SetParent(controller);
		}
		
		public static void DirectGrabEnd(Rigidbody rigid, Transform target, bool g, List<Vector3> pos, List<Vector3> rot, float f, LineRenderer lr)
		{
			rigid.useGravity = g;
			target.SetParent(null);
			rigid.AddForce(Set.Velocity(pos) * (f + f), ForceMode.VelocityChange);
			rigid.AddTorque(Set.AngularVelocity(rot) * (f + f), ForceMode.VelocityChange);
			lr.enabled = true;
		}
		
		public void OnEnd(Transform con)
		{
			switch (con == c.Controller.LeftControllerTransform())
			{
				case true:
					tl.transform.SetParent(null);
					break;
				case false:
					tr.transform.SetParent(null);
					break;
				default:
					throw new ArgumentException();
			}
		}
		
		private static void ControllerFollowing(Transform con, GameObject f, GameObject p, GameObject target)
		{
			f.transform.LookAt(con);
			p.transform.position = con.position;
			p.transform.LookAt(target.transform);
		}
	}
}