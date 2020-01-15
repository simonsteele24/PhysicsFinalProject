using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float health = 10;
    
    public void DamagePlayer(Vector3 knockbackPosition)
    {
        health--;
        if (health <= 0)
        {
            GameManager.manager.RestartLevel();
        }
        GetComponent<PlayerScript>().AddKnockBack(knockbackPosition);
        Debug.Log("Ooph");
    }

    private void Update()
    {
        if (transform.position.y < GameManager.manager.bottomOfWorldY)
        {
            GameManager.manager.RestartLevel();
        }
    }
}
