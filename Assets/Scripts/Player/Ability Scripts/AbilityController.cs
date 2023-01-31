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

    bool abilityActive;
    bool abilityTimerActive;
    bool cooldownActive;
    public int abilityIndex = 0;

	public MusicManager music;
    public Ability activeAbility;
	public AudioManager audioManager;

    [HideInInspector] public bool isVisible = true;

	[SerializeField] private bool detected;
	public bool Detected { get { return detected; } set { detected = value;if(music != null) music.PlayerDetected = value; } }

	// Start is called before the first frame update
	void Start()
    {
        activeAA.action.Enable();
        cycleAbility.action.Enable();
        if (equippedAbilities[abilityIndex] != null)
        {
            activeAbility = equippedAbilities[abilityIndex];
        } 
        else
        {
            activeAbility = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (activeAA.action.ReadValue<float>() > 0 && !abilityActive && activeAbility?.cooldownTimer <= 0 )
        {
            if(activeAbility == null)
            {
                print("No Equipped Ability");
                return;
            }
            abilityActive = true;
            activeAbility.UseAbility();

			if(activeAbility.GetType() == typeof(CloakAbility))
			{
				activeAbility.abilityTime *= PlayerPrefs.GetInt("C_DURATION");
			}

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
            if (abilityIndex == 0)
            {
                if (equippedAbilities[2] != null) abilityIndex = 2;
            }
            else
            {
                if (equippedAbilities[abilityIndex - 1] != null) abilityIndex--;
            }
            activeAbility = equippedAbilities[abilityIndex];
        }
        else if (cycleAbility.action.ReadValue<float>() < 0)
        {
            if (abilityIndex == 2)
            {
                if (equippedAbilities[0] != null) abilityIndex = 0;
            }
            else
            {
                if (equippedAbilities[abilityIndex + 1] != null) abilityIndex++;
            }
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
