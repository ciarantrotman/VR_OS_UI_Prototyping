using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VR_Prototyping.Scripts.Tools.Memo
{
    public class MemoTool : BaseTool
    {        
        [BoxGroup("Memo Tool Settings")] [Required] public GameObject memoPrefab;
        [BoxGroup("Memo Tool Settings")] public List<string> microphones;
        [BoxGroup("Memo Tool Settings")] [Range(.01f, .1f)] public float triggerDistance = .05f;
        private readonly List<AudioSource> audioSources = new List<AudioSource>();
        private MemoNode memoNode;
        private string microphone;
        private int index;
        private int clipLength;
        private float time;

        protected override void Initialise()
        {
            SetupMicrophone();
        }

        private void SetupMicrophone()
        {
            foreach (string device in Microphone.devices)
            {
                if (device != null)
                {
                    microphones.Add(device);
                }
            }

            microphone = microphones[0];
        }

        public void ChangeMicrophone(int i)
        {
            if (i <= microphones.Count - 1)
            {
                microphone = microphones[i];
            }
        }

        protected override void ToolUpdate()
        {
            
        }
        
        protected override void ToolStart()
        {
            NewMemo();
            time = Time.time;
            audioSources.Add(memoNode.AudioSource);
            memoNode.AudioSource.clip = Microphone.Start(microphone, true, 30, 44100);
        }

        protected override void ToolStay()
        {
            clipLength++;
            memoNode.transform.Position(dominant.transform);
        }

        protected override void ToolEnd()
        {
            Microphone.End(microphone);
            memoNode.ReleaseMemo(Time.time - time);
            memoNode = null;
        }

        protected override void ToolInactive()
        {
            
        }

        private void NewMemo()
        {
            var node = Instantiate(memoPrefab);
            memoNode = node.GetComponent<MemoNode>();
            memoNode.Initialise(Color.HSVToRGB(Random.Range(0f, 1f), 1, 1, true), index, this);
            index++;
        }

        public void PlayAudio(int index)
        {
            int i = 0;
            foreach (AudioSource audioSource in audioSources)
            {
                if (index == i)
                {
                    audioSource.Play();
                }
                else
                {
                    audioSource.Stop();
                }

                i++;
            }
        }
    }
}