﻿/***************************
Developer: Ravikiran T.A
Created Date: 07/09/2015
Class Summary: Object interactions is responsible for raising events for different types of interactions with objects.
***************************/

using UnityEngine;
using System.Collections;

//===== Serialized class for helping with keeping track of the interaction variables =====
#region InteractionsClass
[System.Serializable]
public class Interactions{
	public bool interactable;
	public enum interactionTypes{
		Unselected,
		Selected
	}
	public interactionTypes interaction;
	
	public void SetSelected(){
		if(interactable)
			interaction = interactionTypes.Selected;
	}
	
	public void SetUnSelected(){
		interaction = interactionTypes.Unselected;
	}
};
#endregion

#region ObjectInteractionsClass
public class ObjectInteractionClient : MonoBehaviour {
	
	#region Variables
	public Interactions objectInteraction;
	
	//------ Used to trigger events whenever an object is selected -----
	public delegate void ObjectSelection(GameObject selectedObject);
	public static event ObjectSelection objectSelected;
	
	//---- Used to trigger events whenever an object is selected -----
	public delegate void EnterEditMode();
	public static event EnterEditMode editModeEntered;
	#endregion
	
	void Start () {
		
	}
	
	#region ObjectSelected
	//===== Called when an mouse clicks on an object with this script =====
	public void OnMouseDown(){
		Selected();
	}
	
	//===== Called when mouse clicks =====
	void Selected(){
		
		Debug.Log("Selected:" + gameObject.name);
		
		//----- Set the object status to selected -----
		objectInteraction.SetSelected();
		
		//----- Call the object selected trigger -----
		if(objectSelected != null)
			objectSelected(gameObject);
			
		//----- Call the edit mode entered trigger -----
		if(editModeEntered != null)
			editModeEntered();
	}
	#endregion
}
#endregion