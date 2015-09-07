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

	public CornerObject _cornerObj;
	public enum TypeOfCabinet {CornerCabinet, BaseCabinet, TallCabinet, MidSizeCabinet, WallCabinets};
	public TypeOfCabinet _typeOfCabinet;


	public bool isCornerUnit; 		// isCornerUnit = true, if this obj is a corner unit
	public Vector3 boundExtends; 	// mesh bounds of object
	// Use this for initialization
	void Start () {
		_typeOfCabinet = TypeOfCabinet.CornerCabinet;
		CabinetManager.Instance.cabinetsInScene.Add (this.gameObject);

		if (isCornerUnit) { // if this object is a corner object
			_cornerObj.cornerVert = ClosestVertex ().GetComponent<VertScript> (); //assigning the closes vertex

			// adding the corner unit to the wall attached to the vertex
			_cornerObj.cornerVert.wall1.cornerObjects.Add(this.gameObject);            
			_cornerObj.cornerVert.wall2.cornerObjects.Add(this.gameObject);
		}


//		Positioning ();
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown (KeyCode.Q))
//			  Positioning (); // temporarily kept on update, should be called only when an event is triggered
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

		if (isCornerUnit) {
			gameObject.transform.position = _cornerObj.cornerVert.transform.position;
			if(gameObject.tag == "cornerUnitL")
				gameObject.transform.rotation = _cornerObj.cornerVert.transform.rotation;
			boundExtends = CabinetManager.Instance.BoundExtends (this.gameObject);

			if(_cornerObj.cornerVert.name == "vertexA1")
				gameObject.transform.position = new Vector3(gameObject.transform.position.x + boundExtends.x ,0 , gameObject.transform.position.z + boundExtends.z);

			if(_cornerObj.cornerVert.name == "vertexA2")
				gameObject.transform.position = new Vector3(gameObject.transform.position.x + boundExtends.x , 0 , gameObject.transform.position.z - boundExtends.z);

			if(_cornerObj.cornerVert.name == "vertexB1")
				gameObject.transform.position = new Vector3(gameObject.transform.position.x - boundExtends.x , 0 , gameObject.transform.position.z + boundExtends.z);

			if(_cornerObj.cornerVert.name == "vertexB2")
				gameObject.transform.position = new Vector3(gameObject.transform.position.x - boundExtends.x , 0 , gameObject.transform.position.z - boundExtends.z);

		}
	}
}
