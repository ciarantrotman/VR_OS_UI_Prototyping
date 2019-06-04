using Sirenix.OdinInspector;
using UnityEngine;
using VR_Prototyping.Scripts.Keyboard;

namespace VR_Prototyping.Scripts.Tools
{
    public class NoteTool : BaseTool
    {
        [BoxGroup("Note Tool Settings")] [Required] public GameObject notePrefab;
        
        public KeyboardManager KeyboardManager;

        private GameObject _note;
        private NoteObject _noteObject;

        protected override void Initialise()
        {
            KeyboardManager = toolMenu.keyboardManager;
            KeyboardManager.Enter.AddListener(FinishNote);
        }
        
        private void OnEnable()
        {
            KeyboardManager.ToggleKeyboard(true);
        }

        private void NewNote()
        {
            _note = Instantiate(notePrefab);
            _noteObject = _note.GetComponent<NoteObject>();
            _noteObject.Initialise(this);
        }
        
        public void FinishNote()
        {
            _noteObject.SetNote(KeyboardManager.keyboardTarget.CheckText(), Time.time.ToString(), dominant.transform.position);
            KeyboardManager.keyboardTarget.ClearText();
            _note = null;
            _noteObject = null;
        }
    }
}
