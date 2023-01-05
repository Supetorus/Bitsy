using moveen.descs;
using UnityEngine;

namespace moveen.utils {
    public class OrderableTick : IOrderableTick {
        public int executionOrder;
        public bool _participateInUpdate;
        public bool _participateInPhysicsUpdate;
        public bool _isBroken;
        
        
        public int getOrder() {
            return executionOrder;
        }

        public virtual void tick(float dt) {
        }

        public virtual void fixedTick(float dt) {
        }

        public bool getParticipateInTick() {
            return _participateInUpdate;
        }

        public bool getParticipateInFixedTick() {
            return _participateInPhysicsUpdate;
        }

        public void setIsBroken(bool value) {
            _isBroken = value;
        }

        public bool getIsBroken() {
            return _isBroken;
        }
    }
}