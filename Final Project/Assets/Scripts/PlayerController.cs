using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float health = 10;

    // Texts
    public Text healthText;


    public void DamagePlayer(Vector3 knockbackPosition)
    {
        if (!GetComponent<PlayerScript>().isProne)
        {
            health--;
            healthText.text = health.ToString();
            if (health <= 0)
            {
                GetComponent<PlayerScript>().animator.SetTrigger("isDead");
                GetComponent<PlayerScript>().animator.SetTrigger("Knockbacking");
                GetComponent<PlayerScript>().isDead = true;
                GameManager.manager.RestartLevel();
            }
            GetComponent<PlayerScript>().AddKnockBack(knockbackPosition);
        }        
    }

    private void Update()
    {
        if (transform.position.y < GameManager.manager.bottomOfWorldY)
        {
            GameManager.manager.RestartLevel();
        }
    }
}
