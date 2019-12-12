using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float implosionMaxRadius;
    public float implosionMinRadius;
    public float implosionDuration;
    public float implosionForce;

    public float shockwaveSpeed;
    public float shockwaveThickness;
    public float peakConcussionForce;
    public float concussionDuration;

    public float peakConvectionForce;
    public float chimneyRadius;
    public float chimneyHeight;
    public float convectionDuraction;

    bool isConcussing;
    bool hasConvection;

    float time;

    List<Particle3D> objectsInRadius;

    // Start is called before the first frame update
    void Start()
    {
        GameObject [] objects = GameObject.FindGameObjectsWithTag("Destroyable");

        for (int i = 0; i < objects.Length; i++)
        {
            
            if (Vector3.Distance(transform.position, objects[i].transform.position) < implosionMaxRadius)
            {
                objects[i].GetComponent<Particle3D>().enabled = true;
            }
        }

        StartCoroutine(StartExplosion());
    }

    private void Update()
    {
        if (isConcussing)
        {
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

        if (hasConvection)
        {
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


    IEnumerator StartExplosion()
    {
        yield return new WaitForEndOfFrame();

        // Implosion
        GetAllParticlesWithinRadius(implosionMinRadius, implosionMaxRadius);
        AddImplosionForce();

        yield return new WaitForSeconds(implosionDuration);

        isConcussing = true;

        yield return new WaitForSeconds(concussionDuration);

        isConcussing = false;
        hasConvection = true;

        yield return new WaitForSeconds(convectionDuraction);

        hasConvection = false;
    }




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




    void AddImplosionForce()
    {
        for (int i = 0; i < objectsInRadius.Count; i++)
        {
            Vector3 normal = transform.position - objectsInRadius[i].position;
            objectsInRadius[i].AddForce(normal * implosionForce);
        }
    }
}
