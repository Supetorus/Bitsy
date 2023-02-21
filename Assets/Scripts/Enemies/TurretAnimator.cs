using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretAnimator : MonoBehaviour
{
	[SerializeField] public Animator animator;
	[SerializeField] Turret turret;
	private void OnEnable()
	{
		GlobalPlayerDetection.onThreeFourths += EnableTurret;
		GlobalPlayerDetection.onHalf += DisableTurret;
	}

	private void OnDisable()
	{
		GlobalPlayerDetection.onThreeFourths -= EnableTurret;
		GlobalPlayerDetection.onHalf -= DisableTurret;
	}

	// Start is called before the first frame update
	void Start()
    {
		DisableTurret();
    }

	void EnableTurret()
	{
		animator.SetBool("isActive", true);
		turret.enabled = true;
	}

	void DisableTurret()
	{
		animator.SetBool("isActive", false);
		turret.enabled = false;
	}
}
