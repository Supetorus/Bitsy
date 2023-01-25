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
    public Transform spiderCenter;

    [SerializeField] private Ability[] equippedAbilities = new Ability[3];
    public Ability activeAbility;
    bool abilityActive;
    bool abilityTimerActive;
    bool cooldownActive;
    public int abilityIndex = 1;
    [HideInInspector] public bool isVisible = true;

	public AudioManager audioManager;
	[SerializeField] private bool detected;
	public bool Detected { get { return detected; } set { detected = value;if(audioManager != null) audioManager.PlayerDetected = value; } }

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
            if (abilityIndex == 0) abilityIndex = 2;
            else abilityIndex--;
            activeAbility = equippedAbilities[abilityIndex];
        }
        else if (cycleAbility.action.ReadValue<float>() < 0)
        {
            if (abilityIndex == 2) abilityIndex = 0;
            else abilityIndex++;
            activeAbility = equippedAbilities[abilityIndex];
        }

        if(abilityActive && activeAA.action.ReadValue<float>() == 0) abilityActive = false;

        if(abilityTimerActive || cooldownActive) HandleTimers();
    }

    public void HandleTimers()
    {
        if (abilityTimerActive)
        {
            activeAbility.abilityTimer -= Time.deltaTime;
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
            if (activeAbility.cooldownTimer <= 0) cooldownActive = false;
        }
    }
}
