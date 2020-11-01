using System.Collections;
using System.Collections.Generic;
using TPSDemo.Core;
using UnityEngine;

namespace TPSDemo.Characters
{
    public class AnimationsController : MonoBehaviour
    {
        public Animator anim;
        public Entity character;

        private void Awake()
        {
            if (!anim)
                anim = GetComponent<Animator>();
        }

        public void UpdatePlayerAnimations(PlayerStates currentState, FrameInputs inputs) 
        {

        }
    }
}