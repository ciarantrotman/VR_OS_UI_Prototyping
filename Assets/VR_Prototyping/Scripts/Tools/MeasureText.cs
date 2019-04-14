using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class MeasureText : MonoBehaviour
    {
        private MeasureTool measureTool;
        private TextMeshPro text;

        private void Start()
        {
            measureTool = transform.parent.transform.GetComponentInParent<MeasureTool>();
            measureTool.MeasureText = this;
            text = transform.GetComponent<TextMeshPro>();
        }

        private void Update()
        {
            transform.LookAwayFrom(measureTool.controller.CameraTransform(), Vector3.up);
        }

        public void SetText(float distance)
        {
            text.SetText("Tape: <b>{0:2}</b>", distance);
        }
    }
}
