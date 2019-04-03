using UnityEngine;

namespace VR_Prototyping.Scripts
{
    public class DrivenValue : MonoBehaviour
    {
        public Renderer r;

        public Color a;
        public Color b;

        public DirectDial dial;
        public DirectSlider slider;

        private void Update()
        {
            if (dial != null)
            {
                r.material.color = Color.Lerp(a, b, dial.dialValue);
            }
            else if (slider != null)
            {
                r.material.color = Color.Lerp(a, b, slider.sliderValue);
            }
        }
    }
}
