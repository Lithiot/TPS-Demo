﻿using UnityEngine;
using TPSDemo.Core;
using Bolt;

namespace TPSDemo.Characters 
{
    public enum PlayerStates
    {
        Relaxed = 0, Sprinting, Aiming, Count
    }
    public enum PlayerEvents
    {
        Relax = 0, StartSprint, AimDownSights, Count
    }

    public class PlayerCharacter : Entity
    {
        [Header("Control Stats")]
        public float turnSmoothTime = 0.1f;
        public float sprintMultiplier = 2.0f;

        [Header("Resources")]
        public PlayerStates currentState = PlayerStates.Relaxed;
        public Transform Camera;

        [Header("Cinemachine Cameras")]
        public GameObject lookAtPoint;
        public GameObject movementCamera;
        public GameObject aimingCamera;

        private InputDetector inputDetector;
        private CharacterController controller;
        private AnimationsController animController;
        private WeaponController weaponController;
        private float turnSmoothvelocity;

        private void Awake()
        {
            inputDetector = GetComponent<InputDetector>();
            controller = GetComponent<CharacterController>();
            animController = GetComponent<AnimationsController>();
            weaponController = GetComponent<WeaponController>();
        }

        private void Start()
        {
            FSM = new FiniteStateMachine((int)PlayerStates.Count , (int)PlayerEvents.Count);

            // From Relaxed
            FSM.RegisterTransition((int)PlayerStates.Relaxed , (int)PlayerStates.Sprinting , (int)PlayerEvents.StartSprint);
            FSM.RegisterTransition((int)PlayerStates.Relaxed , (int)PlayerStates.Aiming , (int)PlayerEvents.AimDownSights);
            // From Sprinting
            FSM.RegisterTransition((int)PlayerStates.Sprinting , (int)PlayerStates.Relaxed , (int)PlayerEvents.Relax);
            // From Aiming
            FSM.RegisterTransition((int)PlayerStates.Aiming , (int)PlayerStates.Relaxed , (int)PlayerEvents.Relax);

            FSM.onStateChanged -= SwitchCamera;
            FSM.onStateChanged += SwitchCamera;
        }

        private void Update()
        {
            FrameInputs inputs = inputDetector.DetectInputs();

            switch (currentState)
            {
                case PlayerStates.Relaxed:
                    RelaxedUpdate(inputs);
                    break;
                case PlayerStates.Sprinting:
                    SprintingUpdate(inputs);
                    break;
                case PlayerStates.Aiming:
                    AimingUpdate(inputs);
                    break;
            }
        }

        private void RelaxedUpdate(FrameInputs inputs) 
        {
            Movement(inputs);

            // Transitions
            if (inputs.sprint)
                currentState = (PlayerStates)FSM.CheckTransition((int)currentState , (int)PlayerEvents.StartSprint);

            if (inputs.aimDownSights == 1)
                currentState = (PlayerStates)FSM.CheckTransition((int)currentState , (int)PlayerEvents.AimDownSights);
        }

        private void SprintingUpdate(FrameInputs inputs) 
        {
            Movement(inputs , sprintMultiplier);

            // Future Wall running

            if(!inputs.sprint)
                currentState = (PlayerStates)FSM.CheckTransition((int)currentState , (int)PlayerEvents.Relax);
        }

        private void AimingUpdate(FrameInputs inputs) 
        {
            Movement(inputs , 1.0f , false);

            // Fire
            if (inputs.fire != 0.0f) weaponController.TriggerFire();
            // Reload
            if (inputs.reload) weaponController.Reload();

            if (inputs.aimDownSights != 1.0f) 
                currentState = (PlayerStates)FSM.CheckTransition((int)currentState , (int)PlayerEvents.Relax);
        }

        private void Movement(FrameInputs inputs, float speedMultiplier = 1.0f, bool freeLook = true) 
        {
            Vector3 direction = new Vector3(inputs.horizontal, 0.0f, inputs.vertical).normalized;

            float mySpeed = speed * speedMultiplier;
            float targetAngle = 0.0f;

            if (direction.magnitude >= 0.1f || !freeLook) 
            {
                targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(freeLook ? transform.eulerAngles.y : Camera.eulerAngles.y, targetAngle, ref turnSmoothvelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f , angle , 0.0f);

                if (direction.magnitude >= 0.1f) 
                {
                    Vector3 moveDir = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                    controller.Move(moveDir * mySpeed * Time.deltaTime);
                }
            }
        }

        private void SwitchCamera(int newState) 
        {
            bool isAiming = (PlayerStates)newState == PlayerStates.Aiming ? true : false;

            movementCamera.SetActive(!isAiming);
            aimingCamera.SetActive(isAiming);

            if (aimingCamera.activeInHierarchy)
                lookAtPoint.transform.parent = weaponController.firePosition;
            else
                lookAtPoint.transform.parent = gameObject.transform;
        }
    }
}