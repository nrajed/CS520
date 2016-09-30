using UnityEngine;
using System.Collections;

//Moves the main camera when right buttons pressed
public class moveCamera : MonoBehaviour
{

    public float speed = 50.0f; //max speed of camera
    public float mouseWheelFactor = 10f;

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        //Movement of Camera
        Vector3 dir = new Vector3(); //create (0,0,0)

		if (Input.GetKey(KeyCode.W)) {
			dir.z = 1;
		}
		if (Input.GetKey(KeyCode.S)) {
			dir.z = -1;
		}

        dir.y = 0;
        GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - 1 * Input.GetAxis("Mouse ScrollWheel") * mouseWheelFactor;
        //if the size is <3, bring it back up to 3
        if (GetComponent<Camera>().orthographicSize < 3)
        {
            GetComponent<Camera>().orthographicSize = 3;
        }

            if (Input.GetKey(KeyCode.A)) {
			dir.x = -1;
		}
		if (Input.GetKey(KeyCode.D)) {
			dir.x = 1;
		}

		Vector3 movement = Quaternion.Euler(0, Camera.main.transform.localEulerAngles.y, 0) * dir;

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}
