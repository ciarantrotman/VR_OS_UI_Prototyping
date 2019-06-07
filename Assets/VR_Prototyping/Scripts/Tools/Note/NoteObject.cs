using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace VR_Prototyping.Scripts.Tools
{
    public class NoteObject : MonoBehaviour
    {
        private NoteTool NoteTool { get; set; }
        private ControllerTransforms Controller { get; set; }
        [BoxGroup] public TextMeshPro noteText;
        [BoxGroup] public TextMeshPro noteTitle;

        public void Initialise(NoteTool noteTool, ControllerTransforms controllerTransforms)
        {
            Controller = controllerTransforms;
            NoteTool = noteTool;
        }

        private void FixedUpdate()
        {
            transform.LookAtVertical(Controller.CameraTransform());
        }

        public void SetNote(string text, string title, Vector3 position)
        {
            transform.position = position;
            noteText.SetText(text);
            noteTitle.SetText(title);
        }
    }
}
