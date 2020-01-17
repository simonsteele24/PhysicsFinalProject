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
                GameManager.manager.RestartLevel();
            }
            GetComponent<PlayerScript>().AddKnockBack(knockbackPosition);
            Debug.Log("Ooph");
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
