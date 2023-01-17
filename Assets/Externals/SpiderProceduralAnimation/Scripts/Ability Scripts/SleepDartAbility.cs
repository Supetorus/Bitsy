using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepDartAbility : Ability
{
    public override void DeactivateAbility()
    {
        print("Deactive");
    }

    public override void UseAbility()
    {
        print("Sleep");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
