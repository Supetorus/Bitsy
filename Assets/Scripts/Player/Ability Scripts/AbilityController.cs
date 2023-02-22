using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityController : MonoBehaviour
{
	//Ability Actions: AA stands for Ability Action
	//The ability currently equipped
	[Header("Input Settings")]
    public InputActionReference activeAA;

    //Switch to the ability on the left of the UI
    public InputActionReference cycleAbility;

	[Header("Cloak Settings")]
	//Cloak Timers
	[SerializeField] float C_Cooldown;
	[HideInInspector] float C_Timer;
	[HideInInspector] bool C_OnCooldown;

	[SerializeField] float C_Duration;
	[HideInInspector] float C_DurationTimer;
	[HideInInspector] bool C_IsActive;

	[Header("Smoke Bomb Settings")]
	//Smoke Bomb Timers
	[SerializeField] float SB_Cooldown;
	[HideInInspector] float SB_Timer;
	[HideInInspector] bool SB_OnCooldown;

	[Header("EMP Settings")]
	//EMP TIMERS
	[SerializeField] float EMP_Cooldown;
	[HideInInspector] float EMP_Timer;
	[HideInInspector] bool EMP_OnCooldown;

	[Header("Trojan Dart Settings")]
	//TROJAN DART TIMERS
	[SerializeField] float TD_Cooldown;
	[HideInInspector] float TD_Timer;
	[HideInInspector] bool TD_OnCooldown;

    private int abilityIndex = 0;
    private Ability[] equippedAbilities = new Ability[4];
    public Ability activeAbility;
    [HideInInspector] public bool isVisible = true;


	// Start is called before the first frame update
	void Start()
    {
		activeAA.action.Enable();
        cycleAbility.action.Enable();

		equippedAbilities[0] = GetComponent<CloakAbility>();
		equippedAbilities[1] = GetComponent<EMPAbility>();
		equippedAbilities[2] = GetComponent<SmokeBombAbility>();
		equippedAbilities[3] = GetComponent<TrojanDartAbility>();

		C_Timer = C_Cooldown;
		C_DurationTimer = C_Duration;
		SB_Timer = SB_Cooldown;
		TD_Timer = TD_Cooldown;
		EMP_Timer = EMP_Cooldown;

		activeAbility = equippedAbilities[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (activeAA.action.ReadValue<float>() > 0 && !C_OnCooldown && !SB_OnCooldown && !TD_OnCooldown && !EMP_OnCooldown && !C_IsActive)
        {
			switch (abilityIndex)
			{
				case 0:
					if (!C_OnCooldown)
					{
						activeAbility.UseAbility();
						C_IsActive = true;
					} 
					else
					{
						activeAbility.DeactivateAbility();
						C_IsActive = false;
						C_OnCooldown = true;
					}
					break;
				case 1:
					activeAbility.UseAbility();
					EMP_OnCooldown = true;
					break;
				case 2:
					activeAbility.UseAbility();
					SB_OnCooldown = true;
					break;
				case 3:
					activeAbility.UseAbility();
					TD_OnCooldown = true;
					break;
			}
        }
        else if (cycleAbility.action.ReadValue<float>() > 0)
        {
            if (abilityIndex == 0) abilityIndex = 3;
            else if (equippedAbilities[abilityIndex - 1] != null) abilityIndex--;
            activeAbility = equippedAbilities[abilityIndex];
        }
        else if (cycleAbility.action.ReadValue<float>() < 0)
        {
            if (abilityIndex == 3) if (equippedAbilities[0] != null) abilityIndex = 0;
            else if (equippedAbilities[abilityIndex + 1] != null) abilityIndex++;
            activeAbility = equippedAbilities[abilityIndex];
        }
		HandleTimers();
    }

    public void HandleTimers()
    {
		if (C_OnCooldown)
		{
			if (C_Timer >= 0) C_Timer -= Time.deltaTime;
			else
			{
				C_OnCooldown = false;
				C_Timer = C_Cooldown;
			}
		}
		else if(C_IsActive)
		{
			if (C_DurationTimer >= 0) C_DurationTimer -= Time.deltaTime;
			else
			{
				gameObject.GetComponent<CloakAbility>().DeactivateAbility();
				C_DurationTimer = C_Duration;
				C_OnCooldown = true;
				C_IsActive = false;
			}
		}
		if (SB_OnCooldown)
		{
			if (SB_Timer >= 0) SB_Timer -= Time.deltaTime;
			else
			{
				SB_OnCooldown = false;
				SB_Timer = SB_Cooldown;
			}
		}
		if (EMP_OnCooldown)
		{
			if (EMP_Timer >= 0) EMP_Timer -= Time.deltaTime;
			else
			{
				EMP_OnCooldown = false;
				EMP_Timer = EMP_Cooldown;
			}
		}
		if (TD_OnCooldown)
		{
			if (TD_Timer >= 0) TD_Timer -= Time.deltaTime;
			else
			{
				TD_OnCooldown = false;
				TD_Timer = TD_Cooldown;
			}
		}
	}
}
