using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		print("NOW HIDE");
		if (other.TryGetComponent(out AbilityController controller)) controller.isHiding = true;
	}
	private void OnTriggerExit(Collider other)
	{
		print("BYE BYE");
		if (other.TryGetComponent(out AbilityController controller)) controller.isHiding = false;
	}
}
