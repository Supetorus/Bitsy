using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
	[SerializeField] Animator animator;
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
    }

    // Update is called once per frame
    void Update()
    {

	}

	void EnableTurret()
	{
		animator.SetBool("isActive", true);
	}

	void DisableTurret()
	{
		animator.SetBool("isActive", false);
	}
}
