using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Give user hints the first time they do anything. This controls each hint and if they have been triggered yet
public class GestureHints : MonoBehaviour
{
    private MainControls mainControls;
    [SerializeField] private GameObject moveAroundHint;
    [SerializeField] private GameObject gridHint;
    [SerializeField] private GameObject placeItemHint;
    [SerializeField] private GameObject editItemHint;
    [SerializeField] private Text hintText;
    [SerializeField] private GameObject guiCrosshair;

    private bool moveAroundYet;
    private bool buyItemYet;
    private bool gridItemYet; //Player looked at the grid and now has a prview of item
    private bool placedItemYet;
    private bool editItemYet;
    private bool hintsdoneYet;

    public bool MoveAroundYet { get => moveAroundYet; set => moveAroundYet = value; }
    public bool BuyItemYet { get => buyItemYet; set => buyItemYet = value; }
    public bool PlacedItemYet { get => placedItemYet; set => placedItemYet = value; }
    public bool EditItemYet { get => editItemYet; set => editItemYet = value; }
    public bool GridItemYet { get => gridItemYet; set => gridItemYet = value; }
    public bool HintsdoneYet { get => hintsdoneYet; set => hintsdoneYet = value; }
    public GameObject GuiCrosshair { get => guiCrosshair; set => guiCrosshair = value; }

    private float editHintTimer;



    // Start is called before the first frame update
    void Start()
    {
        mainControls = FindObjectOfType<MainControls>().GetComponent<MainControls>();

        moveAroundHint.SetActive(false);
        gridHint.SetActive(false);
        placeItemHint.SetActive(false);
        editItemHint.SetActive(false);
        GuiCrosshair.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (editItemYet)
        {
            editHintTimer += Time.deltaTime;

            if (editHintTimer > 6)
            {
                ShowEditItemHint(false);
                hintsdoneYet = true;
            }
        }
        Vector3 centerOfScreen = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.2f)); //a little offset on the y from center
        GuiCrosshair.transform.position = centerOfScreen;

    }

    public void ShowMoveAroundHint(bool toggle)
    {
        moveAroundHint.SetActive(toggle);
        if (toggle)
        {
            hintText.text = "Move around to calibrate.";
        }
        else
        {
            hintText.text = "";
        }
    }
    public void ShowBuyItemHint()
    {

    }




    public void ShowPlaceItemHint(bool toggle)
    {
        placeItemHint.SetActive(toggle);
        placedItemYet = true;

        if (toggle)
        {
            hintText.text = "Swipe to rotate. Double tap to drop.";
        }
        else
        {
            hintText.text = "";
        }
    }


    public void ShowGridHint(bool toggle)
    {
        gridHint.SetActive(toggle);
        gridItemYet = true;

        if (toggle)
        {
            hintText.text = "Aim at grid lines."; //maybe add starting crosshair here
          //  GuiCrosshair.SetActive(true);
        }
        else
        {
            //   hintText.text = "grid off";
          //  GuiCrosshair.SetActive(false);

        }

    }
    public void ShowEditItemHint(bool toggle)
    {
        editItemHint.SetActive(toggle);
        editItemYet = true;
        if (toggle)
        {
            hintText.text = "Long tap on objects to modify.";
        }
        else
        {

            hintText.text = "";
        }
    }




}
