using FishNet;
using Magus.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.ActiveSkills
{
    public class Projectile : MonoBehaviour
    {
        private Vector3 direction;
        private float passedTime = 0f;
        private float moveRate;
        private float lifetime;

        public string playerTag;
        public float damage;

        public void Initialize(ProjectileSkillData skillData, Vector3 direction, float passedTime, string playerTag, int skillLevel = 0)
        {
            this.passedTime = passedTime;
            this.direction = direction;
            this.playerTag = playerTag;

            // replace skill level with global skill manager level in the future
            this.moveRate = skillData.moveRate[skillLevel];
            this.lifetime = skillData.lifetime[skillLevel];
            this.damage = skillData.damage[skillLevel];
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (gameObject == null) return;
            float delta = Time.deltaTime;

            float passedTimeDelta = 0f;
            if(passedTime > 0f)
            {
                float step = (passedTime * 0.2f);
                passedTime -= step;

                if (passedTime <= (delta / 2f))
                {
                    step += passedTime;
                    passedTime = 0f;
                }
                passedTimeDelta = step;
            }

            transform.position += transform.forward * (moveRate * (delta + passedTimeDelta));
            lifetime -= delta;
            if (lifetime < 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag))
            {
                if (InstanceFinder.IsServerStarted)
                {

                }
                Destroy(gameObject);
            }
        }
    }
}
