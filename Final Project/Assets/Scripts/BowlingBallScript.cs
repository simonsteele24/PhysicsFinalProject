using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingBallScript : MonoBehaviour
{
    public float hitRadius = 1.5f;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        Vector3 direction =  player.transform.position - transform.position;

        if (Physics.Raycast(transform.position,direction.normalized,out hit,hitRadius))
        {
            if (hit.collider.tag == "Player")
            {
                player.GetComponent<PlayerController>().DamagePlayer(transform.position);
            }
        }
    }
}
