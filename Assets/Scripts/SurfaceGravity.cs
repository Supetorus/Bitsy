using moveen.descs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceGravity : MonoBehaviour
{
	public Vector3 upDir;
	RaycastHit groundHit;
	public float gravScale = -9.8f;
	public MoveenStepper5 moveen;
	public Rigidbody bodyRigid;

	void Start()
	{
		if (moveen != null)
		{
			bodyRigid = moveen.body == null ? moveen.gameObject.GetComponent<Rigidbody>() : moveen.body.GetComponent<Rigidbody>();
		}
	}

	void FixedUpdate()
	{
		if (bodyRigid == null) return;

		//upDir = Vector3.up;
		//get normal of ground
		if (Physics.Raycast(bodyRigid.position, -bodyRigid.transform.up, out groundHit, 1))
		{
			//upDir = groundHit.normal;
			bodyRigid.AddRelativeForce(Vector3.up * gravScale * Time.fixedDeltaTime, ForceMode.Force);
		}
		else
		{
			bodyRigid.AddForce(Vector3.up * gravScale * Time.fixedDeltaTime, ForceMode.Force);
		}

	}
}
