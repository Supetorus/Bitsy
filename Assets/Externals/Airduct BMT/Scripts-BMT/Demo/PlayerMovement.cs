#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BuildingMakerToolset.Demo
{
    /// <summary>
    /// Demo script for player movement
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {

        public CharacterController controller;
        public Transform cameraTransform;
        public AudioSource footstepSource;

        public float speed = 12f;
        public float gravity = -10f;
        public float jumpHeight = 2f;

        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;

        Vector3 lastPosition;
        Vector3 targetVelocity;
        bool isGrounded;
        Collider[] hitColliders;
        public int numOfGroundColliders;
        float distAfterLastFootstep;

        public enum CurrentTask { none, fly, ladder };
        public CurrentTask currentTask;

        private void OnEnable()
        {
            hitColliders = new Collider[8];
            lastPosition = transform.position;
        }

        [Serializable]
        public class FootStepAudio
        {
            public string name;
            public AudioClip[] audioClips;
            public bool IsValid()
            {
                if (audioClips == null || audioClips.Length == 0)
                    return false;
                return
                    true;
            }
            public AudioClip GetRandomClip()
            {
                if (!IsValid())
                    return null;
                return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
            }
        }

        FootStepAudio curFootstepAudio;
        public FootStepAudio[] footStepAudios;
        public FootStepAudio DefaultFootstepAudio;

#if ENABLE_INPUT_SYSTEM
    InputAction movement;
    InputAction jump;

    void Start()
    {
        movement = new InputAction("PlayerMovement", binding: "<Gamepad>/leftStick");
        movement.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");
        
        jump = new InputAction("PlayerJump", binding: "<Gamepad>/a");
        jump.AddBinding("<Keyboard>/space");

        movement.Enable();
        jump.Enable();
    }
#endif

        // Update is called once per frame
        void Update()
        {
            float inpX = 0;
            float inpY = 0;
            bool jumpPressed = false;

#if ENABLE_INPUT_SYSTEM
        var delta = movement.ReadValue<Vector2>();
        float x = delta.x;
        float z = delta.y;
        jumpPressed = Mathf.Approximately(jump.ReadValue<float>(), 1);
#else
            inpX = Input.GetAxis( "Horizontal" );
            inpY = Input.GetAxis( "Vertical" );
            jumpPressed = Input.GetButtonDown( "Jump" );
#endif
            if (cameraTransform == null)
            {
                currentTask = CurrentTask.none;
            }
            Vector3 move = Vector3.zero;
            switch (currentTask)
            {
                case CurrentTask.none:
                    isGrounded = CheckGround();
                    move = Vector3.ClampMagnitude(transform.right * inpX + transform.forward * inpY,1)* speed;
                    targetVelocity.y += gravity * Time.deltaTime;
                    break;
                case CurrentTask.fly:
                    isGrounded = false;
                    move = Vector3.ClampMagnitude(cameraTransform.right * inpX + cameraTransform.forward * inpY, 1) * speed;
                    targetVelocity.y = move.y;
                    break;
                case CurrentTask.ladder:
                    isGrounded = false;
                    if(!CheckGround())
                        move = Vector3.ClampMagnitude(transform.right * inpX + transform.up * inpY , 1) * speed;
                    else
                        move = Vector3.ClampMagnitude(cameraTransform.right * inpX + cameraTransform.forward * inpY, 1) * speed;
                    targetVelocity.y = move.y;
                    break;
            }            
            targetVelocity.x = move.x;
            targetVelocity.z = move.z;

            if (isGrounded && targetVelocity.y < 0)
            {
                targetVelocity.y = -2f;
            }
            if (jumpPressed && isGrounded)
            {
                targetVelocity.y = Mathf.Sqrt( jumpHeight * -2f * gravity );
            }

               
            
            lastPosition = transform.position;
            controller.Move( targetVelocity * Time.deltaTime);
            Vector3 velocity = transform.position - lastPosition;

            distAfterLastFootstep += velocity.magnitude;
            if (distAfterLastFootstep > 1f)
            {
                distAfterLastFootstep = 0;
                MakeFootstep();
            }


        }
        
        void MakeFootstep()
        {
            if (footstepSource == null)
                return;

            if (currentTask == CurrentTask.none)
            {
                if (numOfGroundColliders == 0)
                    return;
                SetFootstepAudio(hitColliders[0]);
            }
          

            footstepSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            footstepSource.PlayOneShot(curFootstepAudio.GetRandomClip());
        }


        bool CheckGround()
        {
            numOfGroundColliders = Physics.OverlapSphereNonAlloc(groundCheck.position, groundDistance, hitColliders, groundMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < numOfGroundColliders; i++)
                if (hitColliders[i].gameObject == gameObject)
                    numOfGroundColliders--;
            if (numOfGroundColliders == 0)
                return false;
            return true;
        }

        public void SetFootstepAudio(ref string something)
        {
            if (something == null)
            {
                curFootstepAudio = DefaultFootstepAudio;
                return;
            }
            bool foundAudio = false;
            for (int j = 0; j < footStepAudios.Length; j++)
            {
                if (footStepAudios[j].IsValid() && something.Contains(footStepAudios[j].name))
                {
                    foundAudio = true;
                    curFootstepAudio = footStepAudios[j];
                    break;
                }
            }
            if (!foundAudio)
                curFootstepAudio = DefaultFootstepAudio;
        }
        public void SetFootstepAudio(Collider something)
        {
            if (something == null)
            {
                curFootstepAudio = DefaultFootstepAudio;
                return;
            }
            bool foundAudio = false;
            for (int j = 0; j < footStepAudios.Length; j++)
            {
                if (something.material!=null && footStepAudios[j].IsValid() && something.material.name.Contains(footStepAudios[j].name))
                {
                    foundAudio = true;
                    curFootstepAudio = footStepAudios[j];
                    break;
                }
            }
            if (!foundAudio)
                curFootstepAudio = DefaultFootstepAudio;
        }
    }
}
