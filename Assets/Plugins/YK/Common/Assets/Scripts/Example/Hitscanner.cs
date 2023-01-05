using System;
using System.Collections.Generic;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.example {
    /// <summary>
    /// Determines target on the sight. Gun barrel is considered looking at (1, 0, 0) (i.e. right)
    /// </summary>
    public class Hitscanner : OrderedMonoBehaviour {
        [NonSerialized] public bool hitFound;
        [NonSerialized] public RaycastHit raycastHit;
        [NonSerialized] public Vector3 origin;
        public Vector3 direction = Vector3.right;

        [FormerlySerializedAs("target")] //24.11.17
        [Tooltip(
            "GameObject which is represents aim. Its location will be set to be at aimed GameObject. You can put some effects into the \"aim\" to play them when the gun is fired.")]
        public Transform aim;

        [Tooltip("The layer which will be used to detect surface. For example, create a layer for objects you want to walk on and name it as \"ground\" or \"terrain\"")]
        public LayerMask layer = ~0;

        [Tooltip("Don't consider back surface")] public bool ignoreBackSurface = true;
        [Tooltip("How far detector should look")] public float maxDetectDistance = 300;
        [Tooltip("Maximum number of crosses detected. Consider simplification of your scene instead of increasing of this number")] public int bufferSize = 10;

        [NonSerialized] private RaycastHit[] buffer;
        [NonSerialized] public bool resetMaxFoundHits;
        [NonSerialized] public int maxFoundHits;

        public Transform hierarchyToIgnore;
        [NonSerialized] public List<Collider> collidersToIgnore = new List<Collider>();

        public Hitscanner() {
            executionOrder = 100;
        }

        public override void OnEnable() {
            base.OnEnable();
            if (hierarchyToIgnore != null) {
                addCollisionHierarchyToIgnore(hierarchyToIgnore);
            }
        }

        public virtual Ray getRay() {
            return new Ray(transform.position, transform.rotation.rotate(direction));
        }

        public override void tick(float dt) {
            Ray detectionRay = getRay();
            if (resetMaxFoundHits) {
                resetMaxFoundHits = false;
                maxFoundHits = 0;
            }
            if (buffer == null || buffer.Length != bufferSize) buffer = new RaycastHit[bufferSize];
            int foundHits = Physics.RaycastNonAlloc(detectionRay.origin, detectionRay.direction, buffer, maxDetectDistance, layer.value, QueryTriggerInteraction.Ignore);
            maxFoundHits = MyMath.max(foundHits, maxFoundHits);

            RaycastHit closest = new RaycastHit();
            float minDist = float.MaxValue;
            bool found = false;
            if (foundHits > 0) {
                for (int i = 0; i < foundHits; i++) {
                    RaycastHit curHit = buffer[i];
                    if (collidersToIgnore.Contains(curHit.collider)) continue;
                    if (ignoreBackSurface && curHit.normal.scalarProduct(detectionRay.direction) > 0) continue;
                    Vector3 relative = curHit.point.sub(detectionRay.origin);
                    float curDist = relative.scalarProduct(detectionRay.direction);
                    curDist = Math.Abs(curDist);
                    if (curDist < minDist) {
                        minDist = curDist;
                        closest = curHit;
                        found = true;
                    }
                }
            }

            hitFound = found;

            Vector3 hitPos;
            if (found) {
                hitPos = closest.point;
                raycastHit = closest;
                aim.rotation = Quaternion.FromToRotation(Vector3.up, closest.normal);
            } else {
                hitPos = transform.TransformPoint(direction * 100);
            }
            aim.position = hitPos;
            this.origin = detectionRay.origin;

        }

        public void addCollisionHierarchyToIgnore(Transform go) {
            foreach (var c in go.GetComponentsInChildren<Collider>()) {
                collidersToIgnore.Add(c);
            }

        }
        
    }

}