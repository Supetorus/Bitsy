using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	public List<Nodes> nodes;
	private int node_Count = 0;
	private float delay;

	void Update()
    {
		if(delay <= 0) {
			MovePlatform();
		} else {
			delay -= Time.deltaTime;
		}
    }

	public void MovePlatform() {

		transform.position = Vector3.MoveTowards(transform.position, nodes[node_Count].transform.position, 2 * Time.deltaTime);

		if (transform.position == nodes[node_Count].transform.position) {
			nodes[node_Count].last_reached = true;
			nodes[node_Count].previousNode.last_reached = false;
			node_Count++;
			if (node_Count >= nodes.Count) {
				node_Count = 0;
				delay = 4.0f;
			}
			if(node_Count == 6) {
				delay = 4.0f;
			}
		}
	}
}
