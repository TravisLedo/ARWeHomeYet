using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the individual buttons that get generated per item
public class InventoryButton : MonoBehaviour
{
    private MainControls mainControls;
    private Inventory inventory ;

    [SerializeField] private GameObject text;
    [SerializeField] private GameObject linkedItem;
    private GestureHints gestureHints;

    public GameObject LinkedItem { get => linkedItem; set => linkedItem = value; }
    public GameObject Text { get => text; set => text = value; }

    // Start is called before the first frame update
    void Start()
    {
        mainControls = FindObjectOfType<MainControls>().GetComponent<MainControls>();
        inventory = FindObjectOfType<Inventory>().GetComponent<Inventory>();
        gestureHints = FindObjectOfType<GestureHints>().GetComponent<GestureHints>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickThisButton() //send over to the main script the item we selected
    {
        inventory.ToggleInventory(); //hide inventory when user selects item
        GameObject selectedItemOB = Instantiate(linkedItem, mainControls.Crosshair.transform.position, mainControls.Crosshair.transform.rotation);
        mainControls.SelectedItem = selectedItemOB;

        if (mainControls.CurrentItem != null)
        {
            Destroy(mainControls.CurrentItem); //get rid of current item
        }

        if (!gestureHints.GridItemYet && !gestureHints.PlacedItemYet)
        {
            BoughtFirstItem();
        }
    }

    public void BoughtFirstItem()
    {
        if (gestureHints.MoveAroundYet && !gestureHints.GridItemYet)
        {
            gestureHints.ShowGridHint(true);
        }
    }
}
