/***************************
Developer: Ravikiran T.A
Created Date: 09/09/2015
Class Summary: UI manager is responsible for all the UI operations in the application like controlling buttons and panels
***************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	#region UI Buttons
	public GameObject viewModeButton;
	#endregion
	
	#region EventTriggers
	//----- This event is called  to trigger camera manager View Mode functions -----
	public delegate void ViewModeButtonClicked();
	public static event ViewModeButtonClicked EnterViewMode;
	#endregion
	
	#region  Initialization
	void Start () {
		//----- Subscribed to event whenever an object is selected the view mode button will enabl e-----
		ObjectInteractionClient.editModeEntered += EnableViewModeButton;
	}
	#endregion
	
	//====== This function is called from the view mode button to trigger camera manager View Mode functions =====
	public void SetTriggerViewMode(){
		if(EnterViewMode != null)
			EnterViewMode();
		
		//--- Disables the view mode button -----
		DisableViewModeButton();
	}
	
	#region View Mode Button UI Controllers
	//----- Disable the view mode button -----
	void DisableViewModeButton(){
		Debug.Log("Disabling ViewMode");
		viewModeButton.SetActive(false);
	}
	
	//----- Enables the view mode  button -----
	void EnableViewModeButton(){
		viewModeButton.SetActive(true);
	}
	#endregion
}
