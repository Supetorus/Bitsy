using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BuildingMakerToolset.Demo
{
    [RequireComponent(typeof(BoxCollider))]
    public class LadderVolume : TriggerZone
    {
        
        public string footstepAudioName = "LadderStep";

        protected override void OnEnter(PlayerMovement player)
        {
            player.currentTask = PlayerMovement.CurrentTask.ladder;
            player.SetFootstepAudio(ref footstepAudioName);
        }
        protected override void OnExit(PlayerMovement player)
        {
            player.currentTask = PlayerMovement.CurrentTask.none;
        }

    }
}
