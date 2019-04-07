using UnityEngine;
using UnityEngine.Formats.Alembic.Timeline;

namespace VR_Prototyping.Scripts
{
    public class DrivenValue : MonoBehaviour
    {
        public Renderer r;
        public AlembicTrack alembicTrack;

        public enum Function
        {
            Colour,
            Alembic
        }

        public Function drivenValue;

        public Color a;
        public Color b;

        public DirectDial dial;
        public DirectSlider slider;

        private void Update()
        {
            switch (drivenValue)
            {
                case Function.Colour:
                    if (dial != null)
                    {
                        r.material.color = Color.Lerp(a, b, dial.dialValue);
                    }
                    else if (slider != null)
                    {
                        r.material.color = Color.Lerp(a, b, slider.sliderValue);
                    }
                    break;
                case Function.Alembic:
                    if (dial != null)
                    {
                        //alembicTrack.
                    }
                    else if (slider != null)
                    {
                        //r.material.color = Color.Lerp(a, b, slider.sliderValue);
                    }
                    break;
            }
        }
    }
}
