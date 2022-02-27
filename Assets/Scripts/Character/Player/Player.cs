using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour 
{
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
    public Slider health;
    public Slider mana;
    public Slider exp;
    public Text currentHP;
    public Text currentMP;
    public Text level;
    public Text levelPercentage;

    void Start() {
        if(manager == null) {
            Debug.LogFormat("Você precisa anexar o game manager aqui no player");
            return;
        }

        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        health.maxValue = entity.maxHealth;
        health.value = health.maxValue;
        currentHP.text = entity.maxHealth.ToString();

        mana.maxValue = entity.maxMana;
        mana.value = mana.maxValue;
        currentMP.text = entity.maxMana.ToString();

        exp.value = entity.exp;
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
        health.value = entity.currentHealth;
        currentHP.text = entity.currentHealth + " / " + entity.maxHealth;
        mana.value = entity.currentMana;
        currentMP.text = entity.currentMana + " / " + entity.maxMana;

        level.text = "lvl. " + entity.level;
        exp.value = entity.exp;
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
}