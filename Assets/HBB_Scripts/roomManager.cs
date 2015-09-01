using UnityEngine;
using System.Collections;

public class roomManager : MonoBehaviour {
	public float feet_Width;
	public float inch_Width;
	public float feet_Depth;
	public float inch_Depth;

	public GameObject vertA1;
	public GameObject vertA2;
	public GameObject vertB1;
	public GameObject vertB2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Q)) {
			print ("meters : "+inMeters (feet_Width, inch_Width));
			changeDimension ();
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

	void changeDimension ()
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
}
