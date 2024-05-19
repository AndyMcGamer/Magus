using FishNet;
using FishNet.Connection;
using Magus.Game;
using Magus.PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Skills.ActiveSkills
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private GameObject visual;

        private float passedTime = 0f;
        private float moveRate;
        private float lifetime;

        public string playerTag;
        public float damage;

        public void Initialize(ProjectileSkillData skillData, float passedTime, string playerTag, int skillLevel = 0, bool showVisual = true)
        {
            this.passedTime = passedTime;
            this.playerTag = playerTag;

            gameObject.tag = playerTag;

            visual.SetActive(showVisual);

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
                    if (other.TryGetComponent<PlayerCollisionHandler>(out var collisionHandler))
                    {
                        NetworkConnection conn = other.GetComponent<PlayerCollisionHandler>().Owner;
                        GlobalPlayerController.instance.ChangePlayerHealth(conn, -damage);
                        print(other.name);
                    }
                    
                }
                
                Destroy(gameObject);
            }
        }
    }
}
