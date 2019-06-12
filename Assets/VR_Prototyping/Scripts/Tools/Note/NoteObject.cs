using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools.Note
{
    public class NoteObject : MonoBehaviour
    {
        private NoteTool NoteTool { get; set; }
        private ControllerTransforms Controller { get; set; }
        [BoxGroup] [SerializeField] private TextMeshPro noteText;
        [BoxGroup] [SerializeField] private TextMeshPro noteTitle;

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
