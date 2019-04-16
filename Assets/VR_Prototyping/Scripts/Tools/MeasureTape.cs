using System.Collections.Generic;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureTape : MonoBehaviour
    {
        public MeasureTool MeasureTool { get; set; }
        public List<MeasureNode> measureNodes = new List<MeasureNode>();
        public LineRenderer TapeLr { get; set; }
        
        public float Distance { get; set; }   

        public void SetTapeState(bool state)
        {
            foreach (var node in measureNodes)
            {
                node.LockNode = state;
            }
        }

        public void AdjustTape()
        {
            var count = 0;
            Distance = 0;
            var previousNode = Vector3.zero;
            foreach (var node in measureNodes)
            {
                var position = node.transform.position;
                TapeLr.SetPosition(count, position);
                previousNode = previousNode == Vector3.zero ? position : previousNode;
                node.Distance = Vector3.Distance(position, previousNode);
                node.Text.SetText("Distance: {0:2}", node.Distance );
                previousNode = position;
                Distance = Distance + node.Distance;
                count++;
            }
        }
    }
}
