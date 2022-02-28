using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BruteControl : MonoBehaviour {

    public Entity entity;
    public GameManager manager;
    public float patrulhaTempo = 15;
    private WaitForSeconds tempo;
    public Transform[] waypoints;
    private int index;
    private NavMeshAgent agent;
    private Animator anim;

    // Attack
    [SerializeField] private GameObject player;
    private bool pegaEle = false;
    [SerializeField] private float dist = 5;
    [SerializeField] float distAttack;
    [SerializeField] private bool attack = false;

    void Start() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        tempo = new WaitForSeconds(patrulhaTempo);
        agent = GetComponent<NavMeshAgent>();
        index = Random.Range(0, waypoints.Length);
        anim = GetComponent<Animator>();
        StartCoroutine(ChamaPatrulha());
        distAttack = agent.stoppingDistance;
    }

    void Update() {
        anim.SetFloat("Move", agent.velocity.sqrMagnitude, 0.06f, Time.deltaTime);

        PegaHeroi();
        AttackEnemy();
    }

    IEnumerator ChamaPatrulha() {
        while(true) {
            yield return tempo;
            Patrol();
        }
    }

    void Patrol() {
        index = index == waypoints.Length - 1 ? 0 : index + 1;
        agent.destination = waypoints[index].position;
    }

    void PegaHeroi() {
        if(player != null && Vector3.Distance(transform.position, player.transform.position) < dist && !pegaEle) {
            pegaEle = true;
        } else if(Vector3.Distance(transform.position, player.transform.position) > dist) {
            pegaEle = false;
        }

        if(pegaEle) {
            agent.destination = player.transform.position;
        }
    }

    void AttackEnemy() {
        if(player != null && Vector3.Distance(transform.position, player.transform.position) <= distAttack && pegaEle) { // verificação de distancia
            anim.SetBool("Attack", true);
            attack = true;
        } else if(player != null && Vector3.Distance(transform.position, player.transform.position) > distAttack && pegaEle) {
            anim.SetBool("Attack", false);
        }

        if(attack) {
            agent.speed = 0;
            agent.isStopped = true;
        } else {
            agent.speed = 1;
            agent.isStopped = false;
        }
        if(attack) {
            if(entity.attackTimer > 0) {
                entity.attackTimer -= Time.deltaTime;
            }

            if(entity.attackTimer < 0) {
                entity.attackTimer = 0;
            }
            CalculateDamage();
        }
    }

    void CalculateDamage() {
        int dmg = manager.CalculateDamage(entity, entity.physicsDamage);
        int targetDef = manager.CalculateDefense(entity.target.GetComponent<Player>().entity, entity.target.GetComponent<Player>().entity.defense);
        int dmgResult = dmg - targetDef;

        if(dmgResult < 0)
            dmgResult = 0;

        Debug.LogFormat("Inimigo atacou o player, DMG: {0}", dmgResult);
        entity.target.GetComponent<Player>().entity.currentHealth -= dmgResult;
    }

    void AnulaAttack() {
        attack = false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dist);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distAttack);
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
}
