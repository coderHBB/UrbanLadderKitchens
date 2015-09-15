/*****************
 * Manish
 * This script defines the bounding box attached to walls
 * Addition/Subtraction of cabinets are done here
*****************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoundingBox : MonoBehaviour {

	public WallScript attachedToWall; 	// the wall to which the bounding box is linked or attached to.
	public Vector3 boundExtends;		// bounds extends of the boundingBox
	public float totalSpace;        	// the total space available in the bounding box in millimeters.
	public float occupiedSpace;			// the space occupied by the cabinets in the bounding box (in mm).
	public float availableSpace;		// the space available in the bounding box to place more cabinets (in mm).

	public Vector3 bounds_min;			// minimum bounds of bounding box 
	public Vector3 bounds_max;			// maximum bounds of bounding box

	public List<GameObject> cabinetsInBoundingBox = new List<GameObject>(); // cabinets within this bounding box
	public List<float> listOfCabinetsLength = new List<float>();            // list of sizes of each cabinet


	public float previousLengthOfBoundingBox;  // length of bounding box before altering the dimensions of the kitchen

	public List<GameObject> cabinetsToBeDestroyed = new List<GameObject> (); // This list is used to store cabinets that are to be destroyed while subtracting cabinets


	// position, scale and rotation of bounding box.
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
				//since scale changes, bounds also should be updated
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
				//since scale changes, bounds also should be updated
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

	public void FindCabinetsInsideTheBoundingBox ()     // Finds the cabinets within the bounding box and assigns it to the attachedWall
	{
		bounds_min = gameObject.GetComponent<Renderer> ().bounds.min;
		bounds_max = gameObject.GetComponent<Renderer> ().bounds.max;

		foreach (GameObject g in CabinetManager.Instance.cabinetsInScene) {
			if(g.transform.position.x >= bounds_min.x && g.transform.position.x <= bounds_max.x && g.transform.position.z >= bounds_min.z && g.transform.position.z <= bounds_max.z){
				if(!g.GetComponent<CabinetScript>().isAddedToWall){// to add the cabinet only once to the list
					attachedToWall.listOfCabinetsOnWall.Add (g);
					cabinetsInBoundingBox.Add (g);
					listOfCabinetsLength.Add (g.GetComponent<CabinetScript>().sizeOfCabinet);
					g.GetComponent<CabinetScript>().isAddedToWall = true;
					g.GetComponent<CabinetScript>().attachedToWall = attachedToWall.gameObject;
					print ("attached wall "+attachedToWall.name);
				}


			}
		}
	}

	public void AddSubtractBaseCabinets ()
	{

		Vector3 minBound = Vector3.zero;
		Vector3 maxBound = Vector3.zero;

		totalSpace = gameObject.transform.localScale.x * 1000; // converting to mm
		occupiedSpace = listOfCabinetsLength.Sum ();
		availableSpace = totalSpace - occupiedSpace;



		if (attachedToWall.name == "topWall" || attachedToWall.name == "bottomWall") {
			minBound = new Vector3 (bounds_min.x, gameObject.transform.position.y, gameObject.transform.position.z);
			maxBound = new Vector3 (bounds_max.x, gameObject.transform.position.y, gameObject.transform.position.z);
		}
		if (attachedToWall.name == "leftWall" || attachedToWall.name == "rightWall") {
			minBound = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, bounds_min.z);
			maxBound = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, bounds_max.z);
		}

		if (cabinetsInBoundingBox.Count > 0) {

			if(totalSpace > previousLengthOfBoundingBox){
				AddCabinets (minBound,maxBound);
				print ("Addition on "+attachedToWall.name);
			}
			else if(totalSpace < previousLengthOfBoundingBox){
				SubtractCabinets();
				AddCabinets (minBound,maxBound);
				print ("subtraction on "+attachedToWall.name);
			}
		}
	}

	void AddCabinets (Vector3 boundingBox_min,Vector3 boundingBox_max)
	{
		GameObject closestObject_min;
		closestObject_min = ClosestObjectFrom (boundingBox_min);


		GameObject closestObject_max;
		closestObject_max = ClosestObjectFrom (boundingBox_max);

		GameObject tempObjMin;
		GameObject tempObjMax;

		GameObject suggestedCabinetMin;
		GameObject suggestedCabinetMax;

		float spaceAvailableAlongTheBoundingBox_min = 0;
		float spaceAvailableAlongTheBoundingBox_max = 0;

		if (attachedToWall.name == "topWall" || attachedToWall.name == "bottomWall") {
			spaceAvailableAlongTheBoundingBox_min = Vector3.Distance (boundingBox_min, new Vector3 (closestObject_min.transform.position.x - CabinetManager.Instance.BoundExtends (closestObject_min).x, closestObject_min.transform.position.y, closestObject_min.transform.position.z)) * 1000; // converting to mm
			spaceAvailableAlongTheBoundingBox_max = Vector3.Distance (boundingBox_max, new Vector3 (closestObject_max.transform.position.x + CabinetManager.Instance.BoundExtends (closestObject_max).x, closestObject_max.transform.position.y, closestObject_max.transform.position.z)) * 1000; // converting to mm
		}

		if (attachedToWall.name == "leftWall" || attachedToWall.name == "rightWall") {
			spaceAvailableAlongTheBoundingBox_min = Vector3.Distance (boundingBox_min, new Vector3 (closestObject_min.transform.position.x, closestObject_min.transform.position.y, closestObject_min.transform.position.z - CabinetManager.Instance.BoundExtends (closestObject_min).z)) * 1000; // converting to mm
			spaceAvailableAlongTheBoundingBox_max = Vector3.Distance (boundingBox_max, new Vector3 (closestObject_max.transform.position.x, closestObject_max.transform.position.y, closestObject_max.transform.position.z + CabinetManager.Instance.BoundExtends (closestObject_max).z)) * 1000; // converting to mm

		}
		//along the minimum side
		if (spaceAvailableAlongTheBoundingBox_min >= 400) {
			suggestedCabinetMin = SuggestCabinetFromDataBase (spaceAvailableAlongTheBoundingBox_min);

			tempObjMin = Instantiate (suggestedCabinetMin);

			if(attachedToWall.name == "topWall")
				tempObjMin.transform.Rotate(0,180,0);
			if(attachedToWall.name == "bottomWall")
				tempObjMin.transform.Rotate(0,0,0);
			if(attachedToWall.name == "leftWall")
				tempObjMin.transform.Rotate(0,90,0);
			if(attachedToWall.name == "rightWall")
				tempObjMin.transform.Rotate(0,270,0);

			tempObjMin.GetComponent<CabinetScript>().boundExtends = CabinetManager.Instance.BoundExtends (tempObjMin);

			if(attachedToWall.name == "topWall" || attachedToWall.name == "bottomWall")
				tempObjMin.transform.position = new Vector3 (closestObject_min.transform.position.x - CabinetManager.Instance.BoundExtends (closestObject_min).x - CabinetManager.Instance.BoundExtends (tempObjMin).x, closestObject_min.transform.position.y, closestObject_min.transform.position.z);              
			if(attachedToWall.name == "leftWall" || attachedToWall.name == "rightWall")
				tempObjMin.transform.position = new Vector3 (closestObject_min.transform.position.x , closestObject_min.transform.position.y, closestObject_min.transform.position.z - CabinetManager.Instance.BoundExtends (closestObject_min).z - CabinetManager.Instance.BoundExtends (tempObjMin).z);              


			tempObjMin.transform.parent = CabinetManager.Instance.cabinetHolder.transform; // parenting
			
			tempObjMin.GetComponent<CabinetScript> ().attachedToWall = attachedToWall.gameObject;
			
			cabinetsInBoundingBox.Add (tempObjMin);
			listOfCabinetsLength.Add (tempObjMin.GetComponent<CabinetScript> ().sizeOfCabinet);
			attachedToWall.listOfCabinetsOnWall.Add (tempObjMin);
			tempObjMin.GetComponent<CabinetScript> ().isAddedToWall = true;

//			print ("spaceAvailableAlongTheBoundingBox_min : "+spaceAvailableAlongTheBoundingBox_min);
//			print ("closest object min : " +closestObject_min.name);

		} 
//		else
//			print ("Add filler in min side");

		if (spaceAvailableAlongTheBoundingBox_max >= 400) {
			suggestedCabinetMax = SuggestCabinetFromDataBase (spaceAvailableAlongTheBoundingBox_max);
			tempObjMax = Instantiate (suggestedCabinetMax);

			if(attachedToWall.name == "topWall")
				tempObjMax.transform.Rotate(0,180,0);
			if(attachedToWall.name == "bottomWall")
				tempObjMax.transform.Rotate(0,0,0);
			if(attachedToWall.name == "leftWall")
				tempObjMax.transform.Rotate(0,90,0);
			if(attachedToWall.name == "rightWall")
				tempObjMax.transform.Rotate(0,270,0);

			tempObjMax.GetComponent<CabinetScript>().boundExtends = CabinetManager.Instance.BoundExtends (tempObjMax);


			if(attachedToWall.name == "topWall" || attachedToWall.name == "bottomWall")
				tempObjMax.transform.position = new Vector3 (closestObject_max.transform.position.x + CabinetManager.Instance.BoundExtends (closestObject_max).x + CabinetManager.Instance.BoundExtends (tempObjMax).x, closestObject_max.transform.position.y, closestObject_max.transform.position.z);              
			if(attachedToWall.name == "leftWall" || attachedToWall.name == "rightWall")
				tempObjMax.transform.position = new Vector3 (closestObject_max.transform.position.x, closestObject_max.transform.position.y, closestObject_max.transform.position.z  + CabinetManager.Instance.BoundExtends (closestObject_max).z + CabinetManager.Instance.BoundExtends (tempObjMax).z);              

			tempObjMax.transform.parent = CabinetManager.Instance.cabinetHolder.transform; // parenting

			tempObjMax.GetComponent<CabinetScript> ().attachedToWall = attachedToWall.gameObject;

			cabinetsInBoundingBox.Add (tempObjMax);
			listOfCabinetsLength.Add (tempObjMax.GetComponent<CabinetScript> ().sizeOfCabinet);
			attachedToWall.listOfCabinetsOnWall.Add (tempObjMax);
			tempObjMax.GetComponent<CabinetScript> ().isAddedToWall = true;
		} 
//		else
//			print ("Add filler in max side");

		if(spaceAvailableAlongTheBoundingBox_max >= 400 || spaceAvailableAlongTheBoundingBox_min >= 400)
			AddCabinets (boundingBox_min, boundingBox_max);
	}

	public void SubtractCabinets ()
	{

		foreach (GameObject cabinet in cabinetsInBoundingBox) {

			if(attachedToWall.name == "topWall" || attachedToWall.name == "bottomWall"){
				if((cabinet.transform.position.x - cabinet.GetComponent<CabinetScript>().boundExtends.x) < bounds_min.x || (cabinet.transform.position.x + cabinet.GetComponent<CabinetScript>().boundExtends.x) > bounds_max.x)
				{
					cabinetsToBeDestroyed.Add (cabinet);
				}
			}

			if(attachedToWall.name == "leftWall" || attachedToWall.name == "rightWall"){
				if((cabinet.transform.position.z - cabinet.GetComponent<CabinetScript>().boundExtends.z) < bounds_min.z || (cabinet.transform.position.z + cabinet.GetComponent<CabinetScript>().boundExtends.z) > bounds_max.z)
				{
					cabinetsToBeDestroyed.Add (cabinet);
				}
			}

		}

		if(cabinetsToBeDestroyed.Count > 0)
		foreach (GameObject cabinet in cabinetsToBeDestroyed) {
			CabinetManager.Instance.cabinetsInScene.Remove (cabinet);
			cabinetsInBoundingBox.Remove (cabinet);
			listOfCabinetsLength.Remove (cabinet.GetComponent<CabinetScript>().sizeOfCabinet);
			attachedToWall.listOfCabinetsOnWall.Remove (cabinet);
			Destroy (cabinet);
		}

		cabinetsToBeDestroyed.Clear ();

	}

	GameObject ClosestObjectFrom (Vector3 point)
	{
		GameObject closestObject;
		float distance;
		float minimumDistance;

		minimumDistance = Vector3.Distance (point, cabinetsInBoundingBox [0].transform.position);
		closestObject = cabinetsInBoundingBox [0];

		foreach (GameObject cabinet in cabinetsInBoundingBox) {

			distance = Vector3.Distance (point, cabinet.transform.position);

			if(distance <= minimumDistance)
			{
				minimumDistance = distance;
				closestObject = cabinet;
			}

		}
		return closestObject;
	}

	GameObject SuggestCabinetFromDataBase (float size)
	{
		//To suggest an object that is not beyond the maximumSwapSize (600 is standard)
		if (size > CabinetManager.Instance.maximumSwapSize)
			size = CabinetManager.Instance.maximumSwapSize;

		GameObject suggestedCabinet;
		float minimumDifference;
		float difference;

		minimumDifference = Mathf.Abs (size - Database.Instance.cabinetsInDatabase [0].sizeOfCabinet);
		suggestedCabinet = Database.Instance.cabinetsInDatabase [0].gameObject;

		foreach (CabinetScript cabinet in Database.Instance.cabinetsInDatabase) {
			difference = Mathf.Abs(size - cabinet.sizeOfCabinet);
			if(difference < minimumDifference)
			{
				if(size >= cabinet.sizeOfCabinet){
					minimumDifference = difference;
					suggestedCabinet = cabinet.gameObject;
				}
			}
		}

		return suggestedCabinet;

	}
}
