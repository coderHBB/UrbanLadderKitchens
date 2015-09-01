using UnityEngine;
using System.Collections;

public class wallScript : MonoBehaviour {

	public GameObject initialVertex;  // initialPoint of the wall
	public GameObject finalVertex;	  // finalPoint of the wall
	public float lengthOfWall;        // The length of the wall, from initialVertex to FinalVertex.

	//to calculate angle of rotation
	Vector3 thirdPoint;
	float hyp;
	float oppositeSide;
	float angle;
	/// 

	public Vector3 boundExtends; // Vector that stores the mesh bounds of the wall
	Vector3 wallPosition;        // position of the wall
	Vector3 alteredPosition;      // altered position of wall after

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		if(Input.GetKeyDown (KeyCode.Space))
			wallUpdate();
	}

	void bounds () //mesh bounds of the wall
	{
		boundExtends = gameObject.GetComponent<Renderer>().bounds.extents;
	}

	void wallUpdate ()
	{

		wallPosition = (initialVertex.transform.position + finalVertex.transform.position)/2; //positioning the wall

		// keeps the wall edges without overlapping each other
		if(gameObject.name == "topWall")
			alteredPosition = new Vector3 (wallPosition.x, -0.5f, wallPosition.z + boundExtends.z);
		if(gameObject.name == "bottomWall")
			alteredPosition = new Vector3 (wallPosition.x, -0.5f, wallPosition.z - boundExtends.z);
		if(gameObject.name == "leftWall")
			alteredPosition = new Vector3 (wallPosition.x - boundExtends.x, -0.5f, wallPosition.z);
		if(gameObject.name == "rightWall")
			alteredPosition = new Vector3 (wallPosition.x + boundExtends.x, -0.5f, wallPosition.z);
		///
		gameObject.transform.position = alteredPosition; // assigning position

//		gameObject.transform.position = (initialVertex.transform.position + finalVertex.transform.position)/2; //positioning the wall
		lengthOfWall = Vector3.Distance (initialVertex.transform.position,finalVertex.transform.position); // calculates the length of the wall
		gameObject.transform.localScale = new Vector3(lengthOfWall,2.4384f,0.1f);  // scaling the wall

		//calculate angle of wall
		thirdPoint = new Vector2 (finalVertex.transform.position.x, initialVertex.transform.position.z);
		hyp = Vector2.Distance (new Vector3 (initialVertex.transform.position.x, initialVertex.transform.position.z), new Vector3 (finalVertex.transform.position.x, finalVertex.transform.position.z));
		oppositeSide = Vector2.Distance (thirdPoint, new Vector3 (finalVertex.transform.position.x, finalVertex.transform.position.z));
		if (oppositeSide != hyp)
			angle = Mathf.Asin (oppositeSide / hyp) * 57.3f;
		else
			angle = 90;

		if ((initialVertex.transform.position.x > finalVertex.transform.position.x && initialVertex.transform.position.z < finalVertex.transform.position.z) ||
		    (initialVertex.transform.position.z > finalVertex.transform.position.z && initialVertex.transform.position.x < finalVertex.transform.position.x)){
			gameObject.transform.rotation = Quaternion.Euler (0, angle, 0);
			
		}
		else{
			gameObject.transform.rotation = Quaternion.Euler (0, -angle, 0); // inverts the wall
			
		}

		bounds ();
	}
}
