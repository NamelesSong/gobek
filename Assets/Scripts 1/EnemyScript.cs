using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    float currentHealth;
    private float lerpSpeed = 0.05f;

    private GameObject player;
    //public PlayerController bruh;
    private Animator animator;
    

    public float attackCD = 3f;
    public float attackRange = 4f;

    public bool canAttack = true;
    public bool isAttacking = false;
    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHealth, lerpSpeed);
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
        {
            if (canAttack)
            {
                animator.SetTrigger("EnemyAttack");
                isAttacking = true;
                canAttack = false;
                StartCoroutine(ResetAttackCooldown());
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && isAttacking)
        {
            Debug.Log(other.gameObject.name);
            player.GetComponent<PlayerController>().TakeDamage(40f);
        }
    }

    IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(1.0f);
        canAttack = true;
    }

    IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
    }

    public void TakeDamage(float damage) //silah script ine  taþýmaya çalýþ
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }

        void Die()
        {
            Debug.Log("Ölü");
            //ölme animasyonu
        }   
    }
}
