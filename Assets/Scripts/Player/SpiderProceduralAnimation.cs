using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderProceduralAnimation : MonoBehaviour
{
	[SerializeField, Tooltip("The ideal position for each leg.")]
	private Transform[] legTargets;
	[SerializeField, Tooltip("How far away the leg target needs to get from it's current position before it steps.")]
	private float stepSize = 0.15f;
	[SerializeField, Tooltip("Affects how smooth the leg positions are as well as the general body movement.")]
	private int smoothness = 8;
	[SerializeField, Tooltip("Determines the max height of a foot during a step.")]
	private float stepHeight = 0.15f;
	[SerializeField, Tooltip("Whether the body orientation is decided by this script, or given from the transform")]
	private bool bodyOrientation = true;
	[SerializeField, Tooltip("What layers the spider can walk on.")]
	private LayerMask walkableLayers;
	[SerializeField, Tooltip("For some reason the velocity is multiplied a bunch of times by this.")]
	private float velocityMultiplier = 15f;

	/// <summary>
	/// Determines the radius of the sphere shot from the sky to the ground to determine where the surface is that the player can walk on.
	/// </summary>
	private float sphereCastRadius = 0.125f;
	/// <summary>
	/// How far above the spider the raycast will start from which decends to find the ground. Half the length of the actual raycast.
	/// </summary>
	private float raycastRange = 1.5f;
	/// <summary>
	/// The position of each leg at start relative to the spider.
	/// </summary>
	private Vector3[] defaultLegPositions;
	/// <summary>
	/// The last position of each leg. If a leg is in motion this is its last position before moving.
	/// </summary>
	private Vector3[] lastLegPositions;
	/// <summary>
	/// The up vector last update, used to smooth the rotation if bodyOrientation is true.
	/// </summary>
	private Vector3 lastBodyUp;
	/// <summary>
	/// Keeps track of which legs are moving at any given time.
	/// </summary>
	private bool[] legMoving;
	/// <summary>
	/// The number of legs according to the legTargets array.
	/// </summary>
	private int numberOfLegs;
	private Vector3 lastVelocity;
	private Vector3 lastBodyPos;

	/// <summary>
	/// Returns an array of two elements. The first of which is a position, and the second is a normal.
	/// </summary>
	/// <param name="point">The target of the leg/foot</param>
	/// <param name="halfRange"></param>
	/// <param name="up"></param>
	/// <returns></returns>
	Vector3[] MatchToSurfaceFromAbove(Vector3 point, float halfRange, Vector3 up)
	{
		Vector3[] result = new Vector3[2];
		result[1] = Vector3.zero;
		Ray ray = new Ray(point + halfRange * up / 2f, -up);
Debug.DrawRay(ray.origin, ray.direction, Color.magenta);
		bool doBackface = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		if (Physics.SphereCast(ray, sphereCastRadius, out RaycastHit hit, 2f * halfRange, walkableLayers))
		{
			result[0] = hit.point;
			result[1] = hit.normal;
		}
		else
		{
			result[0] = point;
		}
		Physics.queriesHitBackfaces = doBackface;
		return result;
	}

	void Start()
	{
		lastBodyUp = transform.up;

		numberOfLegs = legTargets.Length;
		defaultLegPositions = new Vector3[numberOfLegs];
		lastLegPositions = new Vector3[numberOfLegs];
		legMoving = new bool[numberOfLegs];
		for (int i = 0; i < numberOfLegs; ++i)
		{
			defaultLegPositions[i] = legTargets[i].localPosition;
			lastLegPositions[i] = legTargets[i].position;
			legMoving[i] = false;
		}
		lastBodyPos = transform.position;
	}

	IEnumerator PerformStep(int index, Vector3 targetPoint)
	{
		legMoving[index] = true;
		Vector3 startPos = lastLegPositions[index];
		for (int i = 1; i <= smoothness; ++i)
		{
			legTargets[index].position = Vector3.Lerp(startPos, targetPoint, i / (float)(smoothness + 1f));
			legTargets[index].position += transform.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight;
			yield return new WaitForFixedUpdate();
		}
		legTargets[index].position = targetPoint;
		lastLegPositions[index] = legTargets[index].position;
		legMoving[index] = false;
	}

	void FixedUpdate()
	{
		// Check which legs should move
		// Find targets for the legs which have just been told to move (but not the ones which are already moving)
		// Move each leg.


		Vector3 velocity = transform.position - lastBodyPos;
		velocity = (velocity + smoothness * lastVelocity) / (smoothness + 1f);

		if (velocity.magnitude < 0.000025f) velocity = lastVelocity;
		else lastVelocity = velocity;

		Vector3[] desiredPositions = new Vector3[numberOfLegs];
		int indexToMove = -1;
		float maxDistance = stepSize;
		// Check if each leg should be moved or not.
		for (int i = 0; i < numberOfLegs; ++i)
		{
			desiredPositions[i] = transform.TransformPoint(defaultLegPositions[i]);

			float distance = Vector3.ProjectOnPlane(desiredPositions[i] + velocity * velocityMultiplier - lastLegPositions[i], transform.up).magnitude;
			if (distance > maxDistance)
			{
				maxDistance = distance;
				indexToMove = i;
			}
		}

		// For non moving legs put their new position to be their old position.
		for (int i = 0; i < numberOfLegs; ++i)
			if (i != indexToMove)
				legTargets[i].position = lastLegPositions[i];

		if (indexToMove != -1 && !legMoving[indexToMove])
		{
			Vector3 targetPoint = desiredPositions[indexToMove] + Mathf.Clamp(velocity.magnitude * velocityMultiplier, 0.0f, 1.5f) * (desiredPositions[indexToMove] - legTargets[indexToMove].position) + velocity * velocityMultiplier;

			Vector3[] positionAndNormalFwd = MatchToSurfaceFromAbove(
					targetPoint + velocity * velocityMultiplier,
					raycastRange,
					(transform.parent.up - velocity * 100).normalized
				);

			if (positionAndNormalFwd[1] == Vector3.zero)
			{
				Vector3[] positionAndNormalBwd = MatchToSurfaceFromAbove(
						targetPoint + velocity * velocityMultiplier,
						raycastRange * (1f + velocity.magnitude),
						(transform.parent.up + velocity * 75).normalized
					);
				StartCoroutine(PerformStep(indexToMove, positionAndNormalBwd[0]));
			}
			else
			{
				StartCoroutine(PerformStep(indexToMove, positionAndNormalFwd[0]));
			}
		}

		lastBodyPos = transform.position;
		if (numberOfLegs > 3 && bodyOrientation)
		{
			Vector3 v1 = legTargets[0].position - legTargets[1].position;
			Vector3 v2 = legTargets[2].position - legTargets[3].position;
			Vector3 normal = Vector3.Cross(v1, v2).normalized;
			Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(smoothness + 1));
			transform.up = up;
			transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
			lastBodyUp = transform.up;
		}
	}

	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < numberOfLegs; ++i)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(legTargets[i].position, 0.05f);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.TransformPoint(defaultLegPositions[i]), stepSize);
		}
	}
}
