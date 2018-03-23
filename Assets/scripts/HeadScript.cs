using UnityEngine;
using System.Collections;

public class HeadScript : MonoBehaviour {
    private Rigidbody rigidbody;
    public GameObject head;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate(){
        rigidbody.MovePosition(head.transform.position);
        rigidbody.MoveRotation(head.transform.rotation);
    }
}
