using System;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class InsertPointTEMP : MonoBehaviour
    {
        public ControllerTransforms con;

        public GameObject intersectionPoint;

        private Vector3 _x;
        
        public List<GameObject> nodes = new List<GameObject>();

        private bool _pS;

        private void Update()
        {
            var index = 0;
            var insertion = 0;

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var line = nodes[i + 1].transform.position - nodes[i].transform.position;
                Debug.DrawRay(nodes[i].transform.position, line, Color.red);

                var intersection = Intersection.Line(
                    con.RightControllerTransform().position,
                    con.RightForwardVector(),
                    nodes[i].transform.position,
                    line,
                    .05f);

                if (intersection != Vector3.zero)
                {
                    intersectionPoint.transform.position = 
                        Math.Abs(Vector3.Distance(nodes[i].transform.position, intersection) +
                                 Vector3.Distance(nodes[i + 1].transform.position, intersection) -
                                 Vector3.Distance(nodes[i].transform.position, nodes[i + 1].transform.position)) < .001f ? 
                            intersection : 
                            Vector3.zero;
                    
                    Debug.DrawRay(con.RightControllerTransform().position, intersection - con.RightControllerTransform().position, Color.green);

                    if (!con.RightSelect() && _pS)
                    {
                        Debug.Log(nodes[i].name + ", " + nodes[i + 1].name);
                        insertion = i + 1;
                    }
                }
            }
            
            if (!con.RightSelect() && _pS)
            {
                AddPoint(insertion);
            }

            _pS = con.RightSelect();
        }

        private void AddPoint(int index)
        {
            GameObject node = Instantiate(intersectionPoint);
            node.transform.position = intersectionPoint.transform.position;
            
            Debug.Log(nodes.Capacity);
            nodes.Capacity++;

            node.transform.name = "NODE_" + index;

            nodes.Insert(index, node);
        }
    }
}
