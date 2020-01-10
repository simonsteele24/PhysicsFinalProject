using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    // Game Objects
    public GameObject ball;

    // Floats
    public float timeToSpawnNextBall = 20;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartSpawningBalls());
    }

    // This function waits a given amount of time and then instantiates a rolling
    // ball at the given transform of the attached Game object
    IEnumerator StartSpawningBalls()
    {
        while(true)
        {
            yield return new WaitForSeconds(timeToSpawnNextBall);
            Instantiate(ball, transform.position, Quaternion.identity);
        }
    }
}
