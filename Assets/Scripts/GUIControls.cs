using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIControls : MonoBehaviour
{
    private MainControls mainControls;

    // Start is called before the first frame update
    void Start()
    {
        mainControls = FindObjectOfType<MainControls>().GetComponent<MainControls>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteCurrentItem()
    {
        mainControls.CancelSelection();
        mainControls.TurnOffPlaneGrids();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
