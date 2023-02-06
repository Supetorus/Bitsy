using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalPlayerDetection : MonoBehaviour
{
	[Tooltip("This event is called when global detection is at 25.")]
	public UnityEvent onQuarter;
	[Tooltip("This event is called when global detection is at 50.")]
	public UnityEvent onHalf;
	[Tooltip("This event is called when global detection is at 75.")]
	public UnityEvent onThreeFourths;
	[Tooltip("This event is called when global detection is at 100.")]
	public UnityEvent onFull;

	[SerializeField] float decreaseOverTime;
	private static float previousGloabalDetectionLevel;
	private static float CurrentGlobalDetectionLevel { get { return CurrentGlobalDetectionLevel; } set { CurrentGlobalDetectionLevel = Mathf.Clamp(value, 0, 100); } }
	public List<GameObject> allEnemies;
    // Start is called before the first frame update
    void Start()
    {
		allEnemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));       
    }

	public static void ChangeDetection(float change, bool isIncrease)
	{
		previousGloabalDetectionLevel = CurrentGlobalDetectionLevel;
		if(isIncrease){
			CurrentGlobalDetectionLevel += change;
		} else
		{
			CurrentGlobalDetectionLevel -= change;
		}
	}

	public void Update()
	{
		
	}

	//Returns true if any enemy can see the player.
	public bool PlayerInSight()
	{
		foreach(var enemy in allEnemies)
		{
			if (enemy.GetComponent<Enemy>().CheckSightlines())return true;
		}
		return false;
	}
}
