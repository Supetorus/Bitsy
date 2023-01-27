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
	[SerializeField, Tooltip("The density of points on the icosphere, used for calculating raycast points around the player."), Min(2)]
	public int icosphereDensity = 2;

	[Header("Gizmo settings")]
	[SerializeField]
	private bool drawRays = true;
	[SerializeField]
	private bool drawHits = true;

	[HideInInspector]
	public Vector3 velocity;

	// An icosphere is just a geometric shape. It's being used to generate a sphere of raycasts.
	//private Mesh icosphere;
	//private Mesh Icosphere
	//{
	//	get
	//	{
	//		if (icosphere == null)
	//		{
	//			icosphere = IcosphereCreator.Create(icosphereDensity, 1);
	//		}
	//		return icosphere;
	//	}
	//}

	private Vector3[] icosphereVertices;

	private void Start()
	{
		if (lesserAttachmentDistance > attachmentDistance) Debug.LogError("Lesser Attachment Distance must be less than Attachment Distance in 'State Data'");
		if ((walkableLayers & LayerMask.GetMask("Player")) > 0) Debug.LogError("Player cannot be in the layermask for 'State Data' Walkable Layers.");
		if (camera == null) Debug.LogError("Camera is not assigned in 'State Data'");



		Mesh icosphere = IcosphereCreator.Create(icosphereDensity, 1);
		List<Vector3> verts = new List<Vector3>();
		foreach(var v in icosphere.vertices)
		{
			if (!verts.Contains(v)) verts.Add(v);
		}
		icosphereVertices = verts.ToArray();
	}

	// This is just used for the gizmo to display.
	private float lastCheckDistance;

	List<Vector3> hitPoints = new List<Vector3>();
	/// <summary>
	/// Returns a list of points that have been hit by SphereRaycast.
	/// </summary>
	/// <param name="checkDistance"></param>
	/// <returns></returns>
	private List<Vector3> SphereRaycast(float checkDistance)
	{
		//hitPoints.Clear();
		hitPoints = new List<Vector3>();
		int vCount = 0;
		foreach (var v in icosphereVertices)
		{
			Physics.Raycast(transform.position, v, out RaycastHit hit, checkDistance, walkableLayers);
			if (hit.collider != null) hitPoints.Add(hit.point);
			if (v == Vector3.up) vCount++;
		}
		return hitPoints;
	}

	private List<Vector3> SphereRaycastNormal(float checkDistance)
	{
		//hitPoints.Clear();
		hitPoints = new List<Vector3>();
		int vCount = 0;
		foreach (var v in icosphereVertices)
		{
			Physics.Raycast(transform.position, v, out RaycastHit hit, checkDistance, walkableLayers);
			if (hit.collider != null)
			{
				hitPoints.Add(hit.normal);
				//Debug.DrawRay(hit.point, hit.normal * 3, Color.cyan);
			}

			if (v == Vector3.up) vCount++;
		}
		return hitPoints;
	}


	/// <summary>
	/// Uses a hemisphere of points below the player (relative to the player) to calculate an average up direction.
	/// </summary>
	/// <param name="checkDistance"></param>
	/// <returns></returns>
	public Vector3 CalculateAverageUp(float checkDistance)
	{
		List<Vector3> points = SphereRaycastNormal(checkDistance);

		List<Vector3> pointsBelowPlayer = new List<Vector3>();
		foreach (Vector3 point in points)
		{
			if (Vector3.Dot(point, -transform.up) > 0)
			{
				pointsBelowPlayer.Add(point);
			}
		}

		Vector3 average = Vector3.zero;
		foreach (var point in points)
		{
			average += point;
		}
		//Debug.DrawRay(transform.position, average.normalized * 5, Color.yellow);
		return average.normalized;
	}

	#region ClosestPoint
	private Vector3? closestPoint = null;
	/// <summary>
	/// Returns the closest (walkable) point to this transform.position. If there are no objects within radius then null is returned.
	/// </summary>
	/// <param name="radius">The spherical radius to check for nearby objects.</param>
	/// <returns></returns>
	public Vector3? GetClosestPoint(float checkDistance)
	{
		if (checkDistance <= 0)
		{
			Debug.LogError("You cannot check for walkable objects in a radius less than or equal to zero.");
			return null;
		}
		lastCheckDistance = checkDistance;
		// Collect the list of hits.
		List<Vector3> hits = SphereRaycast(checkDistance);

		// Calculate the closest point of those hits.
		closestPoint = null;
		float closestPointSqrDistance = float.MaxValue;
		foreach (Vector3 point in hits)
		{
			float distance = (transform.position - point).sqrMagnitude;
			if (distance < closestPointSqrDistance)
			{
				closestPointSqrDistance = distance;
				closestPoint = point;
			}
		}
		return closestPoint;
	}

	/// <summary>
	/// Returns the closest (walkable) point to this transform.position. If there are no objects within radius then null is returned.
	/// It will also set the audio source depending on the surface's tag.
	/// </summary>
	/// <param name="radius">The spherical radius to check for nearby objects.</param>
	/// <returns></returns>
	public Vector3? GetClosestPoint(float checkDistance, ref AudioSource audioSource)
	{
		if (checkDistance <= 0)
		{
			Debug.LogError("You cannot check for walkable objects in a radius less than or equal to zero.");
			return null;
		}
		lastCheckDistance = checkDistance;
		// Collect the list of hits.
		List<RaycastHit> hits = new List<RaycastHit>();
		foreach (var v in icosphereVertices)
		{
			Physics.Raycast(transform.position, v, out RaycastHit hit, checkDistance, walkableLayers);
			if (hit.collider != null) hits.Add(hit);
		}

		// Calculate the closest point of those hits.
		closestPoint = null;
		float closestPointSqrDistance = float.MaxValue;
		foreach (RaycastHit hit in hits)
		{
			Vector3 point = hit.point;
			float distance = (transform.position - point).sqrMagnitude;
			if (distance < closestPointSqrDistance)
			{
				closestPointSqrDistance = distance;
				closestPoint = point;
			}
		}
		return closestPoint;
	}
	#endregion

	private void OnDrawGizmos()
	{
		// If the program isn't running I can't get the icosphere mesh in order to calculate the raycast points,
		// so here are some consolation spheres.
		if (!Application.isPlaying && drawRays)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, attachmentDistance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, lesserAttachmentDistance);
			return;
		}

		if (drawHits)
		{
			Gizmos.color = Color.green;
			foreach (var p in hitPoints)
			{
				Gizmos.DrawSphere(p, 0.01f);
			}
		}

		if (drawRays)
		{
			// If the program is running you get to see where the raycasts actually are.
			Gizmos.color = Color.yellow;
			foreach (Vector3 v in icosphereVertices)
			{
				Gizmos.DrawLine(transform.position + Vector3.zero, transform.position + v * lastCheckDistance);
			}
		}

		// This draws a line to the closest point.
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, closestPoint != null ? (Vector3)closestPoint : transform.position);
	}
}