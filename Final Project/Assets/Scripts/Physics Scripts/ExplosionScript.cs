using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    // Explosion Variables
    public float explosionRange = 50.0f;

    // Implosion Variables
    public float implosionMaxRadius;
    public float implosionMinRadius;
    public float implosionDuration;
    public float implosionForce;

    // Concussion Variables
    public float shockwaveSpeed;
    public float shockwaveThickness;
    public float peakConcussionForce;
    public float concussionDuration;

    // Convection Variables
    public float peakConvectionForce;
    public float chimneyRadius;
    public float chimneyHeight;
    public float convectionDuraction;

    // General booleans to control explosions
    bool isConcussing;
    bool hasConvection;

    float time;

    List<Particle3D> objectsInRadius;

    // Start is called before the first frame update
    void Start()
    {
        // Find all applicable gameobjects
        GameObject [] objects = GameObject.FindGameObjectsWithTag("Destroyable");

        // Go through all objects and check if objects are within destruction range
        for (int i = 0; i < objects.Length; i++)
        {
            
            if (Vector3.Distance(transform.position, objects[i].transform.position) < implosionMaxRadius)
            {
                objects[i].GetComponent<Particle3D>().enabled = true;
            }
        }

        // Start explosion
        StartCoroutine(StartExplosion());
    }





    
    private void Update()
    {
        // Is the explosion in concussion phase?
        if (isConcussing)
        {
            // Do all calculations to the objects in shockwave range
            time += Time.deltaTime;
            Particle3D[] temp = (Particle3D[])FindObjectsOfType(typeof(Particle3D));
            for (int i = 0; i < temp.Length; i++)
            {
                float distance = Vector3.Distance(temp[i].position, transform.position);
                if (((shockwaveSpeed * time) - shockwaveThickness) <= distance && distance < shockwaveSpeed * time)
                {
                    Vector3 normal = objectsInRadius[i].position - transform.position;
                    objectsInRadius[i].AddForce(normal * (peakConcussionForce * (1 - ((shockwaveSpeed * time) - distance) / shockwaveThickness)));
                }
                else if (shockwaveSpeed * time <= distance && distance < shockwaveSpeed * time + shockwaveThickness) 
                {
                    Vector3 normal = objectsInRadius[i].position - transform.position;
                    objectsInRadius[i].AddForce(normal * peakConcussionForce);
                }
                else if (shockwaveSpeed * time + shockwaveThickness <= distance && distance < shockwaveSpeed * time + shockwaveThickness)
                {
                    Vector3 normal = objectsInRadius[i].position - transform.position;
                    objectsInRadius[i].AddForce(normal * (peakConcussionForce * ((distance - (shockwaveSpeed * time) - shockwaveThickness) / shockwaveThickness)));
                }
            }

        }

        // Is explosion in convection phase?
        if (hasConvection)
        {
            // Get all particles in range and simulate
            GetAllParticlesWithinRadius(0,chimneyRadius);
            for (int i = 0; i < objectsInRadius.Count; i++)
            {
                float tempDist = objectsInRadius[i].position.y;
                float tempExplosionDist = transform.position.y;
                Vector3 tempDistXZ = new Vector3(objectsInRadius[i].position.x, 0, objectsInRadius[i].position.z);
                Vector3 tempExplosionDistXZ = new Vector3(transform.position.x, 0, transform.position.z);
                if (tempDist - tempExplosionDist < chimneyHeight)
                {
                    Vector3 normal = new Vector3(0, tempDist - tempExplosionDist, 0);
                    objectsInRadius[i].AddForce(normal * peakConvectionForce * (Vector3.Distance(tempExplosionDistXZ, tempDistXZ) / shockwaveThickness));
                }
            }
        }
    }





    // This coroutine is meant to simulate the explosion
    IEnumerator StartExplosion()
    {
        yield return new WaitForEndOfFrame();

        // Implosion
        GetAllParticlesWithinRadius(implosionMinRadius, implosionMaxRadius);
        AddImplosionForce();

        yield return new WaitForSeconds(implosionDuration);

        // Concussion
        isConcussing = true;

        yield return new WaitForSeconds(concussionDuration);

        // Convection
        isConcussing = false;
        hasConvection = true;

        yield return new WaitForSeconds(convectionDuraction);

        // End of explosion
        hasConvection = false;
        Destroy(gameObject);
    }





    // This function gets all of the particles within a certain radius relative to the explosion
    void GetAllParticlesWithinRadius(float minRadius, float maxRadius)
    {
        objectsInRadius = new List<Particle3D>();
        Particle3D[] temp = (Particle3D[])FindObjectsOfType(typeof(Particle3D));
        for (int i = 0; i < temp.Length; i++)
        {
            if (Vector3.Distance(temp[i].position, transform.position) < maxRadius && Vector3.Distance(temp[i].position, transform.position) > minRadius)
            {
                objectsInRadius.Add(temp[i]);
            }
        }
    }





    // This function gets all of the particles within a certain radius relative to the explosion on the XZ plane
    void GetAllParticlesWithinXZPlane(float minRadius, float maxRadius)
    {
        objectsInRadius = new List<Particle3D>();
        Particle3D[] temp = (Particle3D[])FindObjectsOfType(typeof(Particle3D));
        for (int i = 0; i < temp.Length; i++)
        {
            Vector3 tempDist = new Vector3(temp[i].position.x, 0, temp[i].position.z);
            Vector3 tempExplosionDist = new Vector3(transform.position.x, 0, transform.position.z);
            if (Vector3.Distance(tempDist, tempExplosionDist) < maxRadius && Vector3.Distance(tempDist, tempExplosionDist) > minRadius)
            {
                objectsInRadius.Add(temp[i]);
            }
        }
    }





    // This function adds an implosion force to each object within range
    void AddImplosionForce()
    {
        for (int i = 0; i < objectsInRadius.Count; i++)
        {
            Vector3 normal = transform.position - objectsInRadius[i].position;
            objectsInRadius[i].AddForce(normal * implosionForce);
        }
    }
}
