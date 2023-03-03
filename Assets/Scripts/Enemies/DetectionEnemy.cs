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

	protected bool ConeDetection(float maxAngle, Transform player, LayerMask layerMask, Vector3? detectionOrigin = null)
	{
		Vector3 origin = detectionOrigin != null ? detectionOrigin.Value : transform.position;
		Vector3 directionToPlayer = player.position - origin;
		float angle = Vector3.Angle(transform.forward, directionToPlayer);
		if (angle < maxAngle &&
			Physics.Raycast(origin, directionToPlayer, out RaycastHit hit, float.PositiveInfinity, layerMask) &&
			hit.collider.CompareTag("Player") &&
			player.GetComponent<AbilityController>().isVisible)
		{
			return true;
		}
		else return false;
	}

	private float lineRotation = 0;
	protected void DrawCone(int lineCount, List<LineRenderer> lines, LineRenderer lineRenderer, float beamRotationSpeed, float maxAngle, Vector3? beamOrigin = null)
	{
		if (lineCount == 0) return;
		while (lines.Count < lineCount) lines.Add(Instantiate(lineRenderer, transform));
		while (lines.Count > lineCount) { Destroy(lines[lines.Count - 1].gameObject); lines.RemoveAt(lines.Count - 1); }

		Vector3 origin = beamOrigin == null ? transform.position : beamOrigin.Value;
		lineRotation += Time.deltaTime * beamRotationSpeed;
		Quaternion toEdge = Quaternion.AngleAxis(maxAngle, transform.up);
		Quaternion aroundCircumference = Quaternion.AngleAxis(lineRotation, transform.forward);
		Quaternion rotationIncrement = Quaternion.AngleAxis(360f / lineCount, transform.forward);
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i].SetPosition(0, origin);
			Vector3 direction = aroundCircumference * toEdge * transform.forward;
			Physics.Raycast(transform.position, direction, out RaycastHit hit);
			if (hit.collider != null) lines[i].SetPosition(1, hit.point);
			else lines[i].SetPosition(1, origin + direction * 200);//200 is just a big number so it's not super noticable that it ends.
			aroundCircumference *= rotationIncrement;
		}
	}
}
