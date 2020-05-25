using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject itemButton;

    private MainControls mainControls;
    private GridLayoutGroup gridLayoutGroup;
    private GameObject[] availableItems; //get from main controls
    private GestureHints gestureHints;

    // Start is called before the first frame update
    void Start()
    {
        mainControls = FindObjectOfType<MainControls>().GetComponent<MainControls>();
        gridLayoutGroup = FindObjectOfType<GridLayoutGroup>().GetComponent<GridLayoutGroup>();
        gestureHints = FindObjectOfType<GestureHints>().GetComponent<GestureHints>();
        availableItems = mainControls.AvailableItems;

        GenerateButtons();
        ToggleInventory();//after generating everything, hide the inventory at startup
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void GenerateButtons()//create a button for each item, give it the same name, make the button link to the item
    {
        foreach (GameObject g in availableItems)
        {
            GameObject buttonOB = Instantiate(itemButton, gridLayoutGroup.gameObject.transform);
            InventoryButton inventoryButton = buttonOB.GetComponent<InventoryButton>();
            inventoryButton.LinkedItem = g;
            inventoryButton.Text.GetComponent<Text>().text = "" + g.name;
        }
    }

    public void ToggleInventory()
    {
        if (isActiveAndEnabled)
        {
            gameObject.SetActive(false);

        }
        else
        {
            gameObject.SetActive(true);
            mainControls.InventorySound.Play();
        }

        if (gestureHints.PlacedItemYet && gestureHints.EditItemYet && !gestureHints.HintsdoneYet )
        {
            gestureHints.ShowEditItemHint(false);
            gestureHints.HintsdoneYet = true;
        }

    }
}
