using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
	public GameObject _player;
	private Vector3 startLocation;

	private void Start()
	{
		startLocation.x = _player.transform.position.x;
		startLocation.y = _player.transform.position.y;
		startLocation.z = _player.transform.position.z;
		Debug.Log("X: " + startLocation.x);
		Debug.Log("Y: " + startLocation.y);
		Debug.Log("Z: " + startLocation.z);
		Debug.Log("Player X: " + _player.transform.position.x);
		Debug.Log("Player Y: " + _player.transform.position.y);
		Debug.Log("Player Z: " + _player.transform.position.z);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Debug.Log("Player has hit death box and fell out of the map");
			_player.transform.position = startLocation;
			Debug.Log("Player X: " + _player.transform.position.x);
			Debug.Log("Player Y: " + _player.transform.position.y);
			Debug.Log("Player Z: " + _player.transform.position.z);
		}
	}
}
