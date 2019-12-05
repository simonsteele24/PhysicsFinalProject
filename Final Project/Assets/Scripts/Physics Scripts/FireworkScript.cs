using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkScript : MonoBehaviour
{
    public GameObject fireWork;
    public float fireworksLeft;
    public float fireworksPerLoad;

    public float minAge;
    public float maxAge;

    public Vector3 minVelocity;
    public Vector3 maxVelocity;



    float age;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(Random.Range(minVelocity.x, maxVelocity.x), Random.Range(minVelocity.y, maxVelocity.y), Random.Range(minVelocity.z, maxVelocity.z));
        GetComponent<Particle3D>().velocity = velocity;
        age = Random.Range(minAge, maxAge);
        StartCoroutine(CommenceLifeCycle());
    }


    IEnumerator CommenceLifeCycle()
    {
        yield return new WaitForSeconds(age);
        Debug.Log(fireworksLeft);
        if (fireworksLeft > 0)
        {
            fireworksLeft--;
            for (int i = 0; i < fireworksPerLoad; i++)
            {
                var newFirework = Instantiate(fireWork, transform.position, transform.rotation);
                newFirework.GetComponent<FireworkScript>().fireworksLeft = fireworksLeft;
            }
        }
        gameObject.SetActive(false);
    }
}
