using UnityEngine;
using TPSDemo.Core;
using System;
using System.Collections;

namespace TPSDemo.Characters
{
    public class Entity : MonoBehaviour
    {
        protected FiniteStateMachine FSM;

        [Header("Entity Stats")]
        public float speed;
        public float health;

        private float maxHealth;

        public float Health
        {
            get => health;
            private set 
            {
                health = value;

                if (health > maxHealth)
                    health = maxHealth;

                if (OnHealthChanged != null)
                    OnHealthChanged.Invoke(health);
            }
        }

        public Action<float> OnHealthChanged;
        public Action OnEntityDeath;

        private void Awake()
        {
            OnHealthChanged -= CheckPlayerAliveState;
            OnHealthChanged += CheckPlayerAliveState;
        }

        public void RecieveDamage(float damage) 
        {
            health -= damage;
        }

        public void Heal(float amount , bool overTime = false, float duration = 0.0f) 
        {
            if (overTime)
                StartCoroutine(healOverTime(amount , duration));
            else
                health += amount;
        }

        IEnumerator healOverTime(float amount, float duration) 
        {
            amount /= duration;

            while (duration > 0.0f)
            {
                Heal(amount);
                yield return null;

                duration -= Time.deltaTime;
            }
        }

        private void CheckPlayerAliveState(float currentHealth) 
        {
            if (currentHealth <= 0.0f && OnEntityDeath != null)
                OnEntityDeath.Invoke();
        }
    }
}