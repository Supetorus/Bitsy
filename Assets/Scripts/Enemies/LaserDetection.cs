using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetection : MonoBehaviour
{
	public bool doesDamage;
	public List<GameObject> dronesToActive;
	public List<Enemy> scriptsToActive;

	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log("Laser: Collided");
		//Debug.Log(other.gameObject.name);
		if (other.gameObject.CompareTag("Player"))
		{
			//Debug.Log("Laser: Player found");
			if (doesDamage)
			{
				//Lower the players Health
				Debug.Log("Laser: You have taken damage");
			}
			else
			{
				//Increase the detection meter
				other.gameObject.GetComponent<Detection>().DetectionValue = 100;
				if(dronesToActive.Count > 0) {
					foreach(var drone in dronesToActive) {
						drone.transform.GetChild(3).gameObject.SetActive(true);
						//Turn on their nodes to foll
					}
					foreach (var enemy in scriptsToActive) {
						enemy.onHunt = true;
					}
				}
				foreach (var alarm in FindObjectsOfType<Alarm>())
				{
					alarm.Play();
				} 
				//Debug.Log("Laser: You Have Been Spotted");
			}
		}
	}

	/*private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Laser: Collided");
		Debug.Log()
		if (collision.collider.gameObject.CompareTag("Player"))
		{
			Debug.Log("Laser: Player found");
			if (doesDamage)
			{
				//Lower the players Health
				Debug.Log("Laser: You have taken damage");
			}
			else
			{
				//Increase the detection meter
				collision.collider.gameObject.GetComponent<Detection>().DetectionValue = 100;
				foreach (var alarm in FindObjectsOfType<Alarm>())
				{
					alarm.Play();
				}
				Debug.Log("Laser: You Have Been Spotted");
			}
		}
	}*/
}
