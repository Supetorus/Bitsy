using System;
using moveen.descs;
using moveen.utils;
using UnityEngine;

namespace moveen.example {
    /// <summary>
    /// This component executes Startables when the leg steps on the ground. It can be used to generate different effects on this event (dust, sound, etc)
    /// </summary>
    public class Step2Event : MonoBehaviour {
        [Tooltip("Target Step2 component. If null, will be looked for on this GameObject")]
        public MoveenStep2 step2;
        [ReadOnly] public bool wasAtGround; 
        [NonSerialized] private Startable[] _startables;
        [NonSerialized] private ParticleSystem[] particleSystems;
        [Tooltip("GameObject that conatins Startables to execute when the event is happening. If not defined - Starables will be collected from the current GameObject")]
        public Transform effects;

        
        private void OnEnable() {
            if (step2 == null) step2 = transform.GetComponent<MoveenStep2>();
            if (step2 == null) return;
            wasAtGround = step2.step.dockedState;
            Transform lookIn = effects == null ? transform : effects;
            particleSystems = lookIn.GetComponentsInChildren<ParticleSystem>();
            _startables = lookIn.GetComponentsInChildren<Startable>();
        }
        
        
        
        private void FixedUpdate() {
            if (step2 == null) return;
            if (!wasAtGround && step2.step.dockedState) {
                for (int i = 0; i < particleSystems.Length; i++) {
                    particleSystems[i].Play();
                }
                for (int i = 0; i < _startables.Length; i++) {
                    Startable startable = _startables[i];
                    if (((MonoBehaviour)startable).enabled) startable.start();
                }
            }
            wasAtGround = step2.step.dockedState;
        }


    }
}