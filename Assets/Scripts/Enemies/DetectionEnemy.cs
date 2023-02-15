using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DetectionEnemy : MonoBehaviour
{
	public abstract bool CheckSightlines();
	public abstract void DartRespond();
	public abstract void EMPRespond(float stunDuration, GameObject stunEffect);
}
