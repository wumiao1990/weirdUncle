using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour {
	public enum Action{
		ADD,
		REMOVE
	}

	public class Path {
		public Vector3[] nodes;

		public float GetDistanceAlongPath(){
			float distance = 0;
			if (nodes == null || nodes.Length <= 1) {
				return distance;
			} else {
				for (int index = 0; index < nodes.Length-1; index++) {
					distance += Vector3.Distance (nodes [index], nodes [index + 1]);
				}
				return distance;
			}
		}
	}

	public static GroundManager instance;

	public GameObject starpos;

	/* public variables */
	public bool showNodes;

	public const int nodeWidth = 30;
	public const int nodeHeight = 3;
	
	public const int startX = 0;
	public const int startY = 0;

	public int[,] instanceNodes;
	public bool[,] pathNodesWithoutWall;
	public bool[,] pathNodesWithWall;


	void Awake(){
		instance = this;
	}

	Color color = Color.green;
	void OnDrawGizmos() {
		if (!showNodes) {
			return;
		}

		if (this.pathNodesWithoutWall == null) {
			return;
		}

		for (int x = startX; x < nodeWidth; x++) {
			for (int y = startY; y < nodeHeight; y++) {
				if (this.pathNodesWithoutWall [x, y] == true) {
					//walkable
					color = Color.green;
					color.a = 0.5f;
					Gizmos.color = color;
				} else {
					color = Color.red;
					color.a = 0.5f;
					Gizmos.color = color;
				}
				Gizmos.DrawCube (new Vector3 (x+0.5f, y+0.5f, 0), new Vector3 (0.4f, 0.4f, 0.4f));
			}
		}


	}

	public void UpdateAllNodes(){
		this.instanceNodes = new int[nodeWidth, nodeHeight];
		this.pathNodesWithoutWall = new bool[nodeWidth, nodeHeight];
		this.pathNodesWithWall = new bool[nodeWidth, nodeHeight];

		for (int x = startX; x < nodeWidth; x++) {
			for (int y = startY; y < nodeHeight; y++) {
				this.instanceNodes [x, y] = -1;
				this.pathNodesWithoutWall [x, y] = true;
				this.pathNodesWithWall [x, y] = true;
			}
		}

		foreach (KeyValuePair<int, BaseItemScript> entry in SceneManager.instance.GetItemInstances()) {
			BaseItemScript item = entry.Value;
            if (!item.itemData.configuration.isCharacter) {
				this.UpdateBaseItemNodes (item, Action.ADD);
			}
		}
	}

	public void UpdateBaseItemNodes(BaseItemScript item, Action action){

		Vector3 pos = item.GetPosition ();

		int x = (int)(pos.x);
		int y = (int)(pos.y);
		int sizeX = (int)item.GetSize ().x;
		int sizeY = (int)item.GetSize ().y;

		for (int indexX = x; indexX < x + sizeX; indexX++) {
			for (int indexY = y; indexY < y + sizeY; indexY++) {
				bool isCellWalkable = false;
				if ((sizeX > 2 && indexX == x) || (sizeX > 2 && indexX == x + sizeX - 1) || (sizeY > 2 && indexY == y) || (sizeY > 2 && indexY == y + sizeY - 1)) {
					//use this for make outer edge walkable for items have size morethan 2x2
					isCellWalkable = true;
				}

				if (item.itemData.name == "ArmyCamp") {
					//make every cell walkable in army camp
					isCellWalkable = true;
				}

				if (action == Action.ADD) {
					//adding scene item to nodes, so walkable is false
					this.instanceNodes [indexX, indexY] = item.instanceId;

					if (item.itemData.name == "Wall") {
						this.pathNodesWithoutWall [indexX, indexY] = true;
						this.pathNodesWithWall [indexX, indexY] = false;
					} else {
						this.pathNodesWithoutWall [indexX, indexY] = isCellWalkable;
					}

				} else if (action == Action.REMOVE) {
					if(this.instanceNodes[indexX, indexY] == item.instanceId){
						this.instanceNodes [indexX, indexY] = -1;
						this.pathNodesWithoutWall [indexX, indexY] = true;
						this.pathNodesWithWall [indexX, indexY] = true;
					}
				}

			}
		}
	}

	public Path GetPath(Vector3 startPoint, Vector3 endPoint, bool considerWalls){
		Path path = new Path ();

		if (endPoint.x < 0 || endPoint.x >= nodeWidth || endPoint.z < 0 || endPoint.z >= nodeHeight) {
			Debug.LogError ("The target point is out of the grid!");
			return path;
		}

		Vector2 startPointInMap = new Vector2 (startPoint.x, startPoint.z);
		Vector2 endPointInMap = new Vector2 (endPoint.x, endPoint.z);
		SearchParameters searchParameter = null;

		if (considerWalls) {
			searchParameter = new SearchParameters (startPointInMap, endPointInMap, pathNodesWithWall);
		} else {
			searchParameter = new SearchParameters (startPointInMap, endPointInMap, pathNodesWithoutWall);
		}

		PathFinder pathFinder = new PathFinder (searchParameter);
		List<Vector2> points = pathFinder.FindPath ();


		int index = -1;
		List<Vector3> nodes = new List<Vector3> ();
		foreach (Vector2 point in points) {
			index++;
//			if (index < points.Count-1 && index % 2 == 1) {
//				continue; //skip consecutive nodes
//			}
			Vector3 pointInGround = new Vector3 (point.x, 0, point.y);
			nodes.Add (pointInGround);
		}
		path.nodes = nodes.ToArray ();

		return path;
	}

	public Vector3 GetRandomFreePosition(){
		int x = Random.Range (5, nodeWidth-5);
		int z = Random.Range (5, nodeHeight-5);

		if (this.instanceNodes [x, z] != -1) {
			return GetRandomFreePosition ();
		}
		return new Vector3 (x, 0, z);
	}

	public Vector3 GetRandomFreePositionForItem(int sizeX, int sizeY){
		Vector3 randomPosition = new Vector3(Random.Range (0, nodeWidth), 0, Random.Range (0, nodeHeight));

		if (!IsPositionPlacable (randomPosition, sizeX, sizeY, -1)) {
			return GetRandomFreePositionForItem (sizeX, sizeY);
		}

		return randomPosition;
	}

	public Vector3 GetFreePositionForItem(int sizeX, int sizeZ){
		Vector3 freePosition = new Vector3 (-1, 0, -1);

		int startX = (int)nodeWidth / 2;
		int startZ = (int)nodeWidth / 2;
		for (int r = 0; r < (int)nodeWidth / 2; r++) {
			for (int x = -r; x <= r; x++) {
				for (int z = -r; z <= r; z++) {
					if (x == -r || x == r || z == -r || z == r) {
						freePosition.x = startX + x;
						freePosition.z = startZ + z;
						if (IsPositionPlacable (freePosition, sizeX, sizeZ, -1)) {
							return freePosition;
						}
					}
				}
			}
		}
		return freePosition;
	}

	public bool IsPositionPlacable(Vector3 position, int sizeX, int sizeY, int instanceId){
		int posX = (int)position.x;
		int posY = (int)position.y;

		for (int indexX = posX; indexX < posX + sizeX; indexX++) {
			for (int indexY = posY; indexY < posY + sizeY; indexY++) {
				if (indexX < 0 || indexX >= nodeWidth || indexY < 0 || indexY >= nodeHeight) {
					//outside grid
					return false;
				}

				if (instanceNodes [indexX, indexY] != -1 && instanceNodes [indexX, indexY] != instanceId) {
					return false;
				}

			}
		}
		return true;
	}

	public BaseItemScript GetItemInPosition(Vector3 position){
		int posX = (int)position.x;
		int posZ = (int)position.z;

		if (posX < 0 || posX >= nodeWidth || posZ < 0 || posZ >= nodeHeight) {
			return null;
		}

		int itemInstanceId = GroundManager.instance.instanceNodes [posX, posZ];
		BaseItemScript item = null;
		SceneManager.instance.GetItemInstances ().TryGetValue (itemInstanceId, out item);
		return item;
	}

}
