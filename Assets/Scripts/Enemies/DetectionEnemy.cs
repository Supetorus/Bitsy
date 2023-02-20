using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DetectionEnemy : MonoBehaviour
{
	public abstract bool CheckSightlines();
	public virtual void DartRespond() { }
	public virtual void EMPRespond(float stunDuration, GameObject stunEffect) { }
}
