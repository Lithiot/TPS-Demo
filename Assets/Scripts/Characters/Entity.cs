using UnityEngine;
using TPSDemo.Core;

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

    public class Entity : MonoBehaviour
    {
        protected FiniteStateMachine FSM;
        public float Speed;

        // Recieve damage
        // Deal Damage
        // Health
        // Speed
    }
}