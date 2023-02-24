using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialGuideTrigger : MonoBehaviour
{
/*	[SerializeField] public FeedNotification feedNotification;
	[SerializeField] public TMP_Text displayTextObject;*/
	[SerializeField] TutorialGuide tutorialGuide;
	[SerializeField] string guideText;
	// Start is called before the first frame update
	private void OnTriggerEnter(Collider other)
	{
		tutorialGuide.feedNotification.enabled = true;
		tutorialGuide.feedNotification.ExpandNotification();
		tutorialGuide.displayTextObject.text = guideText;
	}

	private void OnTriggerExit(Collider other)
	{
		tutorialGuide.displayTextObject.text = "";
		tutorialGuide.feedNotification.MinimizeNotification();
	}
}
