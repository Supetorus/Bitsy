using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : DetectionEnemy
{
	[Header("Navigation")]
	[SerializeField] List<GameObject> nodes;
	[SerializeField] NavMeshAgent agent;
	[SerializeField] Animator animator;
	[Header("Shooting")]
	[SerializeField] Transform gun;
	[SerializeField] GameObject projectile;
	[SerializeField] float projSpeed;
	[SerializeField] float fireRate;
	[Header("Detection")]
	[SerializeField] float maxPlayerDist;
	[SerializeField, Range(0, 180)] private float maxAngle;
	[SerializeField] float sightDist;
	[SerializeField] private LayerMask layerMask;
	[SerializeField] Transform eyes;
	[Header("Detection Area Display")]
	[SerializeField] private int lineCount;
	[SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private float beamRotationSpeed;
	[Header("Animation")]
	[SerializeField] Transform hipsJoint;
	[SerializeField] Transform hips;

	private Quaternion hipsRot;
	private float fireTimer;
	private GameObject targetNode;
	private int nodeIndex;
	private bool canSeePlayer;
	private GameObject player;
	private Vector3 playerDir;
	private Vector3 feetPos { get => new Vector3(transform.position.x, transform.position.y - agent.baseOffset, transform.position.z); }
	private float minDistanceThreshhold = 0.06f;
	private List<LineRenderer> lines = new List<LineRenderer>();

	void Start()
	{
		hipsRot = hipsJoint.rotation;
		player = GameObject.FindGameObjectWithTag("Player");
		ChangeDestination(0);
		fireTimer = fireRate;
	}

	void Update()
	{
		canSeePlayer = ConeDetection(maxAngle, player.transform, layerMask, eyes.position);
		animator.SetBool("CanSeePlayer", canSeePlayer);
		if (canSeePlayer) player.GetComponent<GlobalPlayerDetection>().ChangeDetection(100 * Time.deltaTime);

		DrawCone(lineCount, lines, lineRenderer, beamRotationSpeed, maxAngle, eyes.position);
		//playerDir = (player.transform.position - eyes.position).normalized;
		//if (Physics.Raycast(eyes.position, playerDir, out RaycastHit hit, sightDist, layerMask) &&
		//	hit.collider.gameObject == player &&
		//	player.GetComponent<AbilityController>().isVisible)
		//{
		//	animator.SetBool("CanSeePlayer", true);
		//	canSeePlayer = true;
		//	player.GetComponent<GlobalPlayerDetection>().ChangeDetection(100 * Time.deltaTime);
		//}
		//else
		//{
		//	animator.SetBool("CanSeePlayer", false);
		//	canSeePlayer = false;
		//	ChangeDestination(nodeIndex);
		//}

		if (!canSeePlayer)
		{
			agent.isStopped = false;
			if (Vector3.Distance(feetPos, targetNode.transform.position) < minDistanceThreshhold)
			{
				ChangeDestination((nodeIndex + 1) % nodes.Count);
			}
		}
		else
		{
			if (Vector3.Distance(feetPos, player.transform.position) > maxPlayerDist)
			{
				hipsJoint.rotation = Quaternion.Slerp(
				hipsJoint.rotation,
				hips.rotation,
				Quaternion.Angle(hipsJoint.rotation, hips.rotation) / 420);

				animator.SetBool("CanSeePlayer", false);
				agent.isStopped = false;
				agent.SetDestination(player.transform.position);
			}
			else
			{
				Quaternion newHips = Quaternion.LookRotation(player.transform.position - hipsJoint.position) * Quaternion.Euler(0, -90, 0) * hipsRot;
				hipsJoint.rotation = Quaternion.Slerp(
				hipsJoint.rotation,
				newHips,
				Quaternion.Angle(hipsJoint.rotation, newHips) / 420);

				animator.SetBool("CanSeePlayer", true);
				agent.isStopped = true;
				if (fireTimer <= 0 && Quaternion.Angle(hipsJoint.rotation, newHips) < 25f)
				{
					GameObject bullet = Instantiate(projectile, gun.transform);
					bullet.transform.rotation = Quaternion.LookRotation((player.transform.position - gun.transform.position).normalized);
					bullet.GetComponentInChildren<Rigidbody>().AddForce((player.transform.position - gun.transform.position).normalized * projSpeed);

					fireTimer = fireRate;
				}
			}
			fireTimer -= Time.deltaTime;
		}
	}

	public override bool CheckSightlines()
	{
		return canSeePlayer;
	}

	public override void DartRespond()
	{
		//MUST IMPLEMENT
		Destroy(gameObject);
	}

	public void ChangeDestination(int nodeNum)
	{
		agent.SetDestination(nodes[nodeNum].transform.position);
		targetNode = nodes[nodeNum];
		nodeIndex = nodeNum;
	}

	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			if (i == nodeIndex)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(nodes[i].transform.position, minDistanceThreshhold);
				Gizmos.DrawLine(feetPos, nodes[i].transform.position);
			}
			else
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(nodes[i].transform.position, minDistanceThreshhold);
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(nodes[i].transform.position, nodes[(i + 1) % nodes.Count].transform.position);
		}
	}
}
