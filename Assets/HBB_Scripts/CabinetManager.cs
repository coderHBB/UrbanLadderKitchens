/*****************
 * Manish
 * This script maintains all cabinets and its behavior
*****************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CabinetManager : MonoBehaviour {

	//creating a singlton
	private static CabinetManager instance;

	public static CabinetManager Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<CabinetManager>();
			return instance;
		}
	}

	public List<GameObject> cabinetsInScene = new List<GameObject>();
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 BoundExtends (GameObject obj) // returns the mesh bounds of the object in worldSpace
	{
		Vector3 bounds;
		bounds = obj.GetComponent<Renderer>().bounds.extents;
		return bounds;
	}

}
