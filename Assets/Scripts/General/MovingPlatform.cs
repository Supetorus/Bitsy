using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	public List<Nodes> nodes;
	public Nodes MiddleNode;
	public Nodes StartNode;
	private int node_Count = 0;
	private float delay;
	public float moveSpeed;

	void Update()
    {
		if(delay <= 0) {
			MovePlatform();
		} else {
			delay -= Time.deltaTime;
		}
    }

	public void MovePlatform() {

		transform.position = Vector3.MoveTowards(transform.position, nodes[node_Count].transform.position, moveSpeed * Time.deltaTime);

		if (transform.position == nodes[node_Count].transform.position) {
			nodes[node_Count].last_reached = true;
			nodes[node_Count].previousNode.last_reached = false;
			node_Count++;
			if (node_Count >= nodes.Count) {
				node_Count = 0;
			}
			if (nodes[node_Count].transform == MiddleNode.transform) {
				delay = 4.0f;
			}
			if(nodes[node_Count].transform == StartNode.transform) {
				delay = 4.0f;
			}
		}
	}

	public void OnDrawGizmos() {
		Gizmos.color = Color.red;
		//Gizmos.DrawLine(nodes[0].transform.position, nodes[nodes.Count - 1].transform.position);
		foreach (Nodes node in nodes) {
			Gizmos.DrawSphere(node.transform.position, 0.1f);
			Gizmos.DrawLine(node.transform.position, node.nextNode.transform.position);
		}
		if (!Application.isPlaying) return;
	}
}
