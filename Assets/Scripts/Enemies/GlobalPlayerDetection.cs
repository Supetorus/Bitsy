using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GlobalPlayerDetection : MonoBehaviour
{
	public delegate void DetectionAction();

	[Tooltip("This event is called when global detection is at 0.")]
	public static event DetectionAction onEmpty;
	[Tooltip("This event is called when global detection is at 25.")]
	public static event DetectionAction onQuarter;
	[Tooltip("This event is called when global detection is at 50.")]
	public static event DetectionAction onHalf;
	[Tooltip("This event is called when global detection is at 75.")]
	public static event DetectionAction onThreeFourths;
	[Tooltip("This event is called when global detection is at 100.")]
	public static event DetectionAction onFull;

	[SerializeField] float decreaseOverTime;
	[SerializeField] float decreaseHowOften;

	private static float previousGloabalDetectionLevel;
	private static float detectionLevel;
	[SerializeField] public float currentDetectionLevel { get { return detectionLevel; } set { detectionLevel = Mathf.Clamp(value, 0, 100); } }
	public List<GameObject> allEnemies;
	public bool detectionChanged = false;
    // Start is called before the first frame update
    void Start()
    {
		allEnemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
    }

	public void ChangeDetection(float change, bool isIncrease)
	{
		previousGloabalDetectionLevel = currentDetectionLevel;
		if(isIncrease){
			currentDetectionLevel += change;
		} else
		{
			currentDetectionLevel -= change;
		}
		CheckEvents();
	}

	public void CheckEvents()
	{
		if (currentDetectionLevel >= 95)
		{
			if (onFull != null) onFull();
		}
		else if (currentDetectionLevel >= 75)
		{
			if (onThreeFourths != null) onThreeFourths();
		}
		else if (currentDetectionLevel >= 50)
		{
			if (onHalf != null) onHalf();
		}
		else if (currentDetectionLevel >= 25)
		{
			if (onQuarter != null) onQuarter();
		} else if(currentDetectionLevel == 0)
		{
			if (onEmpty != null) onEmpty();
		}
	}


	public void Update() 
	{
		if (!PlayerInSight())
		{
			StartCoroutine(DecreaseDetection());
		}
		else
		{
			StopCoroutine(DecreaseDetection());
		}
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

	IEnumerator DecreaseDetection()
	{
		yield return new WaitForSeconds(decreaseHowOften);
		ChangeDetection(decreaseOverTime, false);
	}
}
