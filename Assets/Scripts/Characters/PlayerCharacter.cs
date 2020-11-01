using UnityEngine;
using TPSDemo.Core;
using Bolt;

namespace TPSDemo.Characters {
    public class PlayerCharacter : Entity
    {
        [Header("Control Stats")]
        public float turnSmoothTime = 0.1f;
        public float sprintMultiplier = 2.0f;

        [Header("Resources")]
        public PlayerStates currentState = PlayerStates.Relaxed;
        public Transform Camera;

        [Header("Cinemachine Cameras")]
        public GameObject movementCamera;
        public GameObject aimingCamera;

        private InputDetector inputDetector;
        private CharacterController controller;
        private float turnSmoothvelocity;

        private void Awake()
        {
            inputDetector = GetComponent<InputDetector>();
            controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            FSM = new FiniteStateMachine((int)PlayerStates.Count , (int)PlayerEvents.Count);

            // From Relaxed
            FSM.RegisterTransition((int)PlayerStates.Relaxed , (int)PlayerStates.Sprinting , (int)PlayerEvents.StartSprint);
            FSM.RegisterTransition((int)PlayerStates.Relaxed , (int)PlayerStates.Aiming , (int)PlayerEvents.AimDownSights);
            // From Sprinting
            FSM.RegisterTransition((int)PlayerStates.Sprinting , (int)PlayerStates.Relaxed , (int)PlayerEvents.Relax);
            FSM.RegisterTransition((int)PlayerStates.Sprinting , (int)PlayerStates.Aiming , (int)PlayerEvents.AimDownSights);
            // From Aiming
            FSM.RegisterTransition((int)PlayerStates.Aiming , (int)PlayerStates.Relaxed , (int)PlayerEvents.Relax);
            FSM.RegisterTransition((int)PlayerStates.Aiming , (int)PlayerStates.Sprinting , (int)PlayerStates.Sprinting);

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

            if(!inputs.sprint)
                currentState = (PlayerStates)FSM.CheckTransition((int)currentState , (int)PlayerEvents.Relax);

            if (inputs.aimDownSights == 1)
                currentState = (PlayerStates)FSM.CheckTransition((int)currentState , (int)PlayerEvents.AimDownSights);
        }

        private void AimingUpdate(FrameInputs inputs) 
        {
            Movement(inputs , 1.0f , false);

            if (inputs.aimDownSights != 1.0f) 
            {
                currentState = (PlayerStates)FSM.CheckTransition((int)currentState , (int)PlayerEvents.Relax);
            }

            if (inputs.sprint)
                currentState = (PlayerStates)FSM.CheckTransition((int)currentState , (int)PlayerEvents.StartSprint);
        }

        private void Movement(FrameInputs inputs, float speedMultiplier = 1.0f, bool freeLook = true) 
        {
            Vector3 direction = new Vector3(inputs.horizontal, 0.0f, inputs.vertical).normalized;

            float mySpeed = Speed * speedMultiplier;
            float targetAngle = 0.0f;

            if (direction.magnitude >= 0.1f) 
            {
                targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(freeLook ? transform.eulerAngles.y : Camera.eulerAngles.y, targetAngle, ref turnSmoothvelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f , angle , 0.0f);

                Vector3 moveDir = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                
                controller.Move(moveDir * mySpeed * Time.deltaTime);
            }
        }

        private void SwitchCamera(int newState) 
        {
            bool isAiming = (PlayerStates)newState == PlayerStates.Aiming ? true : false;

            movementCamera.SetActive(!isAiming);
            aimingCamera.SetActive(isAiming);
        }
    }
}