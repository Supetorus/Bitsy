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

    public InputActionReference interact;

    private readonly Collider[] _colliders = new Collider[3];

    private void Start()
    {
        interact.action.Enable();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0)
        {
            feedNotification.ExpandNotification();
            interactPromptLabel.text = feedNotification.notificationText;
            if (interact.action.WasPressedThisFrame())
            {
                //Debug.Log("SOMETHING WAS HIT");
                feedNotification.minimizeAfter = 1;
                var interactable = _colliders[0].GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
