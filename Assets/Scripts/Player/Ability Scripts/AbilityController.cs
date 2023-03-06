using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class AbilityController : MonoBehaviour
{
	//Ability Actions: AA stands for Ability Action
	//The ability currently equipped
	[Header("Input Settings")]
	public InputActionReference activeAA;

	//Switch to the ability on the left of the UI
	public InputActionReference cycleAbility;

	[Header("Cloak Settings")]
	[SerializeField] Sprite C_Sprite;
	//Cloak Timers
	[SerializeField] float C_Cooldown;
	[HideInInspector] float C_Timer;
	[HideInInspector] bool C_OnCooldown;

	[SerializeField] float C_Duration;
	[HideInInspector] float C_DurationTimer;
	[HideInInspector] bool C_IsActive;

	[Header("Smoke Bomb Settings")]
	[SerializeField] Sprite SB_Sprite;
	//Smoke Bomb Timers
	[SerializeField] float SB_Cooldown;
	[HideInInspector] float SB_Timer;
	[HideInInspector] bool SB_OnCooldown;

	[Header("EMP Settings")]
	[SerializeField] Sprite EMP_Sprite;
	//EMP TIMERS
	[SerializeField] float EMP_Cooldown;
	[HideInInspector] float EMP_Timer;
	[HideInInspector] bool EMP_OnCooldown;

	[Header("Trojan Dart Settings")]
	[SerializeField] Sprite TD_Sprite;
	//TROJAN DART TIMERS
	[SerializeField] float TD_Cooldown;
	[HideInInspector] float TD_Timer;
	[HideInInspector] bool TD_OnCooldown;

	private int abilityIndex = 0;
	private Ability[] equippedAbilities = new Ability[4];
	private List<Sprite> abilitySprites = new List<Sprite>();
	private float[] cooldownContainer = new float[4];
	private float[] timerContainer = new float[4];
	private bool[] onCooldownContainer = new bool[4];
	public Ability activeAbility;

	[Header("UI Objects")]
	[SerializeField] Image currentAbilityHUD;
	[SerializeField] Image prevAbilityHUD;
	[SerializeField] Image nextAbilityHUD;
	[SerializeField] GameObject currentAbilityCD;
	[SerializeField] GameObject prevAbilityCD;
	[SerializeField] GameObject nextAbilityCD;
	[SerializeField] TextMeshProUGUI currentAmmo;
	[SerializeField] TextMeshProUGUI prevAmmo;
	[SerializeField] TextMeshProUGUI nextAmmo;
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

		abilitySprites.Add(C_Sprite);
		abilitySprites.Add(EMP_Sprite);
		abilitySprites.Add(SB_Sprite);
		abilitySprites.Add(TD_Sprite);

		C_Timer = C_Cooldown;
		C_DurationTimer = C_Duration;
		SB_Timer = SB_Cooldown;
		TD_Timer = TD_Cooldown;
		EMP_Timer = EMP_Cooldown;

		cooldownContainer[0] = C_Cooldown;
		cooldownContainer[1] = EMP_Cooldown;
		cooldownContainer[2] = SB_Cooldown;
		cooldownContainer[3] = TD_Cooldown;

		timerContainer[0] = C_Timer;
		timerContainer[1] = EMP_Timer;
		timerContainer[2] = SB_Timer;
		timerContainer[3] = TD_Timer;

		onCooldownContainer[0] = C_OnCooldown;
		onCooldownContainer[1] = EMP_OnCooldown;
		onCooldownContainer[2] = SB_OnCooldown;
		onCooldownContainer[3] = TD_OnCooldown;

		activeAbility = equippedAbilities[0];
		currentAbilityHUD.sprite = abilitySprites[0];
	}

	// Update is called once per frame
	void Update()
	{
		if (activeAA.action.ReadValue<float>() > 0 && !C_OnCooldown && !SB_OnCooldown && !TD_OnCooldown && !EMP_OnCooldown && !C_IsActive)
		{
			//this can potentially replace the switch statement
			/*if (!onCooldownContainer[abilityIndex])
			{
				activeAbility.UseAbility();
				if (abilityIndex == 0) { C_IsActive = true; }
				else { onCooldownContainer[abilityIndex] = true; }
			}
			else if (abilityIndex == 0)
			{
				activeAbility.DeactivateAbility();
				C_IsActive = false;
				C_OnCooldown = true;
			}*/

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
					if (!EMP_OnCooldown)
					{
						activeAbility.UseAbility();
						EMP_OnCooldown = true;
					}
					break;
				case 2:
					if (!SB_OnCooldown)
					{
						activeAbility.UseAbility();
						SB_OnCooldown = true;
					}
					break;
				case 3:
					if (!TD_OnCooldown)
					{
						activeAbility.UseAbility();
						TD_OnCooldown = true;
					}
					break;
			}
		}
		else if (cycleAbility.action.ReadValue<float>() > 0)
		{
			//this can probably be used to simplify the integer looping
			//abilityIndex = (int)Mathf.Repeat(abilityIndex - 1, 3);

			//abilityIndex = (abilityIndex - 1) % equippedAbilities.Length;
			if (abilityIndex == 0) abilityIndex = 3;
			else if (equippedAbilities[abilityIndex - 1] != null) abilityIndex--;
		}
		else if (cycleAbility.action.ReadValue<float>() < 0)
		{
			//abilityIndex = (abilityIndex + 1) % equippedAbilities.Length;
			if (abilityIndex == 3 && equippedAbilities[0] != null) abilityIndex = 0;
			else if (equippedAbilities[abilityIndex + 1] != null) abilityIndex++;
		}

		activeAbility = equippedAbilities[abilityIndex];
		currentAbilityHUD.sprite = abilitySprites[abilityIndex];

		int prevIndex = (abilityIndex == 0) ? equippedAbilities.Length - 1 : abilityIndex - 1;
		int nextIndex = (abilityIndex == equippedAbilities.Length - 1) ? 0 : abilityIndex + 1;

		prevAbilityHUD.sprite = abilitySprites[prevIndex];
		nextAbilityHUD.sprite = abilitySprites[nextIndex];

		currentAmmo.text = (equippedAbilities[abilityIndex].currentAmmo == 0) ? "" : equippedAbilities[abilityIndex].currentAmmo.ToString();
		prevAmmo.text = (equippedAbilities[prevIndex].currentAmmo == 0) ? "" : equippedAbilities[prevIndex].currentAmmo.ToString();
		nextAmmo.text = (equippedAbilities[nextIndex].currentAmmo == 0) ? "" : equippedAbilities[nextIndex].currentAmmo.ToString();

		HandleTimers();

		cooldownContainer[0] = C_Cooldown;
		cooldownContainer[1] = EMP_Cooldown;
		cooldownContainer[2] = SB_Cooldown;
		cooldownContainer[3] = TD_Cooldown;

		timerContainer[0] = C_Timer;
		timerContainer[1] = EMP_Timer;
		timerContainer[2] = SB_Timer;
		timerContainer[3] = TD_Timer;

		onCooldownContainer[0] = C_OnCooldown;
		onCooldownContainer[1] = EMP_OnCooldown;
		onCooldownContainer[2] = SB_OnCooldown;
		onCooldownContainer[3] = TD_OnCooldown;

		DisplayAbilityCooldowns();
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
		else if (C_IsActive)
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

		/*switch (abilityIndex)
		{
			case 0:
				currentAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (C_OnCooldown) ? (C_Timer / C_Cooldown) * 2.5f : 0);
				prevAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (TD_OnCooldown) ? (TD_Timer / TD_Cooldown) * 2.5f : 0);
				nextAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (EMP_OnCooldown) ? (EMP_Timer / EMP_Cooldown) * 2.5f : 0);
				break;
			case 1:
				currentAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (EMP_OnCooldown) ? (EMP_Timer / EMP_Cooldown) * 2.5f : 0);
				prevAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (C_OnCooldown) ? (C_Timer / C_Cooldown) * 2.5f : 0);
				nextAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (SB_OnCooldown) ? (SB_Timer / SB_Cooldown) * 2.5f : 0);
				break;
			case 2:
				currentAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (SB_OnCooldown) ? (SB_Timer / SB_Cooldown) * 2.5f : 0);
				prevAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (EMP_OnCooldown) ? (EMP_Timer / EMP_Cooldown) * 2.5f : 0);
				nextAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (TD_OnCooldown) ? (TD_Timer / TD_Cooldown) * 2.5f : 0);
				break;
			case 3:
				currentAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (TD_OnCooldown) ? (TD_Timer / TD_Cooldown) * 2.5f : 0);
				prevAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (SB_OnCooldown) ? (SB_Timer / SB_Cooldown) * 2.5f : 0);
				nextAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (C_OnCooldown) ? (C_Timer / C_Cooldown) * 2.5f : 0);
				break;
			default:
				break;
		}*/

	}

	private void DisplayAbilityCooldowns(RectTransform.Axis axis = RectTransform.Axis.Vertical)
	{
		currentAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CalcRectAxisSize(abilityIndex, 2.5f, 0.0f));
		prevAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CalcRectAxisSize((abilityIndex == 0) ? 3 : abilityIndex - 1, 2.5f, 0.0f));
		nextAbilityCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CalcRectAxisSize((abilityIndex == 3) ? 0 : abilityIndex + 1, 2.5f, 0.0f));
	}

	private float CalcRectAxisSize(int index, float max, float min)
	{
		if (index != 0 && equippedAbilities[index].currentAmmo == 0)
		{
			return max;
		}
		return onCooldownContainer[index] ? (timerContainer[index] / cooldownContainer[index]) * max : min;

	}
}
