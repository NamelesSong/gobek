using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vuruş : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(other.gameObject.name);
            player.GetComponent<PlayerController>().TakeDamage(40f);
        }
    }
}
