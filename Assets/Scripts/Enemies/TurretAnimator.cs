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
		DetectionLevel.onThreeFourths += Popup;
		DetectionLevel.onThreeFourths += EnableTurret;
		DetectionLevel.onEmpty += DisableTurret;
		DetectionLevel.onEmpty += Hide;
	}

	private void OnDisable()
	{
		DetectionLevel.onThreeFourths -= Popup;
		DetectionLevel.onThreeFourths -= EnableTurret;
		DetectionLevel.onEmpty -= DisableTurret;
		DetectionLevel.onEmpty -= Hide;
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
