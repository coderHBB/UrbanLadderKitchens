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

	//vertices of walls
	public GameObject vertA1;
	public GameObject vertA2;
	public GameObject vertB1;
	public GameObject vertB2;

	//Mesh bounds of walls
	public Vector3 boundExtends_verticalWalls;
	public Vector3 boundExtends_horizontalWalls;

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
		print ("in mm : " +inMillimeters (totalFeet));
		return meter;
	}

	float inMillimeters (float totalFeet) //converting to millimeters,because we are getting the assets from UL in mm
	{
		float mm;
		mm = totalFeet * 304.8f; // 1 foot = 304.8
		return mm;
	}

	public void changeDimension ()
	{
		//Width of Kitchen
		float widthOfKitchenFromUser = inMeters (feet_Width, inch_Width);
		float currentWidthOfKitchen = Vector3.Distance (vertA1.transform.position, vertB1.transform.position);
		float alteredWidth = widthOfKitchenFromUser - currentWidthOfKitchen;

		vertA1.transform.position = new Vector3(vertA1.transform.position.x - (alteredWidth)/2,vertA1.transform.position.y,vertA1.transform.position.z);
		vertB1.transform.position = new Vector3(vertB1.transform.position.x + (alteredWidth)/2,vertB1.transform.position.y,vertB1.transform.position.z);

		vertA2.transform.position = new Vector3(vertA2.transform.position.x - (alteredWidth)/2,vertA2.transform.position.y,vertA2.transform.position.z);
		vertB2.transform.position = new Vector3(vertB2.transform.position.x + (alteredWidth)/2,vertB2.transform.position.y,vertB2.transform.position.z);

		//Depth of Kitchen
		float depthOfKitchenFromUser = inMeters (feet_Depth, inch_Depth);
		float currentDepthOfKitchen = Vector3.Distance (vertA1.transform.position, vertA2.transform.position);
		float alteredDepth = depthOfKitchenFromUser - currentDepthOfKitchen;

		vertA1.transform.position = new Vector3(vertA1.transform.position.x,vertA1.transform.position.y,vertA1.transform.position.z - (alteredDepth)/2);
		vertB1.transform.position = new Vector3(vertB1.transform.position.x,vertB1.transform.position.y,vertB1.transform.position.z - (alteredDepth)/2);
		
		vertA2.transform.position = new Vector3(vertA2.transform.position.x ,vertA2.transform.position.y,vertA2.transform.position.z + (alteredDepth)/2);
		vertB2.transform.position = new Vector3(vertB2.transform.position.x,vertB2.transform.position.y,vertB2.transform.position.z + (alteredDepth)/2);

	}

	public void InputFromUserAccepted () // should be called after the input from the user is received
	{
		print ("meters : "+inMeters (feet_Width, inch_Width));
		changeDimension ();
		topWall.wallUpdate ();
		bottomWall.wallUpdate ();
		leftWall.wallUpdate ();
		rightWall.wallUpdate ();

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

	}
}
