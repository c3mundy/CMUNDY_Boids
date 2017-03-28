using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {

    public float speed;
    public Rigidbody rb;
    public Camera cam;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Movement();
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 50f);

	}

    void Movement()
    {
        Vector3 inputForce = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        rb.MovePosition(transform.position + inputForce * Time.deltaTime * speed);
        
    }
}
