/***************************
Developer: Ravikiran T.A
Created Date: 07/09/2015
Class Summary: Object interactions is responsible for raising events for different types of interactions with objects. This script should be attached to all interactible objects in the scene.
***************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//===== Serialized class for helping with keeping track of the interaction variables =====
#region InteractionsClass
[System.Serializable]
public class Interactions{
	public bool interactable = true;
	
	public bool selectedOnMouseDown;
	
	public enum interactionTypes{
		Unselected,
		Selected
	}
	public interactionTypes interaction;
	
	//---------- Set object value as selected ----------
	public void SetSelected(){
		if(interactable)
			interaction = interactionTypes.Selected;
	}
	
	//---------- Set object value as unselected --------
	public void SetUnSelected(){
		interaction = interactionTypes.Unselected;
	}
};
#endregion

#region ObjectInteractionsClass

[RequireComponent (typeof (Collider))]
public class ObjectInteractionClient : MonoBehaviour {
	
	#region Variables
	public Interactions objectSelectionStatus;
	public Collider selfCollider;
	
	[Header("Is this Object an internal?")]
	public bool internals;
	
	[Header("Child Colliders")]
	public Collider[] childColliders;
	
	[Header("CabinetType")]
	public CabinetScript.TypeOfCabinet selectedObjectType;
	
	
	//------ Used to trigger events whenever an object is selected -----
	public delegate void ObjectSelection(GameObject selectedObject,CabinetScript.TypeOfCabinet objectType);
	public static event ObjectSelection objectSelected;
	
	//---- Used to trigger events whenever an object is selected -----
	public delegate void EnterEditMode();
	public static event EnterEditMode editModeEntered;
	#endregion
	
	void Start () {
		
		//----------- Retrieving selected cabinet type to pass along with object selected event ----------	
		selectedObjectType = GetComponent<CabinetScript>()._typeOfCabinet;
		
		//---------- Retrieving child colliders of internals -------------
		if(!internals){
			childColliders = GetComponentsInChildren<Collider>();
			selfCollider = gameObject.GetComponent<BoxCollider>();
		}
	}
	
	public void OnEnable(){
		//----- Object selection event should to set object deselected for previously selected object ----
		objectSelected += UnSelected;
		
		//---- All object colliders and seleciton status will get reset when the view mode button is pressed ----
		UIManager.EnterViewMode += ResetObjectColliders;	
	}
	
	#region ObjectSelected
	//===== Called when an mouse clicks on an object with this script =====
	public void OnMouseDown(){
		objectSelectionStatus.selectedOnMouseDown = true;
	}
	
	public void OnMouseUpAsButton(){
		if(objectSelectionStatus.interactable && objectSelectionStatus.selectedOnMouseDown)
			Selected();
	}
	
	void OnMouseDrag(){
		
	}
	
	//============ Used to reset colliders on parent and children objects and selection statuses ===========
	void ResetObjectColliders(){
		if(!internals){
			SwitchOnSelfColliders();
			DeactivateCollidersInChildren();
		}
		
		objectSelectionStatus.SetUnSelected();
		objectSelectionStatus.selectedOnMouseDown = false;
	
	}
	
	//===== Called when mouse clicks =====
	void Selected(){
		
		Debug.Log("Selected:" + gameObject.name);
		
		//----- Set the object status to selected -----
		objectSelectionStatus.SetSelected();
		
		//----- Call the object selected trigger -----
		if(objectSelected != null)
			objectSelected(gameObject,selectedObjectType);
			
		//----- Call the edit mode entered trigger -----
		if(editModeEntered != null)
			editModeEntered();
		
		//------ To be executed only for non internals --------
		if(!internals){
			SwitchOffSelfColliders();
		
			ActivateCollidersInChildren();
		}
	}
	
	
	//====== Switch off colliders on cabinet
	void SwitchOffSelfColliders(){
		Debug.Log("Switching Off Colliders");
		if(childColliders.Length > 1)
			selfCollider.enabled = false;
	}
	
	void SwitchOnSelfColliders(){
		Debug.Log("Switching On Colliders");
		selfCollider.enabled = true;
	}
	
	void ActivateCollidersInChildren(){
		for(int i = 1; i < childColliders.Length; i ++) {
			childColliders[i].enabled = true;
		}
	}
	
	void DeactivateCollidersInChildren(){
		for(int i = 1; i < childColliders.Length; i ++) {
			childColliders[i].enabled = false;
		}
	}
	#endregion
	
	#region ObjectUnSelected
	//=========== Called when any object in the scene is selected --- All objects will call this function when any kind of selection event occus. ============
	public void UnSelected(GameObject selectedObject,CabinetScript.TypeOfCabinet objectType){
			
		//---------- Set this object to unselected only if this is not the selected object and it was selected before -------
		if(gameObject != selectedObject && objectSelectionStatus.interaction == Interactions.interactionTypes.Selected){
			objectSelectionStatus.SetUnSelected();
			
			//------ To be executed only for non internals -------- Internals colliders are controlled by parent cabinet --------
			if(!internals){
				SwitchOnSelfColliders();
			
				DeactivateCollidersInChildren();
			}
		}
	}
	#endregion
}
#endregion