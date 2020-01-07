using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ball;
    public float timeToSpawnNextBall = 20;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartSpawningBalls());
    }

    IEnumerator StartSpawningBalls()
    {
        while(true)
        {
            yield return new WaitForSeconds(timeToSpawnNextBall);
            Instantiate(ball, transform.position, Quaternion.identity);
        }
    }
}
