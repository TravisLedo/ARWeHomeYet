using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MainControls : MonoBehaviour
{
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Text debugTextOB;
    [SerializeField] private GameObject[] availableItems;
    [SerializeField] private GameObject deleteButton;

    private List<GameObject> itemsPlaced = new List<GameObject>(); //list of all objects the user placed in the world

    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool placementPoseValid;
    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager planeManager;
    private Renderer planeMeshRenderer;
    private GestureHints gestureHints;


    private Camera mainCam;
    private GameObject currentItem; //Which item the user is currently controlling
    private GameObject selectedItem; // Which item is selected from the inventory 

    private Vector2 startTapPos;
    private Vector2 endTapPos;

    private float rotateSpeedAmplify = 50f;
    private float swipeDistance;
    private float tapHeldDownTime;

    public GameObject[] AvailableItems { get => availableItems; set => availableItems = value; }
    public GameObject SelectedItem { get => selectedItem; set => selectedItem = value; }
    public GameObject CurrentItem { get => currentItem; set => currentItem = value; }
    public ARPlaneManager PlaneManager { get => planeManager; set => planeManager = value; }
    public GameObject DeleteButton { get => deleteButton; set => deleteButton = value; }
    public GameObject Crosshair { get => crosshair; set => crosshair = value; }
    public Text DebugTextOB { get => debugTextOB; set => debugTextOB = value; }
    public AudioSource InventorySound { get => inventorySound; set => inventorySound = value; }

    private bool showGrid = false;
    private bool hasEnoughPlanesToStart = false; //We want to make sure the user moves around a bit first

    [SerializeField] AudioSource selectSound;
    [SerializeField] AudioSource deleteSound;
    [SerializeField] AudioSource inventorySound;
    [SerializeField] AudioSource placeSound;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        aRRaycastManager = arOrigin.GetComponent<ARRaycastManager>();
        PlaneManager = arOrigin.GetComponent<ARPlaneManager>();
        planeMeshRenderer = PlaneManager.planePrefab.GetComponent<Renderer>();
        gestureHints = FindObjectOfType<GestureHints>().GetComponent<GestureHints>();

        TurnOffPlaneGrids();
        deleteButton.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        //debugTextOB.text = ""+ itemsPlaced.Count;
        UpdatePose();
        UpdateCrosshair();
        UpdateItem();
        UpdateGrid();

        if (!gestureHints.MoveAroundYet) {
            MoveAroundHint();
        }





        if (CurrentItem != null && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //If user double taps, they will place a an object at the current position
            if (Input.GetTouch(0).tapCount == 2)
            {
                PlaceItem();
            }
            startTapPos = Input.GetTouch(0).position;


            if (gestureHints.MoveAroundYet && gestureHints.GridItemYet && !gestureHints.PlacedItemYet)
            {
                gestureHints.ShowPlaceItemHint(true); //show the double tap hint once roate hint is done
            }

        }
        //If user holds down tap on an object and not currently controlling an object, they will take control of that object
        if (CurrentItem == null && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary) {
            tapHeldDownTime += Time.deltaTime;
            EditItem();
        }
        //Reset tap held down counter everytime user releases a tap
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            tapHeldDownTime = 0;

        }


            //detecting swipes to rotate object
        if (CurrentItem != null && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            endTapPos = Input.GetTouch(0).position;
            swipeDistance = Vector3.Distance(startTapPos, endTapPos);
            if (startTapPos.x < endTapPos.x)
            {
                RotateItemLeft(swipeDistance);
            }
            if (startTapPos.x > endTapPos.x)
            {
                RotateItemRight(swipeDistance);

            }

            //User stopped moving but did not release tap. We should make this the new start tap position
            if (endTapPos == Input.GetTouch(0).position)
            {
                startTapPos = endTapPos;
            }

        }


    }

    private void UpdateItem()
    {
        if (SelectedItem != null) {
            ChangeCurrentItem(SelectedItem);
            TurnOnPlaneGrids();
            deleteButton.SetActive(true);
        }
    }

    public void ChangeCurrentItem(GameObject item) //get the selected item and make it the current item we are controlling
    {

        if (selectedItem != null)
        {
            selectSound.Play();
            currentItem = Instantiate(item, SelectedItem.transform.position, SelectedItem.transform.rotation); //using the actual item as a preview
            Destroy(selectedItem);
        }
        selectedItem = null;

    }


    private void UpdateCrosshair()
    {
        if (placementPoseValid && Crosshair.activeInHierarchy)
        {
            Crosshair.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }

    }

    void UpdatePose()//AR raycasting
    {
        Vector3 centerOfScreen = mainCam.ViewportToScreenPoint(new Vector3(0.5f, 0.2f)); //a little offset on the y from center
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(centerOfScreen, hits, TrackableType.PlaneWithinPolygon);

        placementPoseValid = hits.Count > 0;
        if (placementPoseValid)
        {
            placementPose = hits[0].pose;
            Vector3 cameraDir = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraDir);

            if (CurrentItem != null)
            {
                CurrentItem.transform.SetPositionAndRotation(placementPose.position, CurrentItem.transform.rotation);
            }

            if (gestureHints.GridItemYet && !gestureHints.PlacedItemYet)
            {
                gestureHints.ShowPlaceItemHint(true);
                gestureHints.ShowGridHint(false);

            }
        }



    }

    //Place item into world
    public void PlaceItem()
    {
        placeSound.Play();

        GameObject placedItemOB = Instantiate(CurrentItem, CurrentItem.transform.position, CurrentItem.transform.rotation);
        itemsPlaced.Add(placedItemOB);
        CancelSelection();
        TurnOffPlaneGrids();
        gestureHints.ShowPlaceItemHint(false);

        if (itemsPlaced.Count == 1 && gestureHints.PlacedItemYet && !gestureHints.HintsdoneYet) // show edit hint
        {
            gestureHints.ShowEditItemHint(true);
        }

    }

    public void RotateItemLeft(float rotateSpeed)
    {
        CurrentItem.transform.Rotate(Vector3.up * rotateSpeedAmplify * rotateSpeed * Time.deltaTime);

    }

    public void RotateItemRight(float rotateSpeed)
    {
        CurrentItem.transform.Rotate(-Vector3.up * rotateSpeedAmplify * rotateSpeed * Time.deltaTime);

    }

    public void CancelSelection()
    {
        if (CurrentItem != null)
        {
            SelectedItem = null; //No item selected
            Destroy(CurrentItem);
        }
        CurrentItem = null; //Clear the floating item preview object
        deleteButton.SetActive(false);
        deleteSound.Play();

    }

    public void TurnOnPlaneGrids() 
    {
        showGrid = true;
        gestureHints.GuiCrosshair.SetActive(true);
    }

    public void TurnOffPlaneGrids() 
    {
        showGrid = false;
        gestureHints.GuiCrosshair.SetActive(false);

    }

    public void UpdateGrid()
    {
       // DebugTextOB.text = "" + planeManager.trackables.count;

        if (showGrid)
        {
            foreach (ARPlane plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(true);
            }
            Crosshair.SetActive(true);

        }
        else
        {
            foreach (ARPlane plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
            Crosshair.SetActive(false);

        }

    }

    public void MoveAroundHint()
    {
        if (planeManager.trackables.count < 1) //have not found a plane yet, show move around hint until at least 1 plane is detected
        {
            gestureHints.ShowMoveAroundHint(true);
        }
        else
        {
            gestureHints.ShowMoveAroundHint(false);
            gestureHints.MoveAroundYet = true;

        }
    }

    public void EditItem()
    {
        if (tapHeldDownTime > 1.5)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider != null)
                {
                    GameObject touchedObject = hit.transform.gameObject;
                    SelectedItem = touchedObject.transform.parent.gameObject;
                    itemsPlaced.Remove(selectedItem);
                    //turn off hint for long tap
                    if (gestureHints.PlacedItemYet && !gestureHints.HintsdoneYet)
                    {
                        gestureHints.ShowEditItemHint(false);
                        gestureHints.HintsdoneYet = true;
                    }
                }
            }
            tapHeldDownTime = 0;
        }
    }
}
