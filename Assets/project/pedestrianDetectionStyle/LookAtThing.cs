using UnityEngine;
using System.Collections;

public class LookAtThing : MonoBehaviour {

	public Transform target;
	public Projector projector;
	public bool animate = false;
	float defaultOrthographicSize;

	// Use this for initialization
	void Start () {
		defaultOrthographicSize = projector.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
		if (animate) {
			projector.orthographicSize = defaultOrthographicSize + Mathf.Sin (Time.time * 2.0f) * 0.15f;
		}
	}

	void LateUpdate() {
		transform.localPosition = Vector3.zero;
		transform.position = new Vector3 (transform.position.x, target.position.y, transform.position.z);
		transform.LookAt (target, Vector3.up);
	}
}
