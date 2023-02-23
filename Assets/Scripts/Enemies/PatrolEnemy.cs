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
	[SerializeField] GameObject projectile;
	[SerializeField] float maxPlayerDist;
	[SerializeField] float sightDist;
	[SerializeField] float projSpeed;
	[SerializeField] float fireRate;

	private float fireTimer;
	private GameObject targetNode;
	private int nodeIndex;
	private bool canSeePlayer;
	private GameObject player;
	private Vector3 playerDir;
	private Vector3 feetPos;

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
		feetPos = new Vector3(transform.position.x, transform.position.y - agent.baseOffset, transform.position.z);
		playerDir = (player.transform.position - eyes.position).normalized;
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
			if (Vector3.Distance(feetPos, targetNode.transform.position) < 0.06f)
			{
				if (nodeIndex == nodes.Count - 1) ChangeDestination(0);
				else ChangeDestination(++nodeIndex);
			}
		} 
		else 
		{	if (Vector3.Distance(feetPos, player.transform.position) > maxPlayerDist)
			{
				agent.isStopped = false;
				agent.SetDestination(player.transform.position);
			}
			else
			{
				agent.isStopped = true;
				if (fireTimer <= 0)
				{
					GameObject bullet = Instantiate(projectile, gun.transform.position, gun.transform.rotation);
					bullet.transform.rotation = Quaternion.LookRotation(playerDir);
					bullet.GetComponent<Rigidbody>().AddForce(playerDir * projSpeed);
					
					fireTimer = fireRate;
				}
			}
			fireTimer -= Time.deltaTime;
		}
    }
}
