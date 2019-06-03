using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools
{
    public class NoteObject : MonoBehaviour
    {
        public NoteTool NoteTool { get; set; }
        public ControllerTransforms Controller { get; set; }
        [BoxGroup] public TextMeshPro NoteText;
        [BoxGroup] public TextMeshPro NoteTitle;

        public void Initialise(NoteTool noteTool)
        {
            NoteTool = noteTool;
        }

        private void FixedUpdate()
        {
            Set.LookAtVertical(transform, Controller.CameraTransform());
        }

        public void SetNote(string text, string title, Vector3 position)
        {
            transform.position = position;
            NoteText.SetText(text);
            NoteTitle.SetText(title);
        }
    }
}
