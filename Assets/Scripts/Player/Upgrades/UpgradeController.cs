using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeController : MonoBehaviour
{
	[Header("Input Actions")]
	//Input Actions
	public InputActionReference lunge;
	public InputActionReference webZip;

	//MovementController
	[HideInInspector] public MovementController controller;

	[Header("WebZip Settings")]
	[SerializeField] Sprite WZ_Sprite;
	//WebZip Variables WZ_ = WebZip
	private float WZ_MaxDist = 6.5f;
	private float WZ_Cooldown = 3.75f;
	private float WZ_Timer;
	private bool WZ_OnCooldown;
	[SerializeField] LayerMask WZ_LayerMask;

	[Header("Visuals")]
	[SerializeField] LineRenderer lineRenderer;

	[SerializeField] Sprite L_Sprite;
	//Lunge Variables L_ = Lunge
	private float L_Cooldown = 1.5f;
	private float L_Timer;
	private bool L_OnCooldown;

	private List<Sprite> upgradeSprites = new List<Sprite>();
	private float[] cooldownContainer = new float[2];
	private float[] timerContainer = new float[2];
	private bool[] onCooldownContainer = new bool[2];

	//Upgrade Variables
	private const int SB_REGULAR_DURATION = 1;
	private const int SB_UPGRADED_DURATION = 2;

	private const int C_REGULAR_DURATION = 1;
	private const int C_UPGRADED_DURATION = 2;

	private const int EMP_REGULAR_RADIUS = 1;
	private const int EMP_UPGRADED_RADIUS = 2;

	private const int EMP_REGULAR_DURATION = 1;
	private const int EMP_UPGRADED_DURATION = 2;

	private StateData sd;
	RaycastHit zipPoint;

	[SerializeField] Image webzipHUD;
	[SerializeField] Image lungeHUD;
	[SerializeField] GameObject webzipCD;
	[SerializeField] GameObject lungeCD;

	void Start()
	{
		controller = GetComponent<MovementController>();
		sd = GetComponent<StateData>();
		WZ_LayerMask = 3;

		//Setting Timers
		WZ_Timer = WZ_Cooldown;
		L_Timer = L_Cooldown;
		lineRenderer.enabled = false;

		upgradeSprites.Add(WZ_Sprite);
		upgradeSprites.Add(L_Sprite);

		cooldownContainer[0] = WZ_Cooldown;
		cooldownContainer[1] = L_Cooldown;

		timerContainer[0] = WZ_Timer;
		timerContainer[1] = L_Timer;

		onCooldownContainer[0] = WZ_OnCooldown;
		onCooldownContainer[1] = L_OnCooldown;
	}

	public void Update()
	{
		if (lunge.action.ReadValue<float>() > 0 && !L_OnCooldown)
		{
			Lunge();
		}
		else if (webZip.action.IsPressed() && !WZ_OnCooldown)
		{
			DrawZipLine();
		}
		else if (webZip.action.WasReleasedThisFrame() && !WZ_OnCooldown)
		{
			WebZip();
		}
		HandleCooldowns();

		cooldownContainer[0] = WZ_Cooldown;
		cooldownContainer[1] = L_Cooldown;

		timerContainer[0] = WZ_Timer;
		timerContainer[1] = L_Timer;

		onCooldownContainer[0] = WZ_OnCooldown;
		onCooldownContainer[1] = L_OnCooldown;

		DisplayUpgradeCooldowns();
	}

	public void HandleCooldowns()
	{
		if (L_OnCooldown)
		{
			if (L_Timer >= 0) L_Timer -= Time.deltaTime;
			else
			{
				L_OnCooldown = false;
				L_Timer = L_Cooldown;
			}
		}
		if (WZ_OnCooldown)
		{
			if (WZ_Timer >= 0) WZ_Timer -= Time.deltaTime;
			else
			{
				WZ_OnCooldown = false;
				WZ_Timer = WZ_Cooldown;
			}
		}
	}

	private void DrawZipLine()
	{
		if (Physics.Raycast(Player.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out RaycastHit cameraHit, float.PositiveInfinity, GetComponent<StateData>().walkableLayers) &&
			Physics.Raycast(transform.position, cameraHit.point - transform.position, out RaycastHit hit, WZ_MaxDist, sd.walkableLayers))
		{
			zipPoint = hit;
			lineRenderer.enabled = true;
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, hit.point);
		}
		else
		{
			lineRenderer.enabled = false;
		}
	}

	public void WebZip()
	{
		lineRenderer.enabled = false;
		if (zipPoint.collider != null)
		{
			WZ_OnCooldown = true;
			controller.zipState.attachedObject = zipPoint;
			controller.CurrentMovementState = controller.zipState;
		}
	}

	public void Lunge()
	{
		Vector3 lungeDir;
		var camera = GetComponent<StateData>().camera.GetComponent<ThirdPersonCameraController>();
		lungeDir = camera.transform.forward;
		L_OnCooldown = true;
		LungeState lunge = GetComponent<LungeState>();
		lunge.lungeDirection = lungeDir;
		controller.CurrentMovementState = lunge;
	}

	public void EnableUpgrade(string upgrade)
	{
		if (PlayerPrefs.HasKey(upgrade))
		{
			PlayerPrefs.SetString(upgrade, "True");
			switch (upgrade)
			{
				case "Lunge":
					lunge.action.Enable();
					break;
				case "WebZip":
					webZip.action.Enable();
					break;
				case "SB_DU":
					PlayerPrefs.SetInt("SB_DURATION", SB_UPGRADED_DURATION);
					break;
				case "EMP_DU":
					PlayerPrefs.SetInt("EMP_DURATION", EMP_UPGRADED_DURATION);
					break;
				case "EMP_RU":
					PlayerPrefs.SetInt("EMP_RADIUS", EMP_UPGRADED_RADIUS);
					break;
				case "TD_U":
					PlayerPrefs.SetString("TD_PENETRATION", "True");
					break;
				case "C_DU":
					PlayerPrefs.SetInt("C_DURATION", C_UPGRADED_DURATION);
					break;
			}
		}
	}

	public void ChangeAmmoMult(AmmoUpgrade.AMMO_UPGRADE ammoType, int newMult)
	{
		if (PlayerPrefs.HasKey(ammoType.ToString()))
		{
			PlayerPrefs.SetInt(ammoType.ToString(), newMult);
		}
		switch (ammoType)
		{
			case AmmoUpgrade.AMMO_UPGRADE.SB_AM:
				gameObject.TryGetComponent<SmokeBombAbility>(out SmokeBombAbility sba);
				sba.UpdateAmmo();
				break;
			case AmmoUpgrade.AMMO_UPGRADE.EMP_AM:
				gameObject.TryGetComponent<EMPAbility>(out EMPAbility empa);
				empa.UpdateAmmo();
				break;
			case AmmoUpgrade.AMMO_UPGRADE.TD_AM:
				gameObject.TryGetComponent<TrojanDartAbility>(out TrojanDartAbility tda);
				tda.UpdateAmmo();
				break;
		}
	}

	public void DisableUpgrade(string upgrade)
	{
		if (PlayerPrefs.HasKey(upgrade))
		{
			PlayerPrefs.SetString(upgrade, "False");
			switch (upgrade)
			{
				case "Lunge":
					lunge.action.Disable();
					break;
				case "WebZip":
					webZip.action.Disable();
					break;
				case "SB_U":
					PlayerPrefs.SetInt("SB_DURATION", SB_REGULAR_DURATION);
					break;
				case "EMP_DU":
					PlayerPrefs.SetInt("EMP_DURATION", EMP_REGULAR_DURATION);
					break;
				case "EMP_RU":
					PlayerPrefs.SetInt("EMP_RADIUS", EMP_REGULAR_RADIUS);
					break;
				case "TD_U":
					PlayerPrefs.SetString("TD_PENETRATION", "False");
					break;
				case "C_U":
					PlayerPrefs.SetInt("C_DURATION", C_REGULAR_DURATION);
					break;
			}
		}
	}

	private void DisplayUpgradeCooldowns(RectTransform.Axis axis = RectTransform.Axis.Vertical)
	{
		webzipCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CalcRectAxisSize(0, 2.5f, 0.0f));
		lungeCD.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CalcRectAxisSize(1, 2.5f, 0.0f));
	}

	private float CalcRectAxisSize(int index, float max, float min)
	{
		return onCooldownContainer[index] ? (timerContainer[index] / cooldownContainer[index]) * max : min;
	}
}
