using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject fireWork;
    bool isDone;

    // Update is called once per frame
    void Update()
    {
        GameObject [] remainingBlocks = GameObject.FindGameObjectsWithTag("Destroyable");
        if (remainingBlocks.Length == 0 && !isDone)
        {
            Instantiate(fireWork, transform.position, transform.rotation);
            isDone = true;
        }
    }
}
