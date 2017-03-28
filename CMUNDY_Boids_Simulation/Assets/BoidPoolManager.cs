using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidPoolManager : MonoBehaviour
{
    public List<BoidsBehaviourScript> boidScripts;
    public GameObject boidPrefab;
    public static BoidPoolManager singleton;

    Vector3 spawnZone;

    void Awake()
    {
        singleton = this;
    }

	// Use this for initialization
	void Start ()
    {
	    boidScripts = new List<BoidsBehaviourScript>();

        for(int i = 0; i < 150; i++)
        {
            spawnZone.x = Random.Range(347, 400);
            spawnZone.y = Random.Range(12, 20);
            spawnZone.z = Random.Range(165, 200);

            GameObject newBoid = Instantiate(boidPrefab);
            newBoid.transform.position = spawnZone;
            boidScripts.Add(newBoid.GetComponent<BoidsBehaviourScript>());
            boidScripts[i].isActive = false;
            boidScripts[i].gameObject.SetActive(false);
        }

        ActivateBoids(20);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void ActivateBoids(int numBoids)
    {
        for(int i = 0; i < boidScripts.Count; i++)
        {
            if(i < numBoids)
            {
                if(!boidScripts[i].isActive)
                {
                    boidScripts[i].isActive = true;
                    boidScripts[i].gameObject.SetActive(true);
                }
            }
            else
            {
                boidScripts[i].isActive = false;
                boidScripts[i].gameObject.SetActive(false);
            }
        }
    }
}
