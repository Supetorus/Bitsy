using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
   public void ResetPlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
}
