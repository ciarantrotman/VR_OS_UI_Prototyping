using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureTape : MonoBehaviour
    {
        public MeasureTool MeasureTool { get; set; }
        public ControllerTransforms Controller { get; set; }
        public List<MeasureNode> measureNodes = new List<MeasureNode>();
        public LineRenderer TapeLr { get; set; }
        public string TapeName { get; set; }

        private Vector3 _x;
        private Vector3 _xP;
        private bool _rSelectP;
        private bool _lSelectP;

        public Color tapeColor;

        public float TapeDistance()
        {
            var count = 0;
            var distance = 0f;
            var previousNode = Vector3.zero;
            
            foreach (var node in measureNodes)
            {
                var position = node.transform.position;
                
                TapeLr.SetPosition(count, position);
                previousNode = previousNode == Vector3.zero ? position : previousNode;
                node.Distance = Vector3.Distance(position, previousNode);
                switch (node.LockNode)
                {
                    case true:
                        node.Text.SetText("[L] Node: {0} - Distance: {1:2}", node.NodeIndex, node.Distance);
                        break;
                    default:
                        node.Text.SetText("Node: {0} - Distance: {1:2}", node.NodeIndex, node.Distance);
                        break;
                }

                previousNode = position;
                distance += node.Distance;
                count++;
            }

            return distance;
        }

        private void Update()
        {
            if (measureNodes.Count < 2 || MeasureTool == null || MeasureTool.MeasureTape != this) return;
            
            int index = 0;

            for (int i = 0; i < measureNodes.Count - 1; i++)
            {
                var currNodePos = measureNodes[i].transform.position;
                var nextNodePos = measureNodes[i + 1].transform.position;
                var line = measureNodes[i + 1].transform.position - measureNodes[i].transform.position;
                switch (MeasureTool.toolMenu.dominantHand)
                {
                    case ToolMenu.Handedness.Left:
                        _x = Intersection.Line(
                            Controller.LeftPosition(),
                            Controller.LeftForwardVector(),
                            currNodePos,
                            line,
                            MeasureTool.tolerance);
                        if (_x != Vector3.zero)
                        {
                            _x = Check.IsCollinear(currNodePos, nextNodePos, _x, MeasureTool.tolerance) ? 
                                    _x : 
                                    Vector3.zero;
                            
                            if (_x != Vector3.zero && !Controller.LeftSelect() && _lSelectP)
                            {
                                index = i + 1;
                            }
                        }
                        break;
                    case ToolMenu.Handedness.Right:
                        _xP = Intersection.Line(
                            Controller.RightPosition(),
                            Controller.RightForwardVector(),
                            currNodePos,
                            line,
                            MeasureTool.tolerance);
                        
                        if (_xP != Vector3.zero)
                        {
                            Debug.DrawRay(_xP, Vector3.up * .5f, Color.blue);

                            if (Check.IsCollinear(currNodePos, nextNodePos, _xP, MeasureTool.tolerance))
                            {
                                Debug.DrawRay(_xP, Vector3.down * .5f, Color.green);
                                _x = _xP;
                            }
                            
                            if (Check.IsCollinear(currNodePos, nextNodePos, _xP, MeasureTool.tolerance) && !Controller.RightSelect() && _rSelectP)
                            {
                                index = i + 1;
                            }
                        }
                        /*
                        else
                        {
                            _x = Vector3.zero;
                        }
                        */
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            Debug.DrawRay(_x, Vector3.forward * .5f, Color.red);
            
            MeasureTool.Insertion = _xP != Vector3.zero;
            
            if(!MeasureTool.Insertion) return;
            
            Debug.DrawRay(_x, Vector3.back * .5f, Color.yellow);

            MeasureTool.intersectionPointPrefab.transform.position = _x;
            
            switch (MeasureTool.toolMenu.dominantHand)
            {
                case ToolMenu.Handedness.Right when !Controller.RightSelect() && _rSelectP && index > 0 && Vector3.Distance(_x, Controller.RightPosition())  > .02f:// <= MeasureTool.nodeGrabDistance:
                    MeasureTool.InsertNode(this, _x, index);
                    break;
                case ToolMenu.Handedness.Left when !Controller.LeftSelect() && _lSelectP && index > 0 && Vector3.Distance(_x, Controller.LeftPosition()) > .02f:// <= MeasureTool.nodeGrabDistance:
                    MeasureTool.InsertNode(this, _x, index);
                    break;
            }

            _rSelectP = Controller.RightSelect();
            _lSelectP = Controller.LeftSelect();

            
        }
        
        public void SetTapeState(bool state)
        {
            foreach (var node in measureNodes)
            {
                node.LockNode = state;
            }
        }

        public void RefactorNodes()
        {
            int index = 0;
            foreach (var node in measureNodes)
            {
                node.NodeIndex = index;
                node.name = "Node_" + index;
                index++;
            }
            AdjustTape();
        }

        public void AdjustTape()
        {
            TapeDistance();
        }

        public void SetColor(Color color)
        {
            tapeColor = color;
            TapeLr.material.color = color;
            foreach (var node in measureNodes)
            {
                node.SetColor(color);
            }
        }
    }
}
