using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class SphereRaycaster
{
	public static bool detectBackfaces = true;
	private static Vector3[] icosphereVertices;
	//The density of points on the icosphere, used for calculating raycast points around the player.
	private static int icosphereDensity = 5;

	private static bool initialized = false;
	private static void Init()
	{
		if (initialized) return;
		initialized = true;
		// For some reason I don't care to explore right now, the resulting mesh
		// contains duplicate vertices, so I'm filtering those out so they don't
		// interfere with calculations or take up excess memory.
		Mesh icosphere = IcosphereCreator.Create(icosphereDensity, 1);
		List<Vector3> verts = new List<Vector3>();
		foreach (var v in icosphere.vertices)
		{
			if (!verts.Contains(v)) verts.Add(v);
		}
		icosphereVertices = verts.ToArray();
	}

	/// <summary>
	/// Returns a list of points that have been hit by SphereRaycast.
	/// </summary>
	/// <param name="checkDistance"></param>
	/// <returns></returns>
	public static List<RaycastHit> SphereRaycast(Vector3 position, float checkDistance, LayerMask layerMask)
	{
		Init();
		bool backfaceFlip;
		if (detectBackfaces) { Physics.queriesHitBackfaces = true; backfaceFlip = true; }
		else { Physics.queriesHitBackfaces = false; backfaceFlip = true; }
		List<RaycastHit> hits = new List<RaycastHit>();

		foreach (var v in icosphereVertices)
		{
			Physics.Raycast(position, v, out RaycastHit hit, checkDistance, layerMask);
			if (hit.collider != null) hits.Add(hit);
		}
		if (backfaceFlip) Physics.queriesHitBackfaces = !Physics.queriesHitBackfaces;
		return hits;
	}

	/// <summary>
	/// Uses a hemisphere of points below the player (relative to the player) to calculate an average up direction.
	/// </summary>
	/// <param name="checkDistance"></param>
	/// <returns></returns>
	public static Vector3 CalculateAverageUp(List<RaycastHit> hits, Vector3 position, Vector3 currentUp)
	{
		//var hits = SphereRaycast(position, checkDistance, layerMask);
		List<Vector3> directions = new List<Vector3>();
		foreach (var hit in hits)
		{
			directions.Add(position - hit.point);
		}

		List<Vector3> pointsBelowPlayer = new List<Vector3>();
		foreach (Vector3 point in directions)
		{
			if (Vector3.Dot(point, -currentUp) > 0) // Ignore hits that are above, so the spider won't bounce between two close surfaces.
			{
				pointsBelowPlayer.Add(point);
			}
		}

		Vector3 average = Vector3.zero;
		foreach (var point in directions)
		{
			average += point;
		}
		return average.normalized;
	}

	/// <summary>
	/// Returns the closest (walkable) point to this transform.position. If there are no objects within radius then null is returned.
	/// </summary>
	/// <param name="radius">The spherical radius to check for nearby objects.</param>
	/// <returns></returns>
	public static Vector3? GetClosestPoint(List<RaycastHit> hits, Vector3 position)
	{
		//lastCheckDistance = checkDistance;
		// Collect the list of hits.
		List<Vector3> points = new List<Vector3>();
		foreach (var hit in hits) points.Add(hit.point);

		// Calculate the closest point of those hits.
		Vector3? closestPoint = null;
		float closestPointSqrDistance = float.MaxValue;
		foreach (Vector3 point in points)
		{
			float distance = (position - point).sqrMagnitude;
			if (distance < closestPointSqrDistance)
			{
				closestPointSqrDistance = distance;
				closestPoint = point;
			}
		}
		return closestPoint;
	}
}
