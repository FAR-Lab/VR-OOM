
var spinRate : float;

function Update () {
	gameObject.transform.Rotate(Vector3(0.0, (spinRate * Time.deltaTime), 0.0));
}