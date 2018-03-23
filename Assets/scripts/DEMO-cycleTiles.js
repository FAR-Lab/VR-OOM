
var tiles : GameObject[];
private var index : int;


function Update () {
	if (Input.GetKeyDown(KeyCode.Space)) {
		var oldTiles = tiles[index];
		if (index < tiles.Length-1) {
			index++;
		}
		else {
			index = 0;
		}
		oldTiles.SetActiveRecursively(false);
		tiles[index].SetActiveRecursively(true);
	}
}