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

	#region Input Fields
	public InputField roomWidthInFeet;
	public InputField roomWidthInInches;
	public InputField roomDepthInFeet;
	public InputField roomDepthInInches;
	#endregion
	
	#region EventTriggers
	//----- This event is called  to trigger camera manager View Mode functions -----
	public delegate void ViewModeButtonClicked();
	public static event ViewModeButtonClicked EnterViewMode;

	//----This event is called when the room width or depth fields are changed
	public delegate void RoomSizeChanged(float widthFeet,float widthInch,float depthFeet,float depthInch);
	public static event RoomSizeChanged OnRoomSizeChange;
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


	//======= This function is called when the room width or depth is changed
	public void SetTriggerRoomSizeChanged(){
		float widthFeet = 0f;
		float widthInch = 0f;
		float depthFeet = 0f;
		float depthInch = 0f;
		
		if (roomDepthInFeet.text != null) {
			float.TryParse (roomWidthInFeet.text, out widthFeet);
			Debug.Log ("Changed Width" + widthFeet.ToString ());
		}

		if (roomDepthInInches.text != null) {
			float.TryParse (roomWidthInInches.text, out widthInch);
			Debug.Log ("Changed Width" + widthInch.ToString ());
		}
		

		if (roomWidthInFeet.text != null) {
			float.TryParse (roomDepthInFeet.text, out depthFeet);
			Debug.Log ("Changed Depth" + depthFeet.ToString ());
		}

		if (roomWidthInInches.text != null) {
			float.TryParse (roomDepthInInches.text, out depthInch);
			Debug.Log ("Changed Depth" + depthFeet.ToString ());
		}


//		if (OnRoomSizeChange != null) {
//			OnRoomSizeChange (widthFeet, widthInch, depthFeet, depthInch);
//			Debug.Log("Calling Event");
//		}

		RoomManager.Instance.DimensionInput (widthFeet, widthInch, depthFeet, depthInch);
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
