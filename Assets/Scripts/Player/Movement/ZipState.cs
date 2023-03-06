using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZipState : MovementState
{
    public const float ZIP_SPEED = 20f;
    [HideInInspector]public RaycastHit attachedObject;
    [HideInInspector]public Quaternion originalRot;
    [HideInInspector]public Quaternion targetRot;

    public override void EnterState()
    {
		Player.Camera.GetComponent<ThirdPersonCameraController>().canZoom = false;
		originalRot = transform.rotation;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;

        Vector3 direction = (attachedObject.point - transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(direction * ZIP_SPEED, ForceMode.VelocityChange);
    }

    public override void UpdateState()
    {
        // Todo this should be a slerp from the original position to facing upright,
        // or upright according to where they will land.

        //Total Dist
        //attachedObject.distance

        //RemainingDist
        float remainingDist = (attachedObject.point - transform.position).magnitude;
        if (remainingDist < 1) rigidbody.useGravity = true;

        targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, attachedObject.normal), attachedObject.normal);
        transform.rotation = Quaternion.Slerp(targetRot, originalRot, remainingDist / attachedObject.distance);
        List<RaycastHit> hits = SphereRaycaster.SphereRaycast(transform.position, sd.lesserAttachmentDistance, sd.walkableLayers);
        Vector3? point = SphereRaycaster.GetClosestPoint(hits, transform.position);
        if (point != null)
        {
			c.CurrentMovementState = c.clingState;
        }
    }
}
