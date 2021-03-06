﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSDemo.Core
{
    public struct FrameInputs 
    {
        public float vertical;
        public float horizontal;
        public bool sprint;
        public float aimDownSights;
        public float fire;

        public FrameInputs(float vertical , float horizontal, bool sprint, float aimDownSights, float fire) 
        {
            this.vertical = vertical;
            this.horizontal = horizontal;
            this.sprint = sprint;
            this.aimDownSights = aimDownSights;
            this.fire = fire;
        }
    }

    public class InputDetector : MonoBehaviour
    {
        public string verticalAxis;
        public string horizontalAxis;
        public string fireAxis;
        public string aimAxis;
        public KeyCode sprintKey;

        public FrameInputs DetectInputs() 
        {
            return new FrameInputs
                (
                    Input.GetAxis(verticalAxis),
                    Input.GetAxis(horizontalAxis),
                    Input.GetKey(sprintKey),
                    Input.GetAxis(aimAxis),
                    Input.GetAxis(fireAxis)
                );
        }
    }
}