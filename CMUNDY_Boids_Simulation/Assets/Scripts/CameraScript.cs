using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public Vector3 camOffset = new Vector3(0, 0, -30);
    public Rigidbody rb;
    public int camSpeed;
    public Transform[] cameraPositions;
    public int currentPosition;
    public GameObject leaderBoid;
    public Camera cam;
    public GameObject sharkBoid;
    public Vector3 offset, sharkLookOffset;

    public static CameraScript singleton;

    void Awake()
    {
        singleton = this;
    }

    // Use this for initialization
    void Start ()
    {
        camSpeed = 17;

        if (SceneController.singleton.currentScene == 1)
        {
            transform.position = cameraPositions[currentPosition].position;
            transform.rotation = cameraPositions[currentPosition].rotation;
        }

        if(SceneController.singleton.currentScene == 2)
        {
   
        }
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (SceneController.singleton.currentScene == 2)
        {
            transform.position = Vector3.Lerp(transform.position, sharkBoid.transform.position + (sharkBoid.transform.TransformDirection(offset)), 3 * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(-sharkBoid.transform.right);
            //transform.LookAt(sharkBoid.transform.position + sharkLookOffset);
        }

        //make the camera look at the leader boid in the in-air sim
        if (SceneController.singleton.currentScene == 1)
        {
            transform.LookAt(leaderBoid.transform);
        }

        if (SceneController.singleton.currentScene == 0 || SceneController.singleton.currentScene == 1)
        {
            //controls the zoom functionality for each sim
            if (Input.GetKey(KeyCode.Q))
            {
                //Debug.Break();
                //zoom out
                cam.fieldOfView += 5 * Time.deltaTime;
                if (cam.fieldOfView > 60)
                {
                    cam.fieldOfView = 60;
                }
            }
            if (Input.GetKey(KeyCode.E))
            {
                //zoom in
                cam.fieldOfView -= 5 * Time.deltaTime;
                if (cam.fieldOfView < 13)
                {
                    cam.fieldOfView = 13;
                }
            }
        }

        //controls the movement of the camera for single point sim
        if (SceneController.singleton.currentScene == 0)
        {
            Vector3 inputForce = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            rb.MovePosition(transform.position + inputForce * Time.deltaTime * camSpeed);
        }

        //sets camera bounds for single point sim
        if (SceneController.singleton.currentScene == 0)
        {
            if (transform.position.x < -114.7f)
            {
                transform.position = new Vector3(-114.7f, transform.position.y, transform.position.z);

            }
            if (transform.position.x > 110)
            {
                transform.position = new Vector3(110, transform.position.y, transform.position.z);

            }
            if (transform.position.z < -113.7)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -113.7f);

            }
            if (transform.position.z > 114)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, 114);
            }
        }
    }

    public void ChangeView(bool moveForwards)
    {
        if (moveForwards)
        {
            currentPosition++;
            if(currentPosition > 3)
            {
                currentPosition = 0;
            }

            transform.position = cameraPositions[currentPosition].position;
            transform.rotation = cameraPositions[currentPosition].rotation;
        }
        else
        {
            currentPosition--;
            if(currentPosition < 0)
            {
                currentPosition = 3;
            }

            transform.position = cameraPositions[currentPosition].position;
            transform.rotation = cameraPositions[currentPosition].rotation;
        }
    }


}
