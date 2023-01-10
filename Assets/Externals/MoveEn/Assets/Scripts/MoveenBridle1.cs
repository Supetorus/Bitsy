using moveen.descs;
using moveen.utils;
using UnityEngine;

namespace moveen.example {
    
    ///
    /// This component sets pos/rot of its GameObject as a target for the MoveenStepper5. Not directly, but with a little logic about speed limits.
    /// This class is supposed to be a simplified example of the target for the Moveen animation but also can be used in some scenarios. For example, if you want your Mveen animated robot to just go by some predefined route.
    /// 
    /// <para>
    /// You can implement your own "bridle". Also look at MoveControl1 and MoveControl2 for more complex examples, which could be used as it is in FPS or top-down shooter.
    /// </para>
    public class MoveenBridle1 : MonoBehaviour {
        [BindWarning]
        [Tooltip("Driven animation")]
        public MoveenStepper5 moveenStepper5;
        [Tooltip("Coefficient to convert distance to speed")]
        public float distanceToSpeed = 1;
        [Tooltip("Maximum speed dictated by this bridle")]
        public float maxSpeed = 1;

        public bool dbgRotation;

        public void FixedUpdate() {
            if (moveenStepper5 == null) return;
            moveenStepper5.targetPos = transform.position
                //.mix(oldTransform, 0.98f)//mix with previous position to avoid flicks
                .sub(moveenStepper5.engine.imCenter)
                .mul(distanceToSpeed)
                .limit(maxSpeed)
                .add(moveenStepper5.engine.imCenter)
                .withSetY(transform.position.y)              
                ;

            if (dbgRotation) transform.rotation = transform.rotation * Quaternion.AngleAxis(1, Vector3.up);
            moveenStepper5.targetRot = transform.rotation;
            
        }

        public void OnDrawGizmos() {
            Vector3 pos = Camera.current.transform.worldToLocalMatrix.MultiplyPoint(transform.position);
            Gizmos.matrix = Matrix4x4.TRS(Camera.current.transform.position, Camera.current.transform.rotation, Vector3.one);

            float s1 = 0.1f;
            float s2 = 0.3f;
            float s3 = 0.1f;
            Gizmos.DrawLine(pos + new Vector3(-s1, 0, 0), pos + new Vector3(s1, 0, 0));
            Gizmos.DrawLine(pos + new Vector3(0, -s1, 0), pos + new Vector3(0, s1, 0));

            Gizmos.DrawLine(pos + new Vector3(-s2, -s2, 0), pos + new Vector3(-s2+s3, -s2, 0));
            Gizmos.DrawLine(pos + new Vector3(-s2, -s2, 0), pos + new Vector3(-s2, -s2+s3, 0));
            Gizmos.DrawLine(pos + new Vector3(s2, -s2, 0), pos + new Vector3(s2-s3, -s2, 0));
            Gizmos.DrawLine(pos + new Vector3(s2, -s2, 0), pos + new Vector3(s2, -s2+s3, 0));

            Gizmos.DrawLine(pos + new Vector3(-s2, s2, 0), pos + new Vector3(-s2+s3, s2, 0));
            Gizmos.DrawLine(pos + new Vector3(-s2, s2, 0), pos + new Vector3(-s2, s2-s3, 0));
            Gizmos.DrawLine(pos + new Vector3( s2, s2, 0), pos + new Vector3( s2-s3, s2, 0));
            Gizmos.DrawLine(pos + new Vector3( s2, s2, 0), pos + new Vector3( s2, s2-s3, 0));

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}