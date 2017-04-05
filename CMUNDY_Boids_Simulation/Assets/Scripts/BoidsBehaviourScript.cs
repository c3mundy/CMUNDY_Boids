using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidsBehaviourScript : MonoBehaviour
{
    public Rigidbody myRB;
    public NavMeshAgent myNMA;
    public bool isLeader;
    public ComputeShader boidAlgorithm;

    public static float alignmentWeighting = 1;
    public static float cohesionWeighting = 1;
    public static float separationWeighting = 1;

    Vector3 myAlignment;
    Vector3 myCohesion;
    Vector3 mySeparation;

    Vector3 appliedForce;

    Vector3 newAlignment;
    Vector3 newCohesion;
    Vector3 newSeparation;

    Vector3 collisionTarget;

    public float maxSpeed;
    public float maxForce;

    public float weighting = 1;

    bool setDestination = true;
    Vector3 newDestination, myDestination;
    bool beginMoving = false;
    public int totalNeighbours;
    public bool isActive;
    public bool isPredator;
    float roamSpeed = 2.9f;

    public struct boidInfo
    {
        public Vector3 velocity;
        public Vector3 position;
        public bool hasNMA;
        public Vector3 alignment;
        public Vector3 cohesion;
        public Vector3 separation;
    }

    public boidInfo myInfo;
    public Vector3[] myForces;
    public Vector3[] alignmentData;
    public Vector3[] cohesionData;
    public Vector3[] separationData;

    [SerializeField]
    List<BoidsBehaviourScript> neighbouringBoids;

    //COMPUTE SHADER VARIABLES NOW UNUSED
    List<boidInfo> neighbourInfo;
    ComputeBuffer myBuffer;
    //ComputeBuffer forceBuffer;
    ComputeBuffer alignmentBuffer;
    ComputeBuffer cohesionBuffer;
    ComputeBuffer separationBuffer;
    int structSize;
    int vectorSize;
    Vector3 avoidanceRot;

	// Use this for initialization
	void Start ()
    {
        structSize = sizeof(float) * 15;
        vectorSize = sizeof(float) * 3;
        myForces = new Vector3[3];

        neighbouringBoids = new List<BoidsBehaviourScript>();
        neighbourInfo = new List<boidInfo>();

        if(GetComponent<NavMeshAgent>())
        {
            myNMA = GetComponent<NavMeshAgent>();
            myInfo.hasNMA = true;
        }
        else
        {
            myInfo.hasNMA = false;
        }

        myBuffer = new ComputeBuffer(150, structSize);
        alignmentBuffer = new ComputeBuffer(150, vectorSize);
        cohesionBuffer = new ComputeBuffer(150, vectorSize);
        separationBuffer = new ComputeBuffer(150, vectorSize);

        if(SceneController.singleton.currentScene != 1)
        {
            isActive = true;
        }

        StartCoroutine("CollisionAvoidance");
	}

    void OnDestroy()
    {
        if (myBuffer != null)
        {
            myBuffer.Release();
        }

        //if(forceBuffer != null)
        //{
        //    forceBuffer.Release();
        //}

        if(cohesionBuffer != null)
        {
            cohesionBuffer.Release();
        }

        if(alignmentBuffer != null)
        {
            alignmentBuffer.Release();
        }

        if(separationBuffer != null)
        {
            separationBuffer.Release();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        //UNUSED IMPLEMENTATION FOR THE COMPUTE SHADER

        //if (!myInfo.hasNMA)
        //{
        //    myInfo.velocity = myRB.velocity;
        //}
        //else
        //{
        //    myInfo.velocity = myNMA.velocity;
        //}

        //if (!myInfo.hasNMA)
        //{
        //    myInfo.position = transform.position;
        //}
        //else
        //{
        //    myInfo.position = myNMA.transform.position;
        //}

        if (isActive && !isPredator)
        {
            if (neighbouringBoids.Count > 0)
            {
                if (totalNeighbours <= 3 && !isLeader)
                {
                    RoamBehaviour();
                }
                else
                {
                    //This is where the algorithm is calculated and applied to the boids
                    CalculateAlignment();
                    CalculateCohesion();
                    CalculateSeparation();
                    CombineAndApplyForces();
                }

                //UNUSED IMPLEMENTATION FOR THE COMPUTE SHADER

                //set up the info list so that it has updated info
                //neighbourInfo.Clear();

                //for(int i = 0; i < neighbouringBoids.Count; i++)
                //{
                //    neighbourInfo.Add(neighbouringBoids[i].myInfo);
                //}

                //prepare the list for the buffer (must be an array as this is what the shader uses)
                //boidInfo[] newInfo = neighbourInfo.ToArray();

                //myBuffer.SetData(newInfo);

                //boidAlgorithm.SetVector("myPos", transform.position);

                //boidAlgorithm.SetBuffer(boidAlgorithm.FindKernel("BoidAlgorithm"), "neighbourBoids", myBuffer);

                //boidAlgorithm.Dispatch(boidAlgorithm.FindKernel("BoidAlgorithm"), totalNeighbours / 8, 1, 1);

                //alignmentBuffer.GetData(alignmentData);
                //cohesionBuffer.GetData(cohesionData);
                //separationBuffer.GetData(separationData);    
            }

            if (!isLeader)
            {
                if (totalNeighbours == 0)
                {
                    RoamBehaviour();
                }
            }

            if (myRB.velocity.magnitude > maxSpeed)
            {
                //cap speed
                myRB.velocity.Normalize();
                myRB.velocity *= maxSpeed;
            }

            if (avoidanceRot != Vector3.zero && SceneController.singleton.currentScene != 1)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation,
                       Quaternion.LookRotation(avoidanceRot), 5 * Time.deltaTime);
            }
        }
    }

    IEnumerator CollisionAvoidance()
    {
        //performs collision avoidance for the boids that aren't in the air scene
        while (isActive && SceneController.singleton.currentScene != 1)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f))
            {
                if (!hit.transform.CompareTag("Boid"))
                {
                    avoidanceRot = hit.normal;
                    avoidanceRot.y = 0.0f;
                }
            }
            else
            {
                avoidanceRot = Vector3.zero;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void CalculateAlignment()
    {
        newAlignment = Vector3.zero;

        //sum of velocity of the visable boids/number of visible boids
        for(int i = 0; i < neighbouringBoids.Count; i++)
        {
            if (neighbouringBoids[i].myNMA)
            {
                newAlignment += (neighbouringBoids[i].myNMA.velocity * 30);
                //I have to set the y to 0 because for some reason the navmesh gives it a massive vertical speed
                newAlignment.y = 0;
            }
            else if (neighbouringBoids[i].GetComponent<SharkBehaviourScript>())
            {
                //Makes the fish evade the shark
                Vector3 evadeDir = -(neighbouringBoids[i].transform.position - transform.position).normalized;
                myRB.AddForce(evadeDir * 100, ForceMode.Acceleration);
                newAlignment = (myRB.velocity.magnitude * evadeDir.normalized) * 30;
            }
            else
            {
                newAlignment += neighbouringBoids[i].myRB.velocity * neighbouringBoids[i].weighting;
            }              
        }

        //UNUSED COMPUTE SHADER CODE

        //for(int i = 0; i < groupAlignment.Length; i++)
        //{
        //    newAlignment += groupAlignment[i];

        //}

        if (neighbouringBoids.Count > 0)
        {
            newAlignment /= neighbouringBoids.Count;
        }
        myAlignment = newAlignment;
        myAlignment.Normalize();

    }

    void CalculateCohesion()
    {
        newCohesion = Vector3.zero;

        //find local centre pos of local boids/number of local boids
        //local centre - myPos
        for(int i = 0; i < neighbouringBoids.Count; i++)
        {

            if (neighbouringBoids[i].myNMA)
            {
                newCohesion += (neighbouringBoids[i].transform.position);
            }
            else
            {
                newCohesion += neighbouringBoids[i].myRB.position * weighting;
            }

        }

        //UNUSED COMPUTE SHADER CODE

        //for (int i = 0; i < groupCohesion.Length; i++)
        //{
        //    newCohesion += groupCohesion[i];
        //}

        if (neighbouringBoids.Count > 0)
        {
            newCohesion /= neighbouringBoids.Count;
        }
        myCohesion = newCohesion - transform.position;
        ////normalise if needed
        myCohesion.Normalize();
    }

    void CalculateSeparation()
    {
        newSeparation = Vector3.zero;

        //negative sum of pos of each boid - visible boids positions
        for(int i = 0; i < neighbouringBoids.Count; i++)
        {
            if (neighbouringBoids[i].GetComponent<SharkBehaviourScript>())
            {
                newSeparation += (neighbouringBoids[i].myRB.position - transform.position) * 790;
            }
            else
            {
                newSeparation += (neighbouringBoids[i].myRB.position - transform.position);
            }
        }

        //UNUSED COMPUTE SHADER CODE

        //for (int i = 0; i < groupSeparation.Length; i++)
        //{
        //    newSeparation += groupSeparation[i];
        //}

        if (neighbouringBoids.Count > 0)
        {
            newSeparation /= neighbouringBoids.Count;
        }

        newSeparation.Normalize();
        //negate to make the boids moves away from each other
        newSeparation *= -1;

        mySeparation = newSeparation;
    }

    void CombineAndApplyForces()
    {
        if (!isLeader)
        {
            //apply weights to forces
            myAlignment *= alignmentWeighting;
            myCohesion *= cohesionWeighting;
            mySeparation *= separationWeighting * 0.6f;
        }

        appliedForce = Vector3.zero;
        appliedForce += myAlignment + myCohesion + mySeparation;

        if (appliedForce.magnitude >= maxForce)
        {
            //cap force
            appliedForce = appliedForce.normalized;
            appliedForce *= maxForce;
        }

        if (!isLeader)
        {
            myRB.AddForce(appliedForce);
        }

        if (myRB.velocity.magnitude > 0.3f)
        {
            //make boids rotate to face the direction they're travelling in
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(myRB.velocity.normalized), 5 * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider newNeighbour)
    {
        //add neighbours to list
        if(newNeighbour.gameObject.CompareTag("Boid") && !newNeighbour.isTrigger)
        {
            neighbouringBoids.Add(newNeighbour.gameObject.GetComponent<BoidsBehaviourScript>());
            //neighbourInfo.Add(newNeighbour.gameObject.GetComponent<BoidsBehaviourScript>().myInfo);
            totalNeighbours++;
        }
    }

    void OnTriggerExit(Collider removableNeighbour)
    {
        //remove neighbours from list
        if(removableNeighbour.gameObject.CompareTag("Boid") && !removableNeighbour.isTrigger)
        {
            neighbouringBoids.Remove(removableNeighbour.gameObject.GetComponent<BoidsBehaviourScript>());
            //neighbourInfo.Remove(removableNeighbour.gameObject.GetComponent<BoidsBehaviourScript>().myInfo);
            totalNeighbours--;
        }
    }

    void RoamBehaviour()
    {
        //make the birds roam in single point flocking and in-air sims
        if(SceneController.singleton.currentScene == 1 || SceneController.singleton.currentScene == 0)
        {
            //Debug.Log(gameObject.name + " Trying to roam");
            if (setDestination)
            {
                float newXPos = Random.Range(85.7f, 416);
                float newZPos = Random.Range(98.8f, 411.8f);

                newDestination = new Vector3(newXPos, transform.position.y, newZPos);
                setDestination = false;
                beginMoving = true;
            }

            if (beginMoving)
            {
                myDestination = newDestination - transform.position;
                myDestination.Normalize();
                myRB.velocity = roamSpeed * myDestination;

                if (Vector3.Distance(transform.position, newDestination) < 0.1f)
                {
                    setDestination = true;
                    beginMoving = false;
                }
            }

            if (myRB.velocity.magnitude > 0.3f)
            {
                //rotate to face direction of travel
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(myRB.velocity.normalized), 5 * Time.deltaTime);
            }
        }
    }
}
