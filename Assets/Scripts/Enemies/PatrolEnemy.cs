using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : DetectionEnemy
{
	[SerializeField] private DetectionEnemy detector;
	[SerializeField] private float detectionThreshhold = 75;
	[SerializeField] private bool canSeePlayer;
	[SerializeField, Tooltip("How close the player has to be before this enemy cares and will try to find them")]
	private float playerCareDistance = 20;
	[Header("Navigation")]
	[SerializeField] List<GameObject> nodes;
	[SerializeField] NavMeshAgent agent;
	[SerializeField] Animator animator;
	[SerializeField] float maxPlayerDistance;
	[Header("Shooting")]
	[SerializeField] Transform gun;
	[SerializeField] GameObject projectile;
	[SerializeField] float projSpeed;
	[SerializeField] float fireRate;
	[Header("Animation")]
	[SerializeField] Transform hipsJoint;
	[SerializeField] Transform defaultHips;
	[SerializeField] GameObject deathExplode;

	private Quaternion hipsRot;
	private float fireTimer;
	private GameObject targetNode;
	private int nodeIndex;
	private Vector3 feetPos { get => new Vector3(transform.position.x, transform.position.y - agent.baseOffset, transform.position.z); }
	private float minDistanceThreshhold = 0.1f;

	public override void DartRespond()
	{
		Instantiate(deathExplode, transform.position, transform.rotation);
		Destroy(gameObject, 0.5f);
	}

	void Start()
	{
		animator.SetBool("ShouldWalk", nodes.Count > 0 && (nodes.Count == 1 && Vector3.Distance(feetPos, nodes[0].transform.position) > minDistanceThreshhold));
		hipsRot = hipsJoint.rotation;
		ChangeDestination(0);
		fireTimer = fireRate;
	}

	void Update()
	{
		if (Vector3.Distance(Player.Transform.position, feetPos) < playerCareDistance &&  Player.Detection.currentDetectionLevel > detectionThreshhold)
		{
			canSeePlayer = true;
			Physics.Raycast(detector.transform.position, Player.Transform.position - detector.transform.position, out RaycastHit hit, float.PositiveInfinity);
			if (Vector3.Distance(gun.position, Player.Transform.position) > maxPlayerDistance || !hit.collider.CompareTag("Player"))
			{
				//hipsJoint.rotation = Quaternion.Slerp(
				//	hipsJoint.rotation,
				//	defaultHips.rotation,
				//	Quaternion.Angle(hipsJoint.rotation, defaultHips.rotation) / 420);
				animator.SetBool("ShouldWalk", true);
				animator.SetBool("ShouldShoot", false);
				agent.isStopped = false;
				agent.SetDestination(Player.Transform.position);
			}
			else
			{
				//Quaternion newHips = Quaternion.LookRotation(Player.Transform.position - hipsJoint.position) * Quaternion.Euler(0, -90, 0) * hipsRot;
				//hipsJoint.rotation = Quaternion.Slerp(
				//	hipsJoint.rotation,
				//	newHips,
				//	Quaternion.Angle(hipsJoint.rotation, newHips) / 420);

				animator.SetBool("ShouldWalk", false);
				animator.SetBool("ShouldShoot", true);
				agent.isStopped = true;
				if (fireTimer <= 0 /*&& Quaternion.Angle(hipsJoint.rotation, newHips) < 25f*/)
				{
					GameObject bullet = Instantiate(projectile, gun.transform.position, Quaternion.LookRotation(Player.Transform.position - gun.transform.position));
					bullet.GetComponentInChildren<Rigidbody>().AddForce((Player.Transform.position - gun.transform.position).normalized * projSpeed);

					fireTimer = fireRate;
				}
			}
		}
		else
		{
			canSeePlayer = false;
			animator.SetBool("ShouldWalk", (nodes.Count == 1 && Vector3.Distance(feetPos, nodes[0].transform.position) > minDistanceThreshhold) || nodes.Count > 0);
			animator.SetBool("ShouldShoot", false);
			agent.isStopped = false;
			if (Vector3.Distance(feetPos, targetNode.transform.position) < minDistanceThreshhold)
			{
				ChangeDestination((nodeIndex + 1) % nodes.Count);
			}
			else
			{
				agent.SetDestination(nodes[nodeIndex].transform.position);
			}
		}
		fireTimer -= Time.deltaTime;
	}


	public void ChangeDestination(int nodeNum)
	{
		agent.SetDestination(nodes[nodeNum].transform.position);
		targetNode = nodes[nodeNum];
		nodeIndex = nodeNum;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(feetPos, maxPlayerDistance);
		Gizmos.color = new Color(0.95f, 0.65f, 0.25f);
		Gizmos.DrawWireSphere(feetPos, playerCareDistance);
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

	public override bool CheckSightlines()
	{
		return canSeePlayer;
	}

	public override void EMPRespond(float stunDuration, GameObject stunEffect)
	{
		StartCoroutine(GetStunnedIdiot(stunDuration, stunEffect));
	}

	IEnumerator GetStunnedIdiot(float stunDuration, GameObject stunEffect)
	{
		agent.isStopped = true;
		yield return new WaitForSeconds(stunDuration);
		agent.isStopped = false;
	}
}
