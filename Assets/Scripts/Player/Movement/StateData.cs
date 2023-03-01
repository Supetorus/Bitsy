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

	[HideInInspector]
	public Vector3 velocity;

	private void Start()
	{
		if (lesserAttachmentDistance > attachmentDistance) Debug.LogError("Lesser Attachment Distance must be less than Attachment Distance in 'State Data'");
		if ((walkableLayers & LayerMask.GetMask("Player")) > 0) Debug.LogError("Player cannot be in the layermask for 'State Data' Walkable Layers.");
		if (camera == null) Debug.LogError("Camera is not assigned in 'State Data'");
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, attachmentDistance);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lesserAttachmentDistance);
	}
}