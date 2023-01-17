using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] public float abilityTime;
    [SerializeField] public float cooldownTime;
    [HideInInspector] public float abilityTimer;
    [HideInInspector] public float cooldownTimer;

    protected AbilityController abCon;
    public abstract void UseAbility();
    public abstract void DeactivateAbility();
}
