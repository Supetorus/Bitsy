using System;
using moveen.utils;
using UnityEngine;

namespace moveen.core {
    /// <summary>
    /// A positional motor. Calculates an acceleration needed to move or rotate to a given position. A style of approaching a position/rotation is defined by a bunch of parameters. You can define a smooth or sharp, robotic or natural movement.
    /// <para/>
    /// Does NOT contain actual position or rotation! It only provides the functions (<c>getAccel getTorque</c>) to calculate accel from inputs.
    /// </summary>
    [Serializable]
    public class MotorBean {
        [Tooltip("Coefficient: distance to wanted speed. (It is sometimes called \"Spring\")")]
        public float distanceToSpeed = 5;
        [Tooltip("Maximum wanted speed. Motor will try to slow down if speed is too high")]
        public float maxSpeed = 100;
        [Tooltip("Coefficient: wanted speed to acceleration.")]
        public float speedDifToAccel = 20;
        [Tooltip("Maximum applicable acceleration. Not the force. Corresponds to the power of the motor, but not depends on the mass of the connected body. So you can scale mass without fixing the motor, but you do want to scale this parameter if you want the motor to seem weaker or stronger")]
        public float maxAccel = 100;

        
        //maxAccelMultiplier - usually means count of legs participating in work
        /// <summary>
        /// Calculate an acceleration needed to achieve targetPos given currentPos and currentSpeed with this motor's parameters.
        /// </summary>
        public float getAccel(float targetPos, float currentPos, float currentSpeed, float maxAccelMultiplier) {
            return MyMath.clamp((MyMath.clamp((targetPos - currentPos) * distanceToSpeed, maxSpeed) - currentSpeed) * speedDifToAccel, maxAccel * maxAccelMultiplier);
        }

        //maxAccelMultiplier - usually means count of legs participating in work
        /// <summary>
        /// Calculate an acceleration needed to achieve targetPos given currentPos, currentSpeed, and externalAccel with this motor's parameters.
        /// </summary>
        public float getAccel(float targetPos, float currentPos, float currentSpeed, float externalAccel, float maxAccelMultiplier) {
            return MyMath.clamp((MyMath.clamp((targetPos - currentPos) * distanceToSpeed, maxSpeed) - currentSpeed) * speedDifToAccel + externalAccel, maxAccel * maxAccelMultiplier);
        }

        //maxAccelMultiplier - usually means count of legs participating in work
        /// <summary>
        /// Calculate an acceleration needed to achieve targetPos given currentPos, currentSpeed with this motor's parameters.
        /// </summary>
        public Vector3 getAccel(Vector3 targetPos, Vector3 currentPos, Vector3 currentSpeed, float maxAccelMultiplier = 1) {
            return targetPos
                .sub(currentPos)
                .mul(distanceToSpeed)
                .limit(maxSpeed)
                .sub(currentSpeed)
                .mul(speedDifToAccel)
                .limit(maxAccel * maxAccelMultiplier);
        }

        public void copyFrom(MotorBean other) {
            distanceToSpeed = other.distanceToSpeed;
            maxSpeed = other.maxSpeed;
            speedDifToAccel = other.speedDifToAccel;
            maxAccel = other.maxAccel;
        }

        //maxAccelMultiplier - usually means count of legs participating in work
        /// <summary>
        /// Calculate a torque needed to achieve wantedRot given currentRot, and currentAngularSpeed with this motor's parameters.
        /// </summary>
        public Vector3 getTorque(Quaternion currentRot, Quaternion wantedRot, Vector3 currentAngularSpeed, float maxAccelMultiplier = 1) {
            Quaternion rotSub = wantedRot.mul(currentRot.conjug());
            if (rotSub.w < 0) rotSub = rotSub.mul(-1); //big jump without it eventually
            Vector3 wantedAngularVelocity = rotSub.imaginary()
                .mul(distanceToSpeed)         
                .limit(maxSpeed)
                .sub(currentAngularSpeed)
                .mul(speedDifToAccel)
                .limit(maxAccel * maxAccelMultiplier);
            return wantedAngularVelocity;
        }
    }
    
}
