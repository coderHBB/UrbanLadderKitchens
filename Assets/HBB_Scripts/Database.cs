using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Database : MonoBehaviour {

	//creating a singleton
	private static Database instance;

	public static Database Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<Database>();
			return instance;
		}
	}

	public List<CabinetScript> cabinetsInDatabase = new List<CabinetScript>();

}
