using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.Keyboard;

namespace VR_Prototyping.Scripts.Tools
{
    public class NoteTool : BaseTool
    {
        [BoxGroup("Note Tool Settings")] [Required] public GameObject notePrefab;

        public KeyboardManager KeyboardManager { get; set; }

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
            KeyboardManager.ToggleKeyboard(false);
            KeyboardManager.enter.RemoveListener(FinishNote);
        }

        private void NewNote()
        {
            _note = Instantiate(notePrefab);
            _noteObject = _note.GetComponent<NoteObject>();
            _noteObject.Initialise(this);
        }

        private void FinishNote()
        {
            NewNote();
            _noteObject.SetNote(KeyboardManager.keyboardTarget.CheckText(), "4:20, lmao", dominant.transform.position);
            KeyboardManager.keyboardTarget.ClearText();
            _note = null;
            _noteObject = null;
        }
    }
}
