using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
	public InventoryManager inventory;

	[SerializeField] private Transform _interactionPoint;
	[SerializeField] private float _interactionPointRadius = 0.5f;
	[SerializeField] private LayerMask _interactableMask;
	[SerializeField] private int _numFound;

	private readonly Collider[] _colliders = new Collider[3];

	private void Update()
	{

	}

	private void FixedUpdate()
	{
		_numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
	}

	public void OnInteract(InputValue inputValue)
	{
		if (_numFound > 0)
		{
			Debug.Log(_numFound);
			var interactable = _colliders[0].GetComponent<IInteractable>();

			if (interactable != null)
			{
				interactable.Interact(this);
			}
		}
	}
}
