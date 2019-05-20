using System;
using System.Collections.Generic;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureTape : MonoBehaviour
    {
        public MeasureTool MeasureTool { get; set; }
        public List<MeasureNode> measureNodes = new List<MeasureNode>();
        public LineRenderer TapeLr { get; set; }
        
        public string TapeName { get; set; }

        private Vector3 x;
        private bool _rSP;
        private bool _lSP;

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
                node.Text.SetText("Distance: {0:2}", node.Distance );
                previousNode = position;
                distance += node.Distance;
                count++;
            }

            return distance;
        }

        private void Update()
        {
            var index = 0;
            var insertion = 0;

            for (int i = 0; i < measureNodes.Count - 1; i++)
            {
                var line = measureNodes[i + 1].transform.position - measureNodes[i].transform.position;
                Debug.DrawRay(measureNodes[i].transform.position, line, Color.red);

                switch (MeasureTool.toolMenu.dominantHand)
                {
                    case ToolMenu.Handedness.Left:
                        x = Intersection.Line(
                            MeasureTool.controller.LeftControllerTransform().position,
                            MeasureTool.controller.LeftForwardVector(),
                            measureNodes[i].transform.position,
                            line,
                            .05f);

                        if (x != Vector3.zero)
                        {
                            x = 
                                Math.Abs(Vector3.Distance(measureNodes[i].transform.position, x) +
                                         Vector3.Distance(measureNodes[i + 1].transform.position, x) -
                                         Vector3.Distance(measureNodes[i].transform.position, measureNodes[i + 1].transform.position)) < .001f ? 
                                    x : 
                                    Vector3.zero;
                            
                            if (!MeasureTool.controller.LeftSelect() && _lSP)
                            {
                                insertion = i + 1;
                            }
                        }
                        break;
                    case ToolMenu.Handedness.Right:
                        x = Intersection.Line(
                            MeasureTool.controller.RightControllerTransform().position,
                            MeasureTool.controller.RightForwardVector(),
                            measureNodes[i].transform.position,
                            line,
                            .05f);

                        if (x != Vector3.zero)
                        {
                            x = 
                                Math.Abs(Vector3.Distance(measureNodes[i].transform.position, x) +
                                         Vector3.Distance(measureNodes[i + 1].transform.position, x) -
                                         Vector3.Distance(measureNodes[i].transform.position, measureNodes[i + 1].transform.position)) < .001f ? 
                                    x : 
                                    Vector3.zero;
                            
                            if (!MeasureTool.controller.RightSelect() && _rSP)
                            {
                                insertion = i + 1;
                            }
                        }
                        break;
                }
            }
            
            if (!MeasureTool.controller.RightSelect() && _rSP)
            {
                MeasureTool.InsertNode(this, x, insertion);
            }

            _rSP = MeasureTool.controller.RightSelect();
            _lSP = MeasureTool.controller.LeftSelect();
        }
        
        public void SetTapeState(bool state)
        {
            foreach (var node in measureNodes)
            {
                node.LockNode = state;
            }
        }

        public void AdjustTape()
        {
            TapeDistance();
        }
    }
}
