using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakAbility : Ability
{
    [SerializeField] List<Material> materials;

    SpiderController controller;
    SkinnedMeshRenderer render;
    bool isVisible = true;

    public void Start()
    {
        abCon = GetComponent<AbilityController>();
        controller = GetComponent<SpiderController>();
        render = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void Cloak()
    {
        render.material = materials[1];
        controller.isVisible = false;
        isVisible = false;
    }

    public void Uncloak()
    {
        render.material = materials[0];
        controller.isVisible = true;
        isVisible = true;
    }

    public override void UseAbility()
    {
        if (isVisible)
        {
            Cloak();
        }
        else if (!isVisible)
        {
            Uncloak();
        }
    }

    public override void DeactivateAbility()
    {
        Uncloak();

    }
}
