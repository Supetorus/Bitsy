using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
	[SerializeField] private FeedNotification feedNotification;

	public InventoryManager inventory;

	//[SerializeField] private string interactPrompt;
	public TMP_Text interactPromptLabel;

	[SerializeField] private Transform _interactionPoint;
	[SerializeField] private float _interactionPointRadius = 0.5f;
	[SerializeField] private LayerMask _interactableMask;
	[SerializeField] private int _numFound;
	private float pressTimer = 1;

	public InputActionReference interact;

	private readonly Collider[] _colliders = new Collider[3];

	private void Start()
	{
		interact.action.Enable();
	}

	private void FixedUpdate()
	{
		_numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);
		pressTimer -= Time.fixedDeltaTime;

		if (_numFound > 0)
		{
			IInteractable interactable = _colliders[0].gameObject.GetComponent<IInteractable>();

			if (interactable != null && interactable.CanInteract)
			{
				feedNotification.notificationText = interactable.InteractPrompt;
				feedNotification.ExpandNotification();
				if (interact.action.IsPressed() && pressTimer <= 0)
				{
					pressTimer = 1;
					feedNotification.MinimizeNotification();
					interactable.Interact(this);
					if (_colliders[0].TryGetComponent(out TaskInteract task))
					{
						task.Interact();
					}
				}
			}
		}
		else
		{
			feedNotification.MinimizeNotification();
		}
	}

	private void OnDrawGizmosSelected()
	{
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawSphere(_interactionPoint.position, _interactionPointRadius);
	}
}
