using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class DetectionLevel : MonoBehaviour
{
	[SerializeField]
	private float decreasePerSecond = 10;
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

	[SerializeField] ProgressBar detectionBar;

	private static float detectionLevel;
	private static float prevDetectionLevel;
	[SerializeField] public float currentDetectionLevel { get { return detectionLevel; } private set { detectionLevel = Mathf.Clamp(value, 0, 100); } }

	[SerializeField] private bool ghostMode = false;

	public void Start()
	{
		currentDetectionLevel = 0;
	}

	public void ChangeDetection(float change)
	{
		if (ghostMode) return;
		prevDetectionLevel = currentDetectionLevel;
		currentDetectionLevel += change;
		if (detectionBar) detectionBar.SetValue(currentDetectionLevel);
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
		}
		else if (prevDetectionLevel != 0 && currentDetectionLevel == 0)
		{
			if (onEmpty != null) onEmpty();
		}
	}

	public void Update()
	{
		ChangeDetection(-decreasePerSecond * Time.deltaTime);
	}
}
