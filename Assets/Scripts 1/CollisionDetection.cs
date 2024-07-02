using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public PlayerController atk;
    //public EnemyScript bruh;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && atk.isAttacking)
        {
            Debug.Log(other.name);
            other.GetComponent<Animator>().SetTrigger("Hit");
            other.GetComponent<EnemyScript>().TakeDamage(40f);
        }
    }
}
