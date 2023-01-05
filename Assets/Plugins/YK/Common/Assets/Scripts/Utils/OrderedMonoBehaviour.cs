using System;
using System.Collections;
using moveen.descs;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.utils {
    /// <summary>
    /// OrderedMonoBehaviour serves two purposes:
    /// <para/>
    /// 1. it is a parent for any MonoBehaviour which needs to use the ordered tick feature (implemented in OrderedTick).
    /// <para/>
    /// 2. it delivers Unity's events for the OrderedTick.
    /// </summary>
    public abstract class OrderedMonoBehaviour : MonoBehaviour, IOrderableTick {
        [Tooltip("Execution order in OrderedTick system")]
        public int executionOrder;
        [Tooltip("Should tick() be called")]
        [FormerlySerializedAs("participateInUpdate")]//renamed for 0.83 
        public bool participateInTick = true;
        [Tooltip("Should fixedTick() be called")]
        [FormerlySerializedAs("participateInFixedUpdate")]//renamed for 0.83 
        public bool participateInFixedTick;
        [Tooltip("Is some exceptions were thrown while executing tick/fixedTick (tick/fixedTick is not called if set to true)")]
        public bool isBroken;

        public int getOrder() {
            return executionOrder;
        }

        /// <summary>
        /// Called once per Unity's Update/LateUpdate cycle in the order defined by executionOrder.
        /// <para/>
        /// Note, that calls to tick(dt) and fixedTick(dt) are not correlated (because Unity's Update/FixedUpdate aren't).
        /// </summary>
        public abstract void tick(float dt);

        /// <summary>
        /// Called once per Unity's FixedUpdate cycle in the order defined by executionOrder.
        /// Note, that calls to tick(dt) and fixedTick(dt) are not correlated (because Unity's Update/FixedUpdate aren't).
        /// </summary>
        public virtual void fixedTick(float dt) {
        }

        public bool getParticipateInTick() {
            return participateInTick;
        }

        public bool getParticipateInFixedTick() {
            return participateInFixedTick;
        }

        public void setIsBroken(bool value) {
            isBroken = value;
        }

        public bool getIsBroken() {
            return isBroken;
        }

        public virtual void Update() {
            OrderedTick.onUnityUpdate();
        }

        public void LateUpdate() {
            OrderedTick.onUnityLateUpdate();
        }

        //In Unity's cycle, there are Update and LateUpdate events while for FixedUpdate there are no 'Late' FixedUpdate.
        //These coroutineStarted, coroutineOwner, startAfterPhysics - are used to simulate 'Late' FixedUpdate.
        
        private static bool coroutineStarted;
        [NonSerialized]
        private bool coroutineOwner;

        public void FixedUpdate() {
            //commented because we want fixedTick even when there are no physics participants
            //  (and call it from MonoBehaviour is the only way of getting the event)
            //if (participateInFixedUpdate) {
                if (!coroutineStarted) StartCoroutine(startAfterPhysics());
                OrderedTick.onUnityFixedUpdate(Time.deltaTime);
            //}
        }

        private IEnumerator startAfterPhysics() {
            coroutineStarted = true;
            coroutineOwner = true;
            while (true) {
                yield return new WaitForFixedUpdate();
                OrderedTick.onUnityLateFixedUpdate();
            }
        }

        public virtual void OnEnable() {
            OrderedTick.addComponent(this);
        }
        
        public virtual void OnDisable() {
            removeFromOrderedTick();
        }

        public void OnDestroy() {
            removeFromOrderedTick();
        }

        private void removeFromOrderedTick()
        {
            OrderedTick.deleteComponent(this);
            // OrderedTick.deleteComponentFast(this);
            if (coroutineOwner)
            {
                //because its coroutines will stop working if GameObject is disabled (though they still work if only script is disabled, but we don't distinguish) 
                coroutineStarted = false;
                coroutineOwner = false;
            }
        }

        [NonSerialized] private float lastExecutionOrder;
        public virtual void OnValidate() {
            if (lastExecutionOrder != executionOrder) {
                lastExecutionOrder = executionOrder;
                OrderedTick.setUnsorted();
            }

        }
    }
}