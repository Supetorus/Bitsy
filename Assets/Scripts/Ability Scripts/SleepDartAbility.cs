using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepDartAbility : Ability
{
    [SerializeField] GameObject sleepDart;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] float dartSpeed;

    public override void DeactivateAbility()
    {

    }

    public override void UseAbility()
    {
        GameObject dart = Instantiate(sleepDart, projectileSpawn.position, projectileSpawn.rotation);
        dart.GetComponent<Rigidbody>().AddForce(dart.transform.forward * dartSpeed, ForceMode.Acceleration);
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
