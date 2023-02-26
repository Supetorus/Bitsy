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
		GlobalPlayerDetection.onThreeFourths += Popup;
		GlobalPlayerDetection.onHalf += Hide;
	}

	private void OnDisable()
	{
		GlobalPlayerDetection.onThreeFourths -= Popup;
		GlobalPlayerDetection.onHalf -= Hide;
	}

	// Start is called before the first frame update
	void Start()
    {
		Hide();
		DisableTurret();
    }

	void Popup()
	{
		animator.SetBool("isActive", true);
	}

	void Hide()
	{
		animator.SetBool("isActive", false);
	}

	public void EnableTurret()
	{
		turret.enabled = true;
	}

	public void DisableTurret()
	{
		turret.enabled = false;
	}
}
