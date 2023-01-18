using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityController : MonoBehaviour
{
    //Ability Actions: AA stands for Ability Action
    //The ability currently equipped
    public InputActionReference activeAA;
    //Switch to the ability on the left of the UI
    public InputActionReference cycleAbility;

    [SerializeField] private Ability[] equippedAbilities = new Ability[3];
    public Ability activeAbility;
    bool abilityActive;
    bool abilityTimerActive;
    bool cooldownActive;
    public int abilityIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        activeAA.action.Enable();
        cycleAbility.action.Enable();
        activeAbility = equippedAbilities[abilityIndex];    
    }

    // Update is called once per frame
    void Update()
    {
        if (activeAA.action.ReadValue<float>() > 0 && !abilityActive && activeAbility.cooldownTimer <= 0)
        {
            abilityActive = true;
            activeAbility.UseAbility();
            if (activeAbility.abilityTime != 0) {
                activeAbility.abilityTimer = activeAbility.abilityTime;
                abilityTimerActive = true;
            } else
            {
                activeAbility.cooldownTimer = activeAbility.cooldownTime;
                cooldownActive = true;
            }
        }
        else if (cycleAbility.action.ReadValue<float>() > 0)
        {
            print("Cycle Left");
            if (abilityIndex == 0) abilityIndex = 2;
            else abilityIndex--;
            activeAbility = equippedAbilities[abilityIndex];
            print(activeAbility.name);
        }
        else if (cycleAbility.action.ReadValue<float>() < 0)
        {
            print("Cycle Right");
            if (abilityIndex == 2) abilityIndex = 0;
            else abilityIndex++;
            activeAbility = equippedAbilities[abilityIndex];
            print(activeAbility.name);
        }

        if(abilityActive && activeAA.action.ReadValue<float>() == 0) abilityActive = false;

        if(abilityTimerActive || cooldownActive) HandleTimers();
    }

    public void HandleTimers()
    {
        if (abilityTimerActive)
        {
            activeAbility.abilityTimer -= Time.deltaTime;
            print(activeAbility.abilityTimer);
            if (activeAbility.abilityTimer <= 0)
            {
                abilityTimerActive = false;
                activeAbility.DeactivateAbility();
                cooldownActive = true;

                activeAbility.cooldownTimer = activeAbility.cooldownTime;
            }
        }

        if (cooldownActive)
        {
            activeAbility.cooldownTimer -= Time.deltaTime;
            print(activeAbility.cooldownTimer);
            if (activeAbility.cooldownTimer <= 0) cooldownActive = false;
        }
    }
}
