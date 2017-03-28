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
        if (Input.GetMouseButton(0))
        {
            //myRB.velocity = mySpeed * -transform.right;
            myRB.AddForce(-transform.right * mySpeed);
        }

        if(Input.GetMouseButtonUp(0))
        {
            //myRB.velocity = Vector3.zero;
        }

        if(userInput.y > 0)
        {
            myRotation.z += 0.7f;

            //if (myRotation.z > 343.9f)
            //{
            //    myRotation.z = 343.9f;
            //}
        }

        if(userInput.y < 0)
        {
            myRotation.z -= 0.7f;

            //if (myRotation.z < 14)
            //{
            //    myRotation.z = 14;
            //}
        }

        if(userInput.x > 0)
        {
            myRotation.y += 0.7f;

            //if (myRotation.y > 59.9f)
            //{
            //    myRotation.y = 59.9f;
            //}
        }

        if(userInput.x < 0)
        {
            myRotation.y -= 0.7f;

            //if (myRotation.y < -180)
            //{
            //    myRotation.y = 14;
            //}
        }

        myRotation.z = Mathf.Clamp(myRotation.z, -60, 60);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(myRotation), 5 * Time.deltaTime);
    }
}
