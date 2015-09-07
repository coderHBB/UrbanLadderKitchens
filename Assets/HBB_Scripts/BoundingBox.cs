﻿/*****************
 * Manish
 * This script defines the bounding box attached to walls
*****************/

using UnityEngine;
using System.Collections;

public class BoundingBox : MonoBehaviour {

	public WallScript attachedToWall;
	public Vector3 boundExtends;


	// Use this for initialization
	void Start () {
//		PositionScaleRotation ();
	}
	
	// Update is called once per frame
	void Update () {
		//PositioningAndScaling ();
//		if(Input.GetKeyDown (KeyCode.Q))
//			PositionScaleRotation ();
	}


	public void PositionScaleRotation ()
	{
		float cornerSpace = 0;
		//scale
		gameObject.transform.localScale = new Vector3(attachedToWall.transform.localScale.x,0.6f,0.6f); // 600 mm
		//rotation
		gameObject.transform.rotation = attachedToWall.transform.rotation;

		boundExtends = CabinetManager.Instance.BoundExtends (this.gameObject);

		//position
		if (attachedToWall.name == "topWall") 
			gameObject.transform.position = new Vector3 (attachedToWall.transform.position.x, attachedToWall.transform.position.y, attachedToWall.transform.position.z - boundExtends.z - 0.05f); // attachedWall.boundExtends.z = 0.05f
		if(attachedToWall.name == "leftWall")
			gameObject.transform.position = new Vector3 (attachedToWall.transform.position.x + boundExtends.x + 0.05f, attachedToWall.transform.position.y, attachedToWall.transform.position.z); // attachedWall.boundExtends.x = 0.05f
		if(attachedToWall.name == "rightWall")
			gameObject.transform.position = new Vector3 (attachedToWall.transform.position.x - boundExtends.x - 0.05f, attachedToWall.transform.position.y, attachedToWall.transform.position.z); // attachedWall.boundExtends.x = 0.05f
		if (attachedToWall.name == "bottomWall") 
			gameObject.transform.position = new Vector3 (attachedToWall.transform.position.x, attachedToWall.transform.position.y, attachedToWall.transform.position.z + boundExtends.z + 0.05f); // attachedWall.boundExtends.z = 0.05f


		PositionAndScaleAccordingToCornerObjects (attachedToWall.name);

	}

	void PositionAndScaleAccordingToCornerObjects (string wallName)
	{
		if (wallName == "topWall" || wallName == "bottomWall") {

			if (attachedToWall.cornerObjects.Count == 1) { // if only one corner unit is attached to the wall
				//reducing the scale of the bounding box
				gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - (attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.x * 2),gameObject.transform.localScale.y,gameObject.transform.localScale.z);
				//re-positioning the bounding box according to the corner unit
				if(attachedToWall.cornerObjects[0].transform.position.x < gameObject.transform.position.x)
					gameObject.transform.position = new Vector3(gameObject.transform.position.x + attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.x,gameObject.transform.position.y,gameObject.transform.position.z);
				else
					gameObject.transform.position = new Vector3(gameObject.transform.position.x - attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.x,gameObject.transform.position.y,gameObject.transform.position.z);
				
			}
			
			if (attachedToWall.cornerObjects.Count == 2) { // if two corner units are attached to the wall
				//reducing the scale of the bounding box
				gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - (attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.x * 2) - (attachedToWall.cornerObjects[1].GetComponent<CabinetScript>().boundExtends.x * 2),gameObject.transform.localScale.y,gameObject.transform.localScale.z);
				//since sale is changes bounds also should be updated
				boundExtends = CabinetManager.Instance.BoundExtends (this.gameObject);
				
				if(attachedToWall.cornerObjects[0].transform.position.x < gameObject.transform.position.x)
				{
					gameObject.transform.position = new Vector3(attachedToWall.cornerObjects[0].transform.position.x + attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.x + boundExtends.x, gameObject.transform.position.y,gameObject.transform.position.z);
				}
				if(attachedToWall.cornerObjects[1].transform.position.x < gameObject.transform.position.x)
				{
					gameObject.transform.position = new Vector3(attachedToWall.cornerObjects[1].transform.position.x + attachedToWall.cornerObjects[1].GetComponent<CabinetScript>().boundExtends.x + boundExtends.x, gameObject.transform.position.y,gameObject.transform.position.z);
				}
			}
		}

		if (wallName == "leftWall" || wallName == "rightWall") {

			if (attachedToWall.cornerObjects.Count == 1) { // if only one corner unit is attached to the wall
				//reducing the scale of the bounding box
				gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - (attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.z * 2),gameObject.transform.localScale.y,gameObject.transform.localScale.z);
				//re-positioning the bounding box according to the corner unit
				if(attachedToWall.cornerObjects[0].transform.position.z < gameObject.transform.position.z)
					gameObject.transform.position = new Vector3(gameObject.transform.position.x ,gameObject.transform.position.y,gameObject.transform.position.z + attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.z);
				else
					gameObject.transform.position = new Vector3(gameObject.transform.position.x ,gameObject.transform.position.y,gameObject.transform.position.z - attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.z);
			}

			if (attachedToWall.cornerObjects.Count == 2) { // if two corner units are attached to the wall
				//reducing the scale of the bounding box
				gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - (attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.z * 2) - (attachedToWall.cornerObjects[1].GetComponent<CabinetScript>().boundExtends.z * 2),gameObject.transform.localScale.y,gameObject.transform.localScale.z);
				//since sale is changes bounds also should be updated
				boundExtends = CabinetManager.Instance.BoundExtends (this.gameObject);
				
				if(attachedToWall.cornerObjects[0].transform.position.z < gameObject.transform.position.z)
				{
					gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,attachedToWall.cornerObjects[0].transform.position.z + attachedToWall.cornerObjects[0].GetComponent<CabinetScript>().boundExtends.z + boundExtends.z);
				}
				if(attachedToWall.cornerObjects[1].transform.position.z < gameObject.transform.position.z)
				{
					gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,attachedToWall.cornerObjects[1].transform.position.z + attachedToWall.cornerObjects[1].GetComponent<CabinetScript>().boundExtends.z + boundExtends.z);
				}
			}
		}
	}
}
