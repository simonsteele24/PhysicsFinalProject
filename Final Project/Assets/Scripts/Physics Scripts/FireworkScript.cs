using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkScript : MonoBehaviour
{
    // General variables
    public GameObject fireWork;
    public float fireworksLeft;
    public float fireworksPerLoad;

    // Age variables
    public float minAge;
    public float maxAge;
    float age;

    // Velocity variables
    public Vector3 minVelocity;
    public Vector3 maxVelocity;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent != null)
        {
            fireworksLeft = transform.parent.GetComponent<FireworkScript>().fireworksLeft;
            transform.parent = null;
        }
        // Randomly set the velocity and age
        velocity = new Vector3(Random.Range(minVelocity.x, maxVelocity.x), Random.Range(minVelocity.y, maxVelocity.y), Random.Range(minVelocity.z, maxVelocity.z));
        GetComponent<Particle3D>().velocity = velocity;
        age = Random.Range(minAge, maxAge);

        // Start the lifecycle of the firework
        StartCoroutine(CommenceLifeCycle());
    }





    // This function simulates the entire life of the firework particle
    IEnumerator CommenceLifeCycle()
    {
        yield return new WaitForSeconds(age);

        // Are there more fireworks to spawn?
        if (fireworksLeft > 0 && fireworksLeft != 0)
        {
            // if yes, then spawn more child fireworks
            fireworksLeft--;
            if (fireworksLeft != 0)
            {
                for (int i = 0; i < fireworksPerLoad; i++)
                {
                    var newFirework = Instantiate(fireWork, transform.position, transform.rotation, transform);
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}
