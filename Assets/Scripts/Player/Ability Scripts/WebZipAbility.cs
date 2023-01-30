using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ZipState))]
public class WebZipAbility : Ability
{
    [SerializeField] float maxZipDistance;
    [SerializeField] LayerMask zippableLayers;
    [SerializeField] private bool drawGizmos = false;

    MovementController controller;

    public void Start()
    {
        controller = GetComponent<MovementController>();
    }

    public void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (drawGizmos) Debug.DrawRay(transform.position, ray.direction);
    }

    public override void DeactivateAbility()
    {
    }

    public override void UseAbility()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (drawGizmos) Debug.DrawRay(transform.position, ray.direction);
        Physics.Raycast(ray, out hit, maxZipDistance, zippableLayers);
        if (hit.collider)
        {
            print(hit.collider.name);
            ZipState zip = GetComponent<ZipState>();
            zip.attachedObject = hit;
            controller.CurrentMovementState = zip;
        }
    }
}
