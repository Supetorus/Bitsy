using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	//Goes from this angle
	[SerializeField] protected Vector3 angle1 = new Vector3(0.0F, 45.0F, 0.0F);
	//To this Angle
	[SerializeField] protected Vector3 angle2 = new Vector3(0.0F, -45.0F, 0.0F);
	//At this speed
	[SerializeField] protected float speed = 1.0F;
	public bool isIdle;

	public void Start()
	{
		isIdle = true;
	}

	protected void Update()
	{
		if(isIdle)
		{
			Quaternion from = Quaternion.Euler(angle1);
			Quaternion to = Quaternion.Euler(angle2);

			float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * speed));
			transform.localRotation = Quaternion.Lerp(from, to, lerp);
		}
	}
}
