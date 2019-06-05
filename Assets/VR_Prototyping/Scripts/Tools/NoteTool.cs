using System;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.Keyboard;

namespace VR_Prototyping.Scripts.Tools
{
    public class NoteTool : BaseTool
    {
        [BoxGroup("Note Tool Settings")] [Required]
        public GameObject notePrefab;

        private float _index;
        private KeyboardManager KeyboardManager { get; set; }

        private GameObject _note;
        private NoteObject _noteObject;

        protected override void OnStart()
        {
            KeyboardManager = toolMenu.keyboardManager;
        }

        protected override void ToolActivate()
        {
            KeyboardManager.ToggleKeyboard(true);
            KeyboardManager.enter.AddListener(FinishNote);
        }

        protected override void ToolDeactivate()
        {
            if (KeyboardManager == null) return;
            KeyboardManager.ToggleKeyboard(false);
            KeyboardManager.enter.RemoveListener(FinishNote);
        }

        private void NewNote()
        {
            _note = Instantiate(notePrefab);
            _noteObject = _note.GetComponent<NoteObject>();
            _noteObject.Initialise(this, controller);
        }

        private void FinishNote()
        {
            NewNote();
            var position = controller.CameraTransform().position + controller.CameraForwardVector();
            _noteObject.SetNote(KeyboardManager.KeyboardTarget.CheckText(),
                DateTime.Now.ToString(CultureInfo.InvariantCulture), position);
            KeyboardManager.KeyboardTarget.ClearText();
            _note.name = "Note_" + _index;
            _note = null;
            _noteObject = null;
            _index++;
        }
    }
}