/***************************
Developer: Ravikiran T.A
Date: 07/09/2015
Class Summary: Object interactions is responsible for raising events for different types of interactions done to this object.
***************************/

using UnityEngine;
using System.Collections;

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
};
#endregion

//#region EventTriggersClass
//[System.Serializable]
//public class EventTriggers{
//	public delegate void ObjectSelection();
//	public event ObjectSelection objectSelected;
//};
//#endregion


#region ObjectInteractionsClass
public class ObjectInteractionClient : MonoBehaviour {
	
	#region Variables
	public Interactions objectInteraction;
	
	public delegate void ObjectSelection(GameObject selectedObject);
	public static event ObjectSelection objectSelected;
	#endregion
	
	void Start () {
		
	}
	
	void Update () {
		
	}
	
	#region ObjectSelected
	//Called when an mouse clicks on an object with this script
	public void OnMouseDown(){
		//Debug.Log("Clicked");
		Selected();
	}
	
	//Called when mouse clicks
	void Selected(){
		Debug.Log("Selected:" + gameObject.name);
		objectInteraction.SetSelected();
		objectSelected(gameObject);
	}
	#endregion
}
#endregion