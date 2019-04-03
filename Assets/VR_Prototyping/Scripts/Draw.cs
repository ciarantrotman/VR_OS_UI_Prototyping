using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VR_Prototyping.Scripts
{
	public static class Draw
	{
		public static void BezierLineRenderer(LineRenderer lr, Vector3 p0, Vector3 p1, Vector3 p2, int segments)
		{
			lr.positionCount = segments;
			lr.SetPosition(0, p0);
			lr.SetPosition(segments - 1, p2);

			for (var i = 1; i < segments; i++)
			{
				var point = GetPoint(p0, p1, p2, i / (float) segments);
				lr.SetPosition(i, point);
			}
		}

		private static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
		{
			return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
		}

		public enum Orientation
		{
			Forward,
			Right,
			Up
		}
		
		public static void CircleLineRenderer(LineRenderer lr, float radius, Orientation orientation, int quality)
		{
			lr.positionCount = quality;
			lr.useWorldSpace = false;
			
			var angle = 0f;
			const float arcLength = 360f;
			
			for (var i = 0; i < quality; i++)
			{
				var x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
				var y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
				switch (orientation)
				{
					case Orientation.Forward:
						lr.SetPosition(i, new Vector3(x, y, 0));
						break;
					case Orientation.Right:
						lr.SetPosition(i, new Vector3(x, 0, y));
						break;
					case Orientation.Up:
						lr.SetPosition(i, new Vector3(0, x, y));
						break;
					default:
						throw new ArgumentException();
				}

				angle += arcLength / quality;
			}
		}
	}
}


/*
 * using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class LineRendererCurved : MonoBehaviour
{
	private LineRenderer _lr;
	private float _angle;
	private float _startAngle;
	private float _endAngle;
	private float _arcLength;
	private float _radius;
	
	public float Radius;
	public float StartAngle;
	public float EndAngle;
	public int Quality;
	
	public enum Orientation
	{
	Forward,
	Right,
	Down
	}

	public Orientation orientation;

	public void Start()
	{
		_lr = transform.GetComponent<LineRenderer>();
	}
	
	private void Update ()
	{
		_lr.positionCount = Quality;
		_angle = StartAngle;
		_arcLength = EndAngle - StartAngle;
		for (var i = 0; i < Quality; i++)
		{
			var x = Mathf.Sin(Mathf.Deg2Rad * _angle) * Radius;
			var y = Mathf.Cos(Mathf.Deg2Rad * _angle) * Radius;
			switch (orientation)
			{
				case Orientation.Forward:
					_lr.SetPosition(i, new Vector3(x, y, 0));
					break;
				case Orientation.Right:
					_lr.SetPosition(i, new Vector3(x, 0, y));
					break;
				case Orientation.Down:
					_lr.SetPosition(i, new Vector3(0, x, y));
					break;
				default:
					break;
			}
			_angle += (_arcLength / Quality);
		}
	}
}

 */
