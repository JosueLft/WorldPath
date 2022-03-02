using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(GameManager))]
public class Player : MonoBehaviour {

  public Entity entity;

    [Header("Player Regen System")]
    public bool regenHPEnabled = true;
    public float regenHPTime = 5.0f;
    public int regenHPValue = 5;
    public bool regenMPEnabled = true;
    public float regenMPTime = 5.0f;
    public int regenMPValue = 5;

    [Header("GameManager")]
    public GameManager manager;

    [Header("Player UI")]
    public Slider HPSlider;
    public Slider MPSlider;
    public Slider EXPSlider;
    public Text currentHP;
    public Text currentMP;
    public Text level;
    public Text levelPercentage;

    PhotonView pv;

    void Start() {
        if(manager == null) {
            Debug.LogFormat("Você precisa anexar o game manager aqui no player");
            manager = GetComponent<GameManager>();
            return;
        }

        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        HPSlider.maxValue = entity.maxHealth;
        HPSlider.value = HPSlider.maxValue;
        currentHP.text = entity.maxHealth.ToString();

        MPSlider.maxValue = entity.maxMana;
        MPSlider.value = MPSlider.maxValue;
        currentMP.text = entity.maxMana.ToString();

        EXPSlider.value = entity.exp;
        levelPercentage.text = "0.00%";
        level.text = "lvl. " + entity.level;

        //Iniciar o regenHealth
        StartCoroutine(RegenHealth());
        StartCoroutine(RegenMana());
    }

    void Update() {
        if(entity.dead) {
            return;
        }
        if(entity.currentHealth <= 0) {
            Die();
        }
        HPSlider.value = entity.currentHealth;
        currentHP.text = entity.currentHealth + " / " + entity.maxHealth;
        MPSlider.value = entity.currentMana;
        currentMP.text = entity.currentMana + " / " + entity.maxMana;

        level.text = "lvl. " + entity.level;
        EXPSlider.value = entity.exp;
        levelPercentage.text = "0.00%";
    }

    IEnumerator RegenHealth() {
        while(true) {
            if(regenHPEnabled) {
                if(entity.currentHealth < entity.maxHealth) {
                    entity.currentHealth += regenHPValue;
                    yield return new WaitForSeconds(regenHPTime);
                } else {
                    yield return null;
                }
            } else {
                yield return null;
            }
        }
    }

    IEnumerator RegenMana() {
        while(true) {
            if(regenMPEnabled) {
                if(entity.currentMana < entity.maxMana) {
                    entity.currentMana += regenMPValue;
                    yield return new WaitForSeconds(regenMPTime);
                } else {
                    yield return null;
                }
            } else {
                yield return null;
            }
        }
    }

    void Die() {
        entity.currentHealth = 0;
        entity.dead = true;
        entity.target = null;
        StopAllCoroutines();
    }

    public void Damages(int valueDamage) {
        pv.RPC("TakeDamage", RpcTarget.AllBuffered, valueDamage);
    }

    void TakeDamage(int damage) {
        entity.currentHealth -= damage/100;
    }
}