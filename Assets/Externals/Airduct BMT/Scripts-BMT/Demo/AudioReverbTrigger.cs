using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BuildingMakerToolset.Demo
{
    public class AudioReverbTrigger : TriggerZone
    {
        public AudioReverbPreset reverbPreset;

        protected override void OnEnter(PlayerMovement player)
        {
            AudioReverbFilter rf = player.gameObject.GetComponentInChildren<AudioReverbFilter>();
            if (rf == null)
                return;
            rf.reverbPreset = reverbPreset;
        }
        protected override void OnExit(PlayerMovement player)
        {
            AudioReverbFilter rf = player.gameObject.GetComponentInChildren<AudioReverbFilter>();
            if (rf == null)
                return;
            rf.reverbPreset = AudioReverbPreset.Off;
        }
    }
}
