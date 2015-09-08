/*****************
 * Manish
 * This script is responsible for the Kitchens Dimensions and unit conversions
*****************/

using UnityEngine;
using System.Collections;

public class RoomManager : MonoBehaviour {


	//creating a singlton
	private static RoomManager instance;
	
	public static RoomManager Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<RoomManager>();
			return instance;
		}
	}

	public float feet_Width;
	public float inch_Width;
	public float feet_Depth;
	public float inch_Depth;

	public WallScript topWall;
	public WallScript bottomWall;
	public WallScript leftWall;
	public WallScript rightWall;

	public bool isHobAlongTheWidth; // isHobAlongTheWidth is True, if the hob is along the "topWall" or the "bottomWall.. isHobAlongTheWidth is False, if the hob is along the "leftWall" or the "rightWall

	//vertices of walls
	public GameObject vertA1;
	public GameObject vertA2;
	public GameObject vertB1;
	public GameObject vertB2;

	//Mesh bounds of walls
	public Vector3 boundExtends_verticalWalls;
	public Vector3 boundExtends_horizontalWalls;

	//To find the hobWall ratio
	[Header("To find the hobWall ratio")]
	public WallScript hobOnWall;
	public float distanceFromInitailVertToHob;
	public float distanceFromFinalVertToHob;

	// Use this for initialization
	void Start () {
		InputFromUserAccepted ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Q)) {
			InputFromUserAccepted ();
		}
	}

	float inMeters (float inFeet,float inInches) //converting feet to meters (1 unity unit = 1 meter) to maintain size proportion
	{
		float totalFeet;
		float meter;
		totalFeet = inFeet + (inInches * 0.0833333f); // 1 inch = 0.0833333
		meter = totalFeet * 0.3048f;
//		print ("in mm : " +inMillimeters (totalFeet));
		return meter;
	}

	float inMillimeters (float totalFeet) //converting to millimeters,because we are getting the assets from UL in mm
	{
		float mm;
		mm = totalFeet * 304.8f; // 1 foot = 304.8
		return mm;
	}

	public void ChangeDimension () // Changes the dimensions of the kitchen
	{
		//Width of Kitchen
		float widthOfKitchenFromUser = inMeters (feet_Width, inch_Width);
		float currentWidthOfKitchen = Vector3.Distance (vertA1.transform.position, vertB1.transform.position);
		float alteredWidth = widthOfKitchenFromUser - currentWidthOfKitchen;
		print ("alteredWidth : " + alteredWidth);
	
		if (isHobAlongTheWidth) {
			print ("percentage initial : " + Percentage (distanceFromInitailVertToHob, currentWidthOfKitchen));
			print ("percentage final : " + Percentage (distanceFromFinalVertToHob, currentWidthOfKitchen));
			print ("tobeAddedFromInitial : " + WallLengthToBeChanged (Percentage (distanceFromInitailVertToHob, currentWidthOfKitchen), alteredWidth));
			print ("tobeAddedFromFinal : " + WallLengthToBeChanged (Percentage (distanceFromFinalVertToHob, currentWidthOfKitchen), alteredWidth));

			MoveWallVertex_width (WallLengthToBeChanged (Percentage (distanceFromInitailVertToHob, currentWidthOfKitchen), alteredWidth), WallLengthToBeChanged (Percentage (distanceFromFinalVertToHob, currentWidthOfKitchen), alteredWidth));
		} else
			MoveWallVertex_width ((alteredWidth / 2), (alteredWidth / 2));

//		vertA1.transform.position = new Vector3(vertA1.transform.position.x - (alteredWidth)/2,vertA1.transform.position.y,vertA1.transform.position.z);
//		vertB1.transform.position = new Vector3(vertB1.transform.position.x + (alteredWidth)/2,vertB1.transform.position.y,vertB1.transform.position.z);
//
//		vertA2.transform.position = new Vector3(vertA2.transform.position.x - (alteredWidth)/2,vertA2.transform.position.y,vertA2.transform.position.z);
//		vertB2.transform.position = new Vector3(vertB2.transform.position.x + (alteredWidth)/2,vertB2.transform.position.y,vertB2.transform.position.z);

		//Depth of Kitchen
		float depthOfKitchenFromUser = inMeters (feet_Depth, inch_Depth);
		float currentDepthOfKitchen = Vector3.Distance (vertA1.transform.position, vertA2.transform.position);
		float alteredDepth = depthOfKitchenFromUser - currentDepthOfKitchen;

		vertA1.transform.position = new Vector3(vertA1.transform.position.x,vertA1.transform.position.y,vertA1.transform.position.z - (alteredDepth)/2);
		vertB1.transform.position = new Vector3(vertB1.transform.position.x,vertB1.transform.position.y,vertB1.transform.position.z - (alteredDepth)/2);
		
		vertA2.transform.position = new Vector3(vertA2.transform.position.x ,vertA2.transform.position.y,vertA2.transform.position.z + (alteredDepth)/2);
		vertB2.transform.position = new Vector3(vertB2.transform.position.x,vertB2.transform.position.y,vertB2.transform.position.z + (alteredDepth)/2);

	}

	void MoveWallVertex_width (float toBeAddedOnA1A2, float toBeAddedOnB1B2) // Moves the vertices of the walls based on the parameters passed
	{
		vertA1.transform.position = new Vector3(vertA1.transform.position.x - toBeAddedOnA1A2,vertA1.transform.position.y,vertA1.transform.position.z);
		vertB1.transform.position = new Vector3(vertB1.transform.position.x + toBeAddedOnB1B2,vertB1.transform.position.y,vertB1.transform.position.z);
		
		vertA2.transform.position = new Vector3(vertA2.transform.position.x - toBeAddedOnA1A2,vertA2.transform.position.y,vertA2.transform.position.z);
		vertB2.transform.position = new Vector3(vertB2.transform.position.x + toBeAddedOnB1B2,vertB2.transform.position.y,vertB2.transform.position.z);
	}

	public void InputFromUserAccepted () // should be called after the input from the user is received
	{
//		print ("meters : "+inMeters (feet_Width, inch_Width));
		ChangeDimension ();
		topWall.WallUpdate ();
		bottomWall.WallUpdate ();
		leftWall.WallUpdate ();
		rightWall.WallUpdate ();

		//Assigning the positions of te corner unit
		foreach (GameObject cabinet in CabinetManager.Instance.cabinetsInScene) {
			if(cabinet.GetComponent<CabinetScript>()._typeOfCabinet == CabinetScript.TypeOfCabinet.CornerCabinet)
				cabinet.GetComponent<CabinetScript>().Positioning ();
		}

		//Assinging the positions, scale and rotation of the bounding box
		topWall.boundingBox.PositionScaleRotation ();
		bottomWall.boundingBox.PositionScaleRotation ();
		leftWall.boundingBox.PositionScaleRotation ();
		rightWall.boundingBox.PositionScaleRotation ();

		//Finding the cabinets within the boundingBox and assigning it to its respective walls
		topWall.boundingBox.FindCabinetsInsideTheBoundingBox ();
		bottomWall.boundingBox.FindCabinetsInsideTheBoundingBox ();
		leftWall.boundingBox.FindCabinetsInsideTheBoundingBox ();
		rightWall.boundingBox.FindCabinetsInsideTheBoundingBox ();

		//Assinging the positions of the cabinets (except corner cabinet)
		foreach (GameObject cabinet in CabinetManager.Instance.cabinetsInScene) {
			if(cabinet.GetComponent<CabinetScript>()._typeOfCabinet != CabinetScript.TypeOfCabinet.CornerCabinet)
				cabinet.GetComponent<CabinetScript>().Positioning ();
		}
		
		HobWall ();

	}

	void HobWall () // To find which wall is the hob attached to.
	{
		Vector3 hobWallPoint;

		foreach (GameObject cabinet in CabinetManager.Instance.cabinetsInScene) {
			if(cabinet.GetComponent<CabinetScript>()._typeOfCabinet == CabinetScript.TypeOfCabinet.Hob)
			{
				hobOnWall = cabinet.GetComponent<CabinetScript>().attachedToWall.GetComponent<WallScript>();
//				print ("the hob is atached to "+hobOnWall.name);

				if(hobOnWall.name == "topWall")
				{
					hobWallPoint = new Vector3(cabinet.transform.position.x,cabinet.transform.position.y,cabinet.transform.position.z + cabinet.GetComponent<CabinetScript>().boundExtends.z);
					distanceFromInitailVertToHob = Vector3.Distance (hobOnWall.initialVertex.transform.position,hobWallPoint);
					distanceFromFinalVertToHob = Vector3.Distance (hobOnWall.finalVertex.transform.position,hobWallPoint);
					isHobAlongTheWidth = true;
//					print ("initToHob : "+distanceFromInitailVertToHob+" finalToHob : "+distanceFromFinalVertToHob);
				}

				//do similarly for bottomWall,leftWall,rightWall
			}
		}
	}

	float Percentage (float value, float totalLength) // to find percentage of wall on either side of the hob
	{
		float _percentage;
		_percentage = (value * 100) / totalLength;
		return _percentage;
	}

	float WallLengthToBeChanged (float percentage, float differenceInLength) // The wall length to be altered from the hob
	{
		float _valueToBeAdded;
		_valueToBeAdded = (percentage * differenceInLength) / 100;
		return _valueToBeAdded;
	}
}
