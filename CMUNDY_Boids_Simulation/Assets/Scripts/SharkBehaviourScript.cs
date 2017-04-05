using UnityEngine;
using System.Collections;

public class SharkBehaviourScript : MonoBehaviour
{
    public Rigidbody myRB;
    public float mySpeed;

    Vector3 userInput;
    Vector3 myRotation;
    bool manualControls = true;

	// Use this for initialization
	void Start ()
    {
        myRotation = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (manualControls)
        {
            userInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            Movement();
        }
    }

    void Movement()
    {
        //handles the user input for the shark movement

        if (Input.GetMouseButton(0))
        {
            myRB.AddForce(-transform.right * mySpeed);
        }

        //sets the new rotation of the shark
        if(userInput.y > 0)
        {
            myRotation.z += 0.7f;
        }

        if(userInput.y < 0)
        {
            myRotation.z -= 0.7f;
        }

        if(userInput.x > 0)
        {
            myRotation.y += 0.7f;
        }

        if(userInput.x < 0)
        {
            myRotation.y -= 0.7f;
        }

        myRotation.z = Mathf.Clamp(myRotation.z, -60, 60);

        //applies the new rotation of the shark
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(myRotation), 5 * Time.deltaTime);
    }
}
