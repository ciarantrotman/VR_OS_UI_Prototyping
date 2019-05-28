using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureText : MonoBehaviour
    {
        private MeasureTool MeasureTool { get; set; }
        private TextMeshPro HighLevelText  { get; set; }

        private void Start()
        {
            if (MeasureTool != null) return;
            MeasureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            MeasureTool.MeasureText = MeasureTool.MeasureText == null ? this : MeasureTool.MeasureText;
            HighLevelText = GetComponent<TextMeshPro>();
        }

        public void SetText(float total, string tapeName)
        {
            HighLevelText.SetText("Tape: <b>" + tapeName +
                                  "</b> - Length: <b>{0:2}</b> ", total);
        }
    }
}
