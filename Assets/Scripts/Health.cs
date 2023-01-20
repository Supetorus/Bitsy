using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
	// Values
	[Tooltip("Health will never go above this value. If left at 0 it will be set to float.MaxValue")]
	public float maxValue = 0;
	[Tooltip("The starting amount of health. If left at 0 it will be set to Max Health.")]
	public float startValue;
	// Events
	[Tooltip("This event is called when health value reaches 0.")]
	public UnityEvent onDeath;
	[Tooltip("This event is called when health value decreases.")]
	public UnityEvent onTakeDamage;
	[Tooltip("This event is called when health value increases.")]
	public UnityEvent onHeal;
	[Tooltip("This event is called when the Max Health is reached")]
	public UnityEvent onMaxHealth;
	[Tooltip("This event is called when the health value changes.")]
	public UnityEvent onValueChanged;
	// Display
	[Tooltip("If assigned, this slider will update as the health changes.")]
	public Slider displaySlider;

	[SerializeField, Tooltip("For debug display only. Do not modify.")]
	private float health;

	public float Value
	{
		get { return health; }
		private set
		{
			value = Mathf.Min(maxValue, Mathf.Max(value, 0));
			if (value == 0 && health != 0) onDeath.Invoke();
			if (value == maxValue && health != maxValue) onMaxHealth.Invoke();
			if (value != health)
			{
				onValueChanged.Invoke();
				if (displaySlider != null) UpdateDisplay(value);
			}
			health = Mathf.Min(maxValue, Mathf.Max(value, 0));
		}
	}

	private void Start()
	{
		if (maxValue == 0) maxValue = float.MaxValue;
		if (startValue == 0) startValue = maxValue;
		health = startValue;
		if (displaySlider != null)
		{
			displaySlider.maxValue = maxValue;
			displaySlider.minValue = 0;
			displaySlider.value = health;
		}
	}

	public void InstantKill()
	{
		Value = 0;
	}

	public void TakeDamage(float amount)
	{
		Value -= amount;
		onTakeDamage.Invoke();
	}

	public void Heal(float amount)
	{
		Value += amount;
		onHeal.Invoke();
	}

	public void SetHealth (float newValue)
	{
		Value = newValue;
	}

	private void UpdateDisplay(float newValue)
	{
		displaySlider.value = newValue;
	}
}
