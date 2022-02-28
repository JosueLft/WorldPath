using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Monster : MonoBehaviour {

    public Entity entity;
    public GameManager manager;

    [Header("Experience Reward")]
    public int rewardExperience = 10;
    public int lootGoldMin = 0;
    public int lootGoldMax = 10;

    [Header("Respawn")]
    public GameObject prefab;
    public bool respawn = true;
    public float respawnTime = 10f; // tempo de respawn em segundos

    Rigidbody rigidbody;
    Animator animator;
    NavMeshAgent agent;

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;
    }

    void Update() {
        if(entity.dead) {
            return;
        }

        if(entity.currentHealth <= 0) {
            entity.currentHealth = 0;
            Die();
        }

        if(entity.inCombat) {
            if(entity.attackTimer > 0) {
                entity.attackTimer -= Time.deltaTime;
            }

            if(entity.attackTimer < 0) {
                entity.attackTimer = 0;
            }

            if(entity.target != null && entity.inCombat) {
                // atacar
                if(!entity.combatCoroutine) {
                    StartCoroutine(Attack());
                }
            } else {
                entity.combatCoroutine = false;
                StopCoroutine(Attack());
            }
        } else {
            animator.SetBool("Attack", false);
            entity.inCombat = false;
            entity.target = null;
        }
    }

    private void OnTriggerStay(Collider collider) {
        if(collider.tag == "Player" && !entity.dead) {
            entity.inCombat = true;
            entity.target = collider.gameObject;
        }
    }

    private void OnTriggerExit(Collider collider) {
        if(collider.tag == "Player") {
            entity.inCombat = false;
            entity.target = null;
        }
    }

    IEnumerator Attack() {
        entity.combatCoroutine = true;

        while(true) {
            yield return new WaitForSeconds(entity.cooldown);

            if(entity.target != null && !entity.target.GetComponent<Player>().entity.dead) {
                animator.SetBool("Attack", true);

                float distance = Vector3.Distance(entity.target.transform.position, transform.position);

                if(distance <= entity.attackDistance) {
                    int dmg = manager.CalculateDamage(entity, entity.physicsDamage);
                    int targetDef = manager.CalculateDefense(entity.target.GetComponent<Player>().entity, entity.target.GetComponent<Player>().entity.defense);
                    int dmgResult = dmg - targetDef;

                    if(dmgResult < 0)
                        dmgResult = 0;

                    Debug.LogFormat("Inimigo atacou o player, DMG: {0}", dmgResult);
                    entity.target.GetComponent<Player>().entity.currentHealth -= dmgResult;
                }

                if(entity.inCombat) {
                    agent.speed = 0;
                    agent.isStopped = true;
                } else {
                    agent.speed = 1;
                    agent.isStopped = false;
                }
            }
        }
    }

    void Die() {
        entity.dead = true;
        entity.inCombat = false;
        entity.target = null;

        animator.SetFloat("Move", 0);

        // add exp no player
        // manager.GainExp(rewardExperience);
        Debug.LogFormat("O inimigo {0} morreu", entity.name);
        
        
        StopAllCoroutines();
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn() {
        yield return new WaitForSeconds(respawnTime);

        GameObject newMonster = Instantiate(prefab, transform.position, transform.rotation, null);
        newMonster.name = prefab.name;
        newMonster.GetComponent<Monster>().entity.dead = false;

        Destroy(this.gameObject);
    }
}