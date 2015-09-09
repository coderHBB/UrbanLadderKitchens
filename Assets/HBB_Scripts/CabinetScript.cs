/*****************
 * Manish
 * This script is defines a cabinet
 *****************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class CornerObject 
{
	public VertScript cornerVert;
}

public class CabinetScript : MonoBehaviour {

	public enum TypeOfCabinet {CornerCabinet, BaseCabinet, TallCabinet, MidSizeCabinet, WallCabinets, Hob};
	public TypeOfCabinet _typeOfCabinet;

	public CornerObject _cornerObj;

	//common variables
	public float sizeOfCabinet; // The size of the cabinet
	public Vector3 boundExtends; 	// mesh bounds of object
	public GameObject attachedToWall; // The wall to which this cabinet is attached
	[HideInInspector]
	public bool isAddedToWall; // to add the cabinet to wall's list only once
	// Use this for initialization
	void Start () {

		boundExtends = CabinetManager.Instance.BoundExtends (this.gameObject);
		CabinetManager.Instance.cabinetsInScene.Add (this.gameObject);

		if (_typeOfCabinet == TypeOfCabinet.CornerCabinet) { // if this object is a corner object
			_cornerObj.cornerVert = ClosestVertex ().GetComponent<VertScript> (); //assigning the closes vertex

			// adding the corner unit to the wall attached to the vertex
			_cornerObj.cornerVert.wall1.cornerObjects.Add(this.gameObject);            
			_cornerObj.cornerVert.wall2.cornerObjects.Add(this.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	GameObject ClosestVertex ()
	{
		List<GameObject> verts = new List<GameObject> ();
		verts.Add (RoomManager.Instance.vertA1);
		verts.Add (RoomManager.Instance.vertA2);
		verts.Add (RoomManager.Instance.vertB1);
		verts.Add (RoomManager.Instance.vertB2);

		float minimumDist;
		float distance;
		GameObject closestObject;
		minimumDist = Vector3.Distance (gameObject.transform.position, verts [0].transform.position);
		closestObject = verts [0];
		foreach(GameObject v in verts)
		{
			distance = Vector3.Distance (gameObject.transform.position,v.transform.position);
			if(distance <= minimumDist)
			{
				minimumDist = distance;
				closestObject = v;
			}


		}
		return closestObject;

	}

	public void Positioning ()
	{
		switch (_typeOfCabinet) {

		case TypeOfCabinet.CornerCabinet:

			gameObject.transform.position = _cornerObj.cornerVert.transform.position;
			if(gameObject.tag == "cornerUnitL")
				gameObject.transform.rotation = _cornerObj.cornerVert.transform.rotation;
			boundExtends = CabinetManager.Instance.BoundExtends (this.gameObject);
			
			if(_cornerObj.cornerVert.name == "vertexA1")
				gameObject.transform.position = new Vector3(gameObject.transform.position.x + boundExtends.x ,0 , gameObject.transform.position.z + boundExtends.z);
			
			if(_cornerObj.cornerVert.name == "vertexA2")
				gameObject.transform.position = new Vector3(gameObject.transform.position.x + boundExtends.x ,0 , gameObject.transform.position.z - boundExtends.z);
			
			if(_cornerObj.cornerVert.name == "vertexB1")
				gameObject.transform.position = new Vector3(gameObject.transform.position.x - boundExtends.x ,0 , gameObject.transform.position.z + boundExtends.z);
			
			if(_cornerObj.cornerVert.name == "vertexB2")
				gameObject.transform.position = new Vector3(gameObject.transform.position.x - boundExtends.x ,0 , gameObject.transform.position.z - boundExtends.z);

			break;

		case TypeOfCabinet.BaseCabinet:
			positionCabinets ();
			break;

		case TypeOfCabinet.Hob:
			positionCabinets ();
			break;

		}
	}

	void positionCabinets () // positions all cabinets except corner cabinets
	{
		if (attachedToWall.name == "topWall") {
			gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, attachedToWall.transform.position.z - boundExtends.z - 0.05f);
//			gameObject.transform.Rotate (0,180,0);
		}
		if (attachedToWall.name == "leftWall") {
			gameObject.transform.position = new Vector3 (attachedToWall.transform.position.x + boundExtends.x + 0.05f, gameObject.transform.position.y, gameObject.transform.position.z);
//			gameObject.transform.Rotate (0,90,0);
		}
		if (attachedToWall.name == "rightWall") {
			gameObject.transform.position = new Vector3 (attachedToWall.transform.position.x - boundExtends.x - 0.05f, gameObject.transform.position.y, gameObject.transform.position.z);
//			gameObject.transform.Rotate (0,270,0);
		}
		if (attachedToWall.name == "bottomWall") {
			gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, attachedToWall.transform.position.z + boundExtends.z + 0.05f);
//			gameObject.transform.Rotate (0,0,0);
		}
		boundExtends = CabinetManager.Instance.BoundExtends (this.gameObject);

	}
}
