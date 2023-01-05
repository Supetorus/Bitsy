using System;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.example {
    /// <summary>
    /// Starts all ParticleSystem and Startable in its GameObject or in target GameObject. In a sole, or in all its hierarchy.
    /// <para/>
    /// Very handy to start different effects by event without scripting. For example - when step or fire is made.
    /// </summary>
    public class EverythingStarter : MonoBehaviour, Startable {
        [Tooltip("If false - looks for startable in target GameObject only. If enabled - in its hierarchy too")]
        public bool lookInChildren;
        [FormerlySerializedAs("effects")]//24.11.17
        [Tooltip("Target GameObject. If null - considered this GameObject")]
        public Transform target;

        [NonSerialized] private ParticleSystem[] particleSystems;
        [NonSerialized] private Startable[] startables;

        private void Awake() {
            Transform lookIn = target == null ? transform : target;
            if (lookInChildren) {
                particleSystems = lookIn.GetComponentsInChildren<ParticleSystem>();
                startables = lookIn.GetComponentsInChildren<Startable>();
            } else {
                particleSystems = lookIn.GetComponents<ParticleSystem>();
                startables = lookIn.GetComponents<Startable>();
            }
        }

        public void start() {
            for (int i = 0; i < particleSystems.Length; i++) {
                particleSystems[i].Play();
            }
            for (int i = 0; i < startables.Length; i++) {
                Startable startable = startables[i];
                if (startable != this && ((MonoBehaviour)startable).isActiveAndEnabled) startable.start();
            }
        }
    }
}