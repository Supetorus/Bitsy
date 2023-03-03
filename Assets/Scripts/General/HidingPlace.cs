using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out AbilityController controller)) controller.isVisible = false;
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out AbilityController controller)) controller.isVisible = true;
	}
}
