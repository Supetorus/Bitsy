using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
	public int currentAmmo;
    protected AbilityController abCon;
    public abstract void UseAbility();
    public virtual void DeactivateAbility() { }
}
