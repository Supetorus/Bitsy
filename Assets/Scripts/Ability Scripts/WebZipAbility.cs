using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ZipState))]
public class WebZipAbility : Ability
{
    [SerializeField] float maxZipDistance;
    [SerializeField] LayerMask myLayerMask;

    MovementController controller;

    public void Start()
    {
        controller = GetComponent<MovementController>();
    }

    public void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        Debug.DrawRay(transform.position, ray.direction);
    }

    public override void DeactivateAbility()
    {
    }

    public override void UseAbility()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        Debug.DrawRay(transform.position, ray.direction);
        Physics.Raycast(ray, out hit, maxZipDistance, myLayerMask);
        if (hit.collider)
        {
            print(hit.collider.name);
            ZipState zip = GetComponent<ZipState>();
            zip.attachedObject = hit;
            controller.CurrentMovementState = zip;
        }
    }
}
