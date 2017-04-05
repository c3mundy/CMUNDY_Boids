using UnityEngine;
using System.Collections;

public class LeadingBoidScript : MonoBehaviour
{
    public Transform goalPos;
    public NavMeshAgent leader;
    public GameObject goalPrefab;
    public float mySpeed;
    Rigidbody rb;
    public float maxSpeed;

    NavMeshPath myPath;
    GameObject myGoal;
    float distance;
    Vector3 newDestination;
    bool setDestination = true;
    Vector3 myDirection;
    bool beginMoving = false;
    bool fishMove = false;
    bool setFishDest = true;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        myPath = new NavMeshPath();        
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (SceneController.singleton.currentScene == 0)
        {
            FloorFlockBehaviour();
        }

        if (SceneController.singleton.currentScene == 1)
        {
            AirFlockBehaviour();
        }

        if(SceneController.singleton.currentScene == 2)
        {
            FishShoalBehaviour();
        }
    }

    void FloorFlockBehaviour()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //if the mouse click hits an object tagged as the floor place a seed

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 500))
            {
                if (hit.transform.tag == "Floor")
                {

                    Destroy(myGoal);

                    //set destination for leader to travel to via NavMesh Agent - it moves there automatically
                    leader.destination = hit.point;
                    myGoal = GameObject.Instantiate(goalPrefab, hit.point, transform.rotation) as GameObject;
                    Debug.Log(hit.transform.tag);


                    distance = leader.remainingDistance;
                }
            }
        }
    }

    void AirFlockBehaviour()
    {
            if (setDestination)
            {
            //set the destination manually choosing random values
                float newXPos = Random.Range(85.7f, 416);
                //float newYPos = Random.Range(4.4f, 47.4f);
                float newZPos = Random.Range(98.8f, 411.8f);

                newDestination = new Vector3(newXPos, transform.position.y, newZPos);
                setDestination = false;
            }

            if (Input.GetKeyDown(KeyCode.Space) && !beginMoving)
            {
                beginMoving = !beginMoving;
            }

        if (beginMoving)
        {
            //move to that destination
            myDirection = newDestination - transform.position;
            myDirection.Normalize();
            rb.velocity = mySpeed * myDirection;

            if (Vector3.Distance(transform.position, newDestination) < 0.1f)
            {
                setDestination = true;
            }
        }
    }

    void FishShoalBehaviour()
    {
        if (setFishDest)
        {
            //set destination manually by choosing random values
            float newXPos = Random.Range(89.2f, 415.9f);
            //float newYPos = Random.Range(4.4f, 47.4f);
            float newZPos = Random.Range(72f, 418f);

            newDestination = new Vector3(newXPos, transform.position.y, newZPos);
            setFishDest = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            fishMove = !fishMove;
        }

        if (fishMove)
        {
            //move to that destination
            myDirection = newDestination - transform.position;
            myDirection.Normalize();
            rb.velocity = mySpeed * myDirection;

            if (Vector3.Distance(transform.position, newDestination) < 0.1f)
            {
                setFishDest = true;
            }
        }
    }

}
