using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField] Health health;
	[SerializeField] GameObject lineOfSight;
	[SerializeField] Transform eyes;
	public bool playerInVision;
	public List<Nodes> nodes;
	private int node_Count = 0;

	[SerializeField] private float time = 1f;
	public float timer;
	[SerializeField] private float fullAwareDelay = 5.0f;
	public float fullAwareTimer;

	public AbilityController player;

	[SerializeField] private LayerMask layerMask;

	public enum Mode
	{
		PATROL,
		SUSPICIOUS,
		INVESTIGATING,
		AWARE
	}

	[SerializeField] private float Awareness = 0;
	public float awareness { get { return Awareness; } set { Awareness = Mathf.Clamp(value, 0, 100); } }
	// Start is called before the first frame update
	void Start()
	{
		timer = time;
		fullAwareTimer = fullAwareDelay;
	}

	// Update is called once per frame
	void Update()
	{
		if (awareness == 100)
		{
			if (player != null) player.Detected = true;
			if (!CheckSightlines())
			{
				fullAwareTimer -= Time.deltaTime;

				if (fullAwareTimer <= 0)
				{
					awareness -= 5;
					fullAwareTimer = fullAwareDelay;
				}
			}
		}
		else if (awareness <= 0)
		{
			//Debug.Log("Awareness: 0");
			if (player != null)
			{
				//Debug.Log("Player not null");
				player.Detected = false;
				//Debug.Log("Detected set to false");
				player = null;
				//Debug.Log("Player set to null");
			}
		}
		else
		{
			timer -= Time.deltaTime;
			if (timer <= 0 && !CheckSightlines())
			{
				awareness -= 5;
				timer = time;
			}
		}
		print(awareness);
		MoveDrone();
	}

	public bool CheckSightlines()
	{
		if (player == null) return false;
		if (playerInVision)
		{
			Debug.Log("This is getting hit");
			if (Physics.Raycast(eyes.position, (player.spiderCenter.transform.position - eyes.position), out RaycastHit hit, Mathf.Infinity, layerMask))
			{
				if (hit.collider.gameObject.tag == "Smoke") return false;
				return (hit.collider.gameObject.tag == "Player" && player.isVisible);
			}
		}
		return false;
	}

	public void KnockOut()
	{

	}

	public void MoveDrone()
	{
		//Debug.Log(nodes[node_Count].transform.position.x + " " + nodes[node_Count].transform.position.y + " " + nodes[node_Count].transform.position.z);

		transform.position = Vector3.MoveTowards(transform.position, nodes[node_Count].transform.position, 5 * Time.deltaTime);
		transform.LookAt(nodes[node_Count].transform.position);

		//Debug.Log("Node Number: " + node_Count);
		//Debug.Log("Node Count: " + nodes.Count);
		if (transform.position == nodes[node_Count].transform.position)
		{
			nodes[node_Count].last_reached = true;
			nodes[node_Count].previousNode.last_reached = false;
			node_Count++;
			if (node_Count >= nodes.Count)
			{
				node_Count = 0;
			}
		}
	}
	public void Stun()
	{
		print(gameObject.name + "I'm stunned!");
	}

  public void OnDrawGizmos() {
    Gizmos.color = Color.red;
    Gizmos.DrawLine(nodes[0].transform.position, nodes[nodes.Count - 1].transform.position);
    foreach(Nodes node in nodes)
    {
      Gizmos.DrawSphere(node.transform.position, 0.1f);
      Gizmos.DrawLine(node.transform.position, node.nextNode.transform.position);
    }
    if (!Application.isPlaying) return;
    if (player != null) Gizmos.DrawLine(eyes.position, player.spiderCenter.transform.position);
  }
}
