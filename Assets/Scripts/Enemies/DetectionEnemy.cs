using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public abstract class DetectionEnemy : MonoBehaviour
{
	private bool canSeePlayer;
	public bool CanSeePlayer
	{
		get => canSeePlayer;
		protected set => canSeePlayer = value;
	}

	[SerializeField, Tooltip("The layers that should interrupt raycasts searching for the player, including the player")]
	protected LayerMask layerMask;

	public abstract bool CheckSightlines();
	public virtual void DartRespond() { }
	public virtual void EMPRespond(float stunDuration, GameObject stunEffect) { }
}
