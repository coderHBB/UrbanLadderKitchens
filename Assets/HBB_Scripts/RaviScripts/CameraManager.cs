/***************************
Developer: Ravikiran T.A
Created Date: 07/09/2015
Class Summary: Camera manager is responsible for camera animations and other camera operations in the application.
***************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ObjectSpecificValues{
	//public string name;
	public bool autoFitCameraToBaseCabinetSize;
	[Tooltip("How far in front of the zoomed object should the camera stop")]
	[Range(0.5f,10f)]public float distanceFromObjectOnZoom = 3f;
	
	[Tooltip("How high in front of the zoomed object should the camera stop")]
	[Range(0.1f,2f)]public float cameraHeightWithObjectOnZoom = 0.3f;
	
	[Tooltip("What angle should the camera view rotate w.r.t y axis when selecting object.")]
	[Range(-180f,180f)]public float horizontalViewAngleOnObjectSelection = 180;
	
	[Tooltip("What angle should the camera view rotate w.r.t x axis when selecting object.")]
	[Range(-180f,180f)]public float verticalViewAngleOnObjectSelection = 0f;
	
	[Tooltip("Use manual or object size based values")]	
	[Range(-180f,5f)]public float maxZoomLimitOnObject = 0.9f;
};


public class CameraManager : MonoBehaviour {
	
	#region Singleton
	private static CameraManager instance;
	public static CameraManager Instance{
		get{
			if(instance == null)
				instance = GameObject.FindObjectOfType<CameraManager>();
			
			return instance;
		}
	}
	#endregion
	
	#region Variables
	
	public enum cameraStatus{
		ViewMode,
		EditMode	
	};
	
	//-------------------------------------- Camera Values -------------------------------------------------------------------------------------------------
	[Header("CameraStatus")]
	public cameraStatus camStatus;
	
	public float cameraFOV = 60f;
	[Tooltip("This value is used internally as Mathf.Tan(Mathf.Deg2Rad * CameraFOV / 2f)")]
	public float cameraFOVBasedValue;
	
	//-------------------------------------- Specific Camera Values ----------------------------------------------------------------------------------------
	[Header("View Values For Differnt Cabinets/Units")]
	
	//-------- Values specific to Base cabinet -------------
	public ObjectSpecificValues baseCabinet;
	
	//-------- Values specific to Wall cabinet -------------
	public ObjectSpecificValues wallCabinet;
	
	//-------- Values specific to Corner cabinet -------------
	public ObjectSpecificValues cornerCabinet;
	
	//-------- Values specific to Oil pullout -------------
	public ObjectSpecificValues oilPullout;
	
	//-------- Values specific to Hobs -------------
	public ObjectSpecificValues hob;
	
	[Header("Internals")]
	//-------- Values specific to Hobs -------------
	public ObjectSpecificValues drawers;

	
	//--------------------------------------------- General -----------------------------------------------------------------------------------------------
	[Header("General")]
	[Tooltip("Initial Height of the camera from the floor in view mode")]
	[Range(0f,3f)]public float initialCameraHeight = 1.8288f;
	
	[Header("Camera Animation Speeds")]
	[Tooltip("Time taken to finish camera focus on object")]
	[Range(0.1f,3f)]public float cameraFocusOnObjectSpeed = 1.5f;
	
	[Tooltip("Time taken by camera to return to view mode")]
	[Range(0.1f,3f)]public float cameraResetToViewModeSpeed = 1.5f;
	
	[Tooltip("Time taken by camera to animate to new distance when room size changes")]
	[Range(0.1f,3f)]public float cameraReadjustmentSpeed = 1.5f;
	//--------------------------------------------- Game Object References --------------------------------------------------------------------------------
	[Header("Gameobject Refernces")]
	[Tooltip("Attach here the camera that is to be used for animation")]
	public GameObject animationCamera;
	
	public TBDragOrbit TBDragOrbitScript;
	
	[Tooltip("Attach here the gameobject that is to be used by Tb Drag Orbit Script")]
	public GameObject targetObject;
	
	[Tooltip("Original Rotation Values before going to Edit Mode")]
	public Vector3 originalRotation;
	
	[Tooltip("Original Position Values before going to Edit Mode")]
	public Vector3 originalPosition;
	
	#endregion
	
	
	#region Events
	public delegate void EnterViewMode(float returnTime);
	
	//----- Used by camera manager to re enable camera rotation in TB Drag Orbit Script-----time parameter tells how long to wait for moving animation to finish----
	public static event EnterViewMode enableCameraRotation;
	#endregion
	
	#region Initialization
	void Awake(){
		//----- Event that will be triggered whenever an object with event trigger is selected
		ObjectInteractionClient.objectSelected += FocusCameraOnObject;
		UIManager.EnterViewMode += GoToViewMode;	
	}
	
	void Start () {
		Debug.Log("Getting Camera FOV");
		
		//-------Takes the camera FOV and calculates derived values to later use in camera distance calculation-----
		cameraFOV = animationCamera.GetComponent<Camera>().fieldOfView;
		cameraFOVBasedValue = Mathf.Tan(Mathf.Deg2Rad * cameraFOV / 2f);
		originalRotation = animationCamera.transform.rotation.eulerAngles;
		
		TBDragOrbitScript = animationCamera.GetComponent<TBDragOrbit>();
		
		//-------------- Saving camera position ---------------
		StoreOriginalPosition(animationCamera.transform.position);
		
		//-------------- Saving camera rotation ---------------
		StoreOriginalRotation();
	}
	#endregion
	
	#region FocusCameraOnObject
	//===== Used for direct call or focusing the camera on objects =====
	public void FocusCameraOnObject(GameObject selectedObject,CabinetScript.TypeOfCabinet objectType) {
		
		//-------------- Saving camera position ---------------
		//StoreOriginalPosition(animationCamera.transform.position);
		
		//-------------- Saving camera rotation ---------------
		//StoreOriginalRotation();
		
		
		//---------- Calculate new camera position ----------
		Vector3 position = PositionCamera(selectedObject,objectType);
		
		//----------- Tween to new position -----------
		iTween.MoveTo(animationCamera,position,cameraFocusOnObjectSpeed);
		
		//----------- Calculate new camera rotation -------------------
		Vector3 rotation = RotateCamera(selectedObject,objectType);
		
		//------------- Tween new rotation ------------
		iTween.RotateTo(animationCamera,rotation,cameraFocusOnObjectSpeed);
		
		//----Change Camera status to Edit Mode----It is used as check before re-enabling tb orbit script----
		camStatus = cameraStatus.EditMode;
	}
	
	
	//=========== Function to calculate camera position on selecting different cabinet types ===============
	Vector3 PositionCamera(GameObject selectedObject,CabinetScript.TypeOfCabinet type) {
		
		Vector3 position = new Vector3();
		
		//-------------- Switch will help choose what position values should be used when object is selected ----------
		switch(type){
			//-------------- BASE CABINET -----------------
			case CabinetScript.TypeOfCabinet.BaseCabinet:{
				if(baseCabinet.autoFitCameraToBaseCabinetSize){
					//----------- Set distance from the fron of the cabinet -----
					float sizeOfObject = Mathf.Max(selectedObject.GetComponent<Collider>().bounds.extents.x,selectedObject.GetComponent<Collider>().bounds.extents.y * baseCabinet.maxZoomLimitOnObject);
					float distance = sizeOfObject / cameraFOVBasedValue;
					Debug.Log("Distance Z:" + distance);
					Debug.Log("Object Z:" + selectedObject.transform.position.z);
					position = selectedObject.transform.position + selectedObject.transform.forward * distance * 1.8f;
					
					//-------- Set the height of the camera ------
					position = position + new Vector3(0f,baseCabinet.cameraHeightWithObjectOnZoom,0f);
					Debug.Log("Position Z:" + position.z);
				}
				//--------- Else use manual fit -----------
				else{
					position = selectedObject.transform.position + selectedObject.transform.forward * baseCabinet.distanceFromObjectOnZoom;
					position = position + new Vector3(0f,baseCabinet.cameraHeightWithObjectOnZoom,0f);
				}
				break;
			}
		
			//-------------- WALL CABINET -----------------
			case CabinetScript.TypeOfCabinet.WallCabinets:{
				//---- IF Auto Fit -----------------
				if(wallCabinet.autoFitCameraToBaseCabinetSize){
					//----------- Set distance from the fron of the object -------
					float sizeOfObject = Mathf.Max(selectedObject.GetComponent<Collider>().bounds.extents.x,selectedObject.GetComponent<Collider>().bounds.extents.y * wallCabinet.maxZoomLimitOnObject);
					float distance = sizeOfObject / cameraFOVBasedValue;
					Debug.Log("Distance Z:" + distance);
					Debug.Log("Object Z:" + selectedObject.transform.position.z);
					position = selectedObject.transform.position + selectedObject.transform.forward * distance * 1.8f;
					
					//-------- Set the height of the camera ------
					position = position + new Vector3(0f,wallCabinet.cameraHeightWithObjectOnZoom,0f);
					Debug.Log("Position Z:" + position.z);
				}
				//--------- Else use manual fit -----------
				else{
					position = selectedObject.transform.position + selectedObject.transform.forward * wallCabinet.distanceFromObjectOnZoom;
					position = position + new Vector3(0f,wallCabinet.cameraHeightWithObjectOnZoom,0f);
				}
				break;
			}
			
			//-------------- CORNER CABINET -----------------
			case CabinetScript.TypeOfCabinet.CornerCabinet:{
				//---- IF Auto Fit -----------------
				if(cornerCabinet.autoFitCameraToBaseCabinetSize){
					//----------- Set distance from the front of the object -------
					float sizeOfObject = Mathf.Max(selectedObject.GetComponent<Collider>().bounds.extents.x,selectedObject.GetComponent<Collider>().bounds.extents.y * cornerCabinet.maxZoomLimitOnObject);
					float distance = sizeOfObject / cameraFOVBasedValue;
					Debug.Log("Distance Z:" + distance);
					Debug.Log("Object Z:" + selectedObject.transform.position.z);
					position = selectedObject.transform.position + selectedObject.transform.forward * distance * 1.8f - selectedObject.transform.right * distance * 1.8f;
					
					//-------- Set the height of the camera --------
					position = position + new Vector3(0f,cornerCabinet.cameraHeightWithObjectOnZoom,0f);
					Debug.Log("Position Z:" + position.z);
				}
				//--------- Else use manual fit -----------	
				else{
					position = selectedObject.transform.position + selectedObject.transform.forward * cornerCabinet.distanceFromObjectOnZoom - selectedObject.transform.right * cornerCabinet.distanceFromObjectOnZoom;
					position = position + new Vector3(0f,cornerCabinet.cameraHeightWithObjectOnZoom,0f);
				}
				break;
			}
			
			//-------------- TALL CABINET -----------------
			case CabinetScript.TypeOfCabinet.TallCabinet:{
				//---- IF Auto Fit -----------------
				if(oilPullout.autoFitCameraToBaseCabinetSize){
					//----------- Set distance from the front of the cabinet -----
					float sizeOfObject = Mathf.Max(selectedObject.GetComponent<Collider>().bounds.extents.x,selectedObject.GetComponent<Collider>().bounds.extents.y * oilPullout.maxZoomLimitOnObject);
					float distance = sizeOfObject / cameraFOVBasedValue;
					Debug.Log("Distance Z:" + distance);
					Debug.Log("Object Z:" + selectedObject.transform.position.z);
					position = selectedObject.transform.position + selectedObject.transform.forward * distance * 1.8f;
					
					//-------- Set the height of the camera ------
					position = position + new Vector3(0f,oilPullout.cameraHeightWithObjectOnZoom,0f);
					Debug.Log("Position Z:" + position.z);
				}
				//--------- Else use manual fit -----------
				else{
					position = selectedObject.transform.position + selectedObject.transform.forward * oilPullout.distanceFromObjectOnZoom + selectedObject.transform.right * oilPullout.distanceFromObjectOnZoom;
					position = position + new Vector3(0f,oilPullout.cameraHeightWithObjectOnZoom,0f);
				}
				break;
			}
			
			//-------------- HOB -----------------
			case CabinetScript.TypeOfCabinet.Hob:{
				//---- IF Auto Fit -----------------
				if(hob.autoFitCameraToBaseCabinetSize){
					//----------- Set distance from the fron of the cabinet -----
					float sizeOfObject = Mathf.Max(selectedObject.GetComponent<Collider>().bounds.extents.x,selectedObject.GetComponent<Collider>().bounds.extents.y * hob.maxZoomLimitOnObject);
					float distance = sizeOfObject / cameraFOVBasedValue;
					Debug.Log("Distance Z:" + distance);
					Debug.Log("Object Z:" + selectedObject.transform.position.z);
					position = selectedObject.transform.position + selectedObject.transform.forward * distance * 1.8f;
					
					//-------- Set the height of the camera ------
					position = position + new Vector3(0f,hob.cameraHeightWithObjectOnZoom,0f);
					Debug.Log("Position Z:" + position.z);
				}
				//--------- Else use manual fit -----------
				else{
					position = selectedObject.transform.position + selectedObject.transform.forward * hob.distanceFromObjectOnZoom;
					position = position + new Vector3(0f,hob.cameraHeightWithObjectOnZoom,0f);
				}
				break;
			}
			
			//---------------------------------------------------------- INTERNALS -------------------------------------------------------
			
			//-------------- DRAWS -----------------
			case CabinetScript.TypeOfCabinet.MidSizeCabinet:{
				//---- IF Auto Fit -----------------
				if(drawers.autoFitCameraToBaseCabinetSize){
					//----------- Set distance from the fron of the cabinet -----
					float sizeOfObject = Mathf.Max(selectedObject.GetComponent<Collider>().bounds.extents.x,selectedObject.GetComponent<Collider>().bounds.extents.y * drawers.maxZoomLimitOnObject);
					float distance = sizeOfObject / cameraFOVBasedValue;
					Debug.Log("Distance Z:" + distance);
					Debug.Log("Object Z:" + selectedObject.transform.position.z);
					position = selectedObject.transform.position + selectedObject.transform.forward * distance * 1.8f;
					
					//-------- Set the height of the camera ------
					position = position + new Vector3(0f,drawers.cameraHeightWithObjectOnZoom,0f);
					Debug.Log("Position Z:" + position.z);
				}
				//--------- Else use manual fit -----------
				else{
					position = selectedObject.transform.position + selectedObject.transform.forward * drawers.distanceFromObjectOnZoom;
					position = position + new Vector3(0f,drawers.cameraHeightWithObjectOnZoom,0f);
				}
				break;
			}
		}
		return position;
	}
	
	//=========== Function to calculate camera rotation on selecting different cabinet types ===============
	Vector3 RotateCamera(GameObject selectedObject,CabinetScript.TypeOfCabinet objectType) {
		Vector3 rotation = new Vector3();
		
		//----- Switch will help choose which values to use for camera rotation when an object is selected ------
		switch(objectType){
			//------------------- BASE CABINETS---------------
			case CabinetScript.TypeOfCabinet.BaseCabinet:{
				rotation = new Vector3(selectedObject.transform.rotation.eulerAngles.x + baseCabinet.verticalViewAngleOnObjectSelection,
			    	selectedObject.transform.rotation.eulerAngles.y + baseCabinet.horizontalViewAngleOnObjectSelection,selectedObject.transform.rotation.eulerAngles.z);
				break;
			}
			
			//------------------- WALL CABINETS ---------------
			case CabinetScript.TypeOfCabinet.WallCabinets: {
				rotation = new Vector3(selectedObject.transform.rotation.eulerAngles.x + wallCabinet.verticalViewAngleOnObjectSelection,
			                       selectedObject.transform.rotation.eulerAngles.y + wallCabinet.horizontalViewAngleOnObjectSelection,selectedObject.transform.rotation.eulerAngles.z);
				break;
			}
			
			//------------------- CORNER CABINETS ------------
			case CabinetScript.TypeOfCabinet.CornerCabinet: {
				rotation = new Vector3(selectedObject.transform.rotation.eulerAngles.x + cornerCabinet.verticalViewAngleOnObjectSelection,
			                       selectedObject.transform.rotation.eulerAngles.y + cornerCabinet.horizontalViewAngleOnObjectSelection,selectedObject.transform.rotation.eulerAngles.z);
				break;
			}
			
			//------------------- TALL CABINETS ---------------
			case CabinetScript.TypeOfCabinet.TallCabinet: {
				rotation = new Vector3(selectedObject.transform.rotation.eulerAngles.x + oilPullout.verticalViewAngleOnObjectSelection,
			                       selectedObject.transform.rotation.eulerAngles.y + oilPullout.horizontalViewAngleOnObjectSelection,selectedObject.transform.rotation.eulerAngles.z);
				break;
			}
			
			//-------------------- HOB -----------------------
			case CabinetScript.TypeOfCabinet.Hob: {
				rotation = new Vector3(selectedObject.transform.rotation.eulerAngles.x + hob.verticalViewAngleOnObjectSelection,
				                       selectedObject.transform.rotation.eulerAngles.y + hob.horizontalViewAngleOnObjectSelection,selectedObject.transform.rotation.eulerAngles.z);
				break;
			}
			
			//-------------------- INTERNALS ------------------------
			
			//-------------------- HOB -----------------------
			case CabinetScript.TypeOfCabinet.MidSizeCabinet: {
				rotation = new Vector3(selectedObject.transform.rotation.eulerAngles.x + drawers.verticalViewAngleOnObjectSelection,
			                       selectedObject.transform.rotation.eulerAngles.y + drawers.horizontalViewAngleOnObjectSelection,selectedObject.transform.rotation.eulerAngles.z);
			break;
			}
		}
		return rotation;
	}
	#endregion
	
	#region CameraDistanceOnRoomSizeChange
	//===== Used to calculate camera position based on size of the room =====
	public void RecalculateCameraOnRoomChange(GameObject WidthWall){
	
		//------ Calculate Distance to Move To ---------
		float widthOfKitchen = RoomManager.Instance.topWall.lengthOfWall;
		float depthOfKitchen = RoomManager.Instance.leftWall.lengthOfWall;
		
		Debug.Log("WidthOfKitchen" + widthOfKitchen);
		Debug.Log("DepthOfKitchen" + depthOfKitchen);
		
		//-------- Camera Position ---------
		float distance = Mathf.Max((widthOfKitchen/2) / cameraFOVBasedValue,depthOfKitchen * 1.02f);
		Debug.Log("Distance:" + distance);
		
		Vector3 newPosition = new Vector3(WidthWall.transform.position.x,initialCameraHeight,WidthWall.transform.position.z - distance);
		
		//----- Reset Camera Position ---------
		iTween.MoveTo(targetObject,newPosition,cameraFocusOnObjectSpeed);
		
		
		//----- Reset Camera Rotation ---------
		iTween.RotateTo(animationCamera,originalRotation,cameraFocusOnObjectSpeed);
		
		//-----Store original rotation before focusing--------
		StoreOriginalRotation();
		
		//----Store original position before focussing-------
		StoreOriginalPosition(newPosition);
	}
	#endregion
	
	#region ResetCameraToViewMode
	//======= Used to reset the camera position and rotation when exiting edit mode and returning to view mode ========
	void ResetCameraToViewMode(){
	
		//----- Use itween to set the position and rotation of the camera to the original values that were before going to edit mode ------
		iTween.MoveTo(animationCamera,originalPosition,cameraResetToViewModeSpeed);
		iTween.RotateTo(animationCamera,originalRotation,cameraResetToViewModeSpeed);
		
		//----- Set camera status to View Mode -----
		camStatus = cameraStatus.ViewMode;
		
		TBDragOrbitScript.IdealPitch = 0f;
		TBDragOrbitScript.IdealYaw = 0f;
		TBDragOrbitScript.Yaw = 0f;
		TBDragOrbitScript.Pitch = 0f;
	}
	#endregion
	
	#region StoreOriginalRotation
	//====== Used to store the original rotation when going from view mode to edit mode ======
	void StoreOriginalRotation(){
	
		//----- This will make sure that the value is stored only the first time when moving from view mode to edit mode even if this fuction is called all the time -----
		if(camStatus == cameraStatus.ViewMode){
			originalRotation = new Vector3(animationCamera.transform.rotation.eulerAngles.x,animationCamera.transform.rotation.eulerAngles.y,animationCamera.transform.rotation.eulerAngles.z);
		}
	}
	#endregion
	
	#region StoreOriginalPosition
	//===== Used to store the original position of the camera when going from view mode to edit mode =====
	void StoreOriginalPosition(Vector3 position){
	
		//----- This will make sure that the value is stored only the first time when moving from view mode to edit mode even if this fuction is called all the time -----
		if(camStatus == cameraStatus.ViewMode){
			//originalPosition = new Vector3(animationCamera.transform.position.x,animationCamera.transform.position.y,animationCamera.transform.position.z);
			originalPosition = position;
		}
	}
	#endregion
	
	#region EnterViewMode
	//===== This function will be called from UI when View Mode Button is pressed =====
	public void GoToViewMode(){
		
		ResetCameraToViewMode();
		
		//--- This event trigger will be used to enable the rotation in the TB Drag Orbit script ------
		if(enableCameraRotation != null)
			enableCameraRotation(cameraResetToViewModeSpeed);
				
		
		
	}
	#endregion
}
