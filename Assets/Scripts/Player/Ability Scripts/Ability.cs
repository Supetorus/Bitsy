using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    protected AbilityController abCon;
    public abstract void UseAbility();
    public abstract void DeactivateAbility();
}
