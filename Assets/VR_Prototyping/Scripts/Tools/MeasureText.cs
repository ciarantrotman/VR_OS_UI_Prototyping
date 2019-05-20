using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureText : MonoBehaviour
    {
        public MeasureTool MeasureTool { get; set; }
        public TextMeshPro HighLevelText  { get; set; }

        private void Start()
        {
            if (MeasureTool != null) return;
            
            MeasureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            MeasureTool.MeasureText = this;
            HighLevelText = GetComponent<TextMeshPro>();
        }

        public void SetText(float total, string tapeName)
        {
            HighLevelText.SetText("Total: <b>{1:2}</b> " +
                         "Tape: " + tapeName, total);
        }
    }
}
