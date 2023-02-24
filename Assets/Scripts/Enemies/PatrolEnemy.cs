using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : DetectionEnemy
{
	[SerializeField] List<GameObject> nodes;
	[SerializeField] Animator animator;
	[SerializeField] NavMeshAgent agent;
	[SerializeField] Transform eyes;
	[SerializeField] Transform gun;
	[SerializeField] Transform hipsJoint;
	[SerializeField] Transform hips;
	[SerializeField] GameObject projectile;
	[SerializeField] float maxPlayerDist;
	[SerializeField] float sightDist;
	[SerializeField] float projSpeed;
	[SerializeField] float fireRate;

	private Quaternion hipsRot;
	private float fireTimer;
	private GameObject targetNode;
	private int nodeIndex;
	private bool canSeePlayer;
	private GameObject player;
	private Vector3 playerDir;
	private Vector3 feetPos { get => new Vector3(transform.position.x, transform.position.y - agent.baseOffset, transform.position.z); }
	private float minDistanceThreshhold = 0.06f;

	public override bool CheckSightlines()
	{
		return canSeePlayer;
	}

	public override void DartRespond()
	{
		//MUST IMPLEMENT
		Destroy(gameObject);
	}

	// Start is called before the first frame update
	void Start()
    {
		hipsRot = hipsJoint.rotation;
		player = GameObject.FindGameObjectWithTag("Player");
		ChangeDestination(0);
		fireTimer = fireRate;
	}

	public void ChangeDestination(int nodeNum)
	{
		agent.SetDestination(nodes[nodeNum].transform.position);
		targetNode = nodes[nodeNum];
		nodeIndex = nodeNum;
	}

    // Update is called once per frame
    void Update()
    {
		playerDir = (player.transform.position - eyes.position).normalized;
		Debug.DrawRay(gun.transform.position, (player.transform.position - gun.transform.position).normalized);
		if (Physics.Raycast(eyes.position, playerDir, out RaycastHit hit, sightDist))
		{
			if (hit.collider.gameObject == player && player.GetComponent<AbilityController>().isVisible)
			{
				animator.SetBool("CanSeePlayer", true);
				canSeePlayer = true;
				player.GetComponent<GlobalPlayerDetection>().ChangeDetection(0.25f, true);
			}
			else
			{
				animator.SetBool("CanSeePlayer", false);
				canSeePlayer = false;
				ChangeDestination(nodeIndex);
			}
		}

		if (!canSeePlayer)
		{
			if(agent.isStopped) agent.isStopped = false;
			if (Vector3.Distance(feetPos, targetNode.transform.position) < minDistanceThreshhold)
			{
				if (nodeIndex == nodes.Count - 1) ChangeDestination(0);
				else ChangeDestination(++nodeIndex);
			}
		} 
		else 
		{	if (Vector3.Distance(feetPos, player.transform.position) > maxPlayerDist)
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
					bullet.GetComponent<Rigidbody>().AddForce((player.transform.position - gun.transform.position).normalized * projSpeed);
					
					fireTimer = fireRate;
				}
			}
			fireTimer -= Time.deltaTime;
		}
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
