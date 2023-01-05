using System;
using System.Collections.Generic;
using moveen.core;
using moveen.utils;
using UnityEngine;

namespace moveen.example {
    /// <summary>
    /// Listens to the player's action (Fire1, Fire2, etc) and starts Startables.
    /// Startables can be all of the sorts: sound starters, ParticleSystems (even though they are not Startable), recoils, hit scanners, etc.
    /// </summary>
    public class FireControl2 : MonoBehaviour {
        [Tooltip("FocusGrabber defines if this component will react player's input or not. If not defined - component grabs input")]
        public FocusGrabber focusGrabber;
        [Tooltip("If true - component will start its events only once per mouse click. If false - component can shoot continuously")]
        public bool fireOnClick;
        [Tooltip("Input action name")]
        public string actionCode = "Fire1";
        [Tooltip("Component will not shoot more often than this amount of seconds")]
        public float cooldown = 1;
        [ReadOnly] public float curCooldown;
        [ReadOnly] public bool justFired;

        [NonSerialized] private ParticleSystem[] particleSystems;
        [NonSerialized] private Startable[] _startables;
        
        public Transform effects;

        private void OnEnable() {
            Transform lookIn = effects == null ? transform : effects;
            particleSystems = lookIn.GetComponents<ParticleSystem>();
            _startables = lookIn.GetComponents<Startable>();
            if (focusGrabber == null) focusGrabber = GetComponent<FocusGrabber>();
        }

        private void beginFire() {
            if (curCooldown < cooldown) return;
            curCooldown = 0;
            for (int i = 0; i < particleSystems.Length; i++) {
                particleSystems[i].Play();
                justFired = true;
            }
            for (int i = 0; i < _startables.Length; i++) {
                Startable startable = _startables[i];
                if (((MonoBehaviour)startable).enabled) startable.start();
            }

        }

        private void Update() {
            justFired = false;
            curCooldown += Time.deltaTime;
            bool grabInput = focusGrabber == null || focusGrabber.grab;
            if (!grabInput) return;
            
            if (fireOnClick) {
                if (Input.GetButtonDown(actionCode)) {
                    beginFire();
                }
            } else {
                if (Input.GetButton(actionCode)) {
                    beginFire();
                }
            }
        }
    }
}