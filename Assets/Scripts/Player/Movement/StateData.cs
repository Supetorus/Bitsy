using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// This describes data useful to the current state.
/// </summary>
public class StateData : MonoBehaviour
{
	[SerializeField, Tooltip("How far from the center of the spider it should check for clingable surfaces. This should match or be less than the farthest a leg can reach."), Min(0.000001f)]
	public float attachmentDistance;
	[SerializeField, Tooltip("The distance which the spider attaches from when it is falling. Must be less than attachment distance or else spider will instantly attach again when it falls or jumps."), Min(0.000001f)]
	public float lesserAttachmentDistance;
	[SerializeField, Tooltip("What layers should be considered walkable.")]
	public LayerMask walkableLayers;
	[SerializeField, Tooltip("The main camera")]
	public new Transform camera;

	[Header("Gizmo settings")]
	[SerializeField]
	private bool drawRays = true;
	//[SerializeField]
	//private bool drawHits = true;

	[HideInInspector]
	public Vector3 velocity;

	private void Start()
	{
		if (lesserAttachmentDistance > attachmentDistance) Debug.LogError("Lesser Attachment Distance must be less than Attachment Distance in 'State Data'");
		if ((walkableLayers & LayerMask.GetMask("Player")) > 0) Debug.LogError("Player cannot be in the layermask for 'State Data' Walkable Layers.");
		if (camera == null) Debug.LogError("Camera is not assigned in 'State Data'");
		if (lesserAttachmentDistance <= GetComponent<SphereCollider>().radius) Debug.LogError("Lesser attachment distance must be greater than collider's radius.");
	}

	// This is just used for the gizmo to display.
	//private float lastCheckDistance;
	//List<Vector3> hitPoints = new List<Vector3>();

	//private List<Vector3> SphereRaycastNormal(float checkDistance)
	//{
	//	List<Vector3> points = new List<Vector3>();
	//	foreach (var v in icosphereVertices)
	//	{
	//		Physics.Raycast(transform.position, v, out RaycastHit hit, checkDistance, walkableLayers);
	//		if (hit.collider != null)
	//		{
	//			points.Add(hit.normal);
	//		}
	//	}
	//	return points;
	//}

	#region ClosestPoint
	//private Vector3? closestPoint = null;
	/// <summary>
	/// Returns the closest (walkable) point to this transform.position. If there are no objects within radius then null is returned.
	/// It will also set the audio source depending on the surface's tag.
	/// </summary>
	/// <param name="radius">The spherical radius to check for nearby objects.</param>
	/// <returns></returns>
	//public Vector3? GetClosestPoint(float checkDistance, ref AudioSource audioSource)
	//{
	//	if (checkDistance <= 0)
	//	{
	//		Debug.LogError("You cannot check for walkable objects in a radius less than or equal to zero.");
	//		return null;
	//	}
	//	lastCheckDistance = checkDistance;
	//	// Collect the list of hits.
	//	List<RaycastHit> hits = new List<RaycastHit>();
	//	foreach (var v in icosphereVertices)
	//	{
	//		Physics.Raycast(transform.position, v, out RaycastHit hit, checkDistance, walkableLayers);
	//		if (hit.collider != null) hits.Add(hit);
	//	}

	//	// Calculate the closest point of those hits.
	//	closestPoint = null;
	//	float closestPointSqrDistance = float.MaxValue;
	//	foreach (RaycastHit hit in hits)
	//	{
	//		Vector3 point = hit.point;
	//		float distance = (transform.position - point).sqrMagnitude;
	//		if (distance < closestPointSqrDistance)
	//		{
	//			closestPointSqrDistance = distance;
	//			closestPoint = point;
	//		}
	//	}
	//	return closestPoint;
	//}
	#endregion

	private void OnDrawGizmos()
	{
		// If the program isn't running I can't get the icosphere mesh in order to calculate the raycast points,
		// so here are some consolation spheres.
		if (drawRays)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, attachmentDistance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, lesserAttachmentDistance);
			return;
		}

		//if (drawHits)
		//{
		//	Gizmos.color = Color.green;
		//	foreach (var p in hitPoints)
		//	{
		//		Gizmos.DrawSphere(p, 0.01f);
		//	}
		//}

		//if (drawRays)
		//{
			// If the program is running you get to see where the raycasts actually are.
			//Gizmos.color = Color.yellow;
			//foreach (Vector3 v in icosphereVertices)
			//{
			//	Gizmos.DrawLine(transform.position + Vector3.zero, transform.position + v * lastCheckDistance);
			//}
		//}

		// This draws a line to the closest point.
		//Gizmos.color = Color.red;
		//Gizmos.DrawLine(transform.position, closestPoint != null ? (Vector3)closestPoint : transform.position);
	}
}