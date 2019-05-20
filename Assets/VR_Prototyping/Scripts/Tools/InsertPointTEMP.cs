using System;
using System.Collections.Generic;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class InsertPointTEMP : MonoBehaviour
    {
        public ControllerTransforms con;

        public GameObject intersectionPoint;

        private Vector3 _x;
        
        public List<GameObject> nodes = new List<GameObject>();
        
        private LineRenderer _lr;

        private bool pS;
        
        private void Start()
        {
            _lr = gameObject.AddComponent<LineRenderer>();
            Setup.LineRender(_lr, con.lineRenderMat, .01f, true);
            _lr.positionCount = nodes.Count;
        }

        private void Update()
        {
            var index = 0;
            foreach (var node in nodes)
            {
                _lr.SetPosition(index, node.transform.position);
                index++;
            }

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var line = nodes[i + 1].transform.position - nodes[i].transform.position;
                Debug.DrawRay(nodes[i].transform.position, line, Color.red);

                intersectionPoint.transform.position = Intersection.LineSegment(
                    con.RightControllerTransform().position,
                    con.RightForwardVector(),
                    nodes[i].transform.position,
                    line,
                    nodes[i+1].transform.position,
                    .05f);

                if (!(Vector3.Distance(con.RightControllerTransform().position, intersectionPoint.transform.position) < .5f)) continue;
                if (!con.RightSelect() && pS)
                {
                    AddPoint(i);
                }
            }

            pS = con.RightSelect();
        }

        private void AddPoint(int index)
        {
            _lr.positionCount++;

            var node = Instantiate(gameObject);
            nodes.Insert(index, node);
        }
    }
}
