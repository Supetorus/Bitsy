using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BuildingMakerToolset.Demo {
    /// <summary>
    /// Demo script for opening doors.
    /// </summary>
    public class Door : MonoBehaviour
    {
        float triggerDist = 2.2f;
        float openingSpeed = 1;

        bool triggered = false;
        bool reseting = false;
        Transform triggerer;
        Quaternion initialRotation;
        Vector3 initialRotationEuler;
        float openValue = 0;

        bool approachFromFront = false;
        private void Start()
        {
            PlayerMovement user = Object.FindObjectOfType( typeof( PlayerMovement ) ) as PlayerMovement;
            if (user == null)
            {
                this.enabled = false;
                return;
            }
            triggerer = user.transform;
            initialRotation = transform.localRotation;
            initialRotationEuler = transform.localRotation.eulerAngles;
        }

        private void Update()
        {
            if (reseting)
            {
                Reset_Loop();
                return;
            }
            if (triggered)
            {
                PostTrigger_Loop();
                return;
            }
          
            PreTrigger_Loop();
           

        }

        void PreTrigger_Loop()
        {
            float sqrDist = Vector3.SqrMagnitude( triggerer.position - transform.position );
            if (sqrDist < triggerDist * triggerDist)
            {
                if(openValue==0)
                    TriggerSomething();
            }
            else if(openValue == 1)
            {
                reseting = true;
            }
        }


        void PostTrigger_Loop()
        {
            openValue += Time.deltaTime * openingSpeed;
            if (openValue >= 1)
            {
                openValue = 1;
                triggered = false;
            }
            transform.localEulerAngles = Vector3.Lerp( initialRotationEuler, new Vector3( initialRotationEuler.x, initialRotationEuler.y + (approachFromFront ? 100 : -100), initialRotationEuler.z ), openValue );
        }

        void Reset_Loop()
        {
            openValue -= Time.deltaTime * openingSpeed;
            if (openValue <= 0)
            {
                openValue = 0;
                reseting = false;
            }
            transform.localEulerAngles = Vector3.Lerp( initialRotationEuler, new Vector3( initialRotationEuler.x, initialRotationEuler.y + (approachFromFront ? 100 : -100), initialRotationEuler.z ), openValue );
        }

        void TriggerSomething()
        {
            triggered = true;
            approachFromFront = transform.InverseTransformDirection( triggerer.position ).z > 0;
        }
    }
}
