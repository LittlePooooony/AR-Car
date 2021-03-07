using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AppController : MonoBehaviour
{
    public GameObject spawnPrefab;

    public GameObject UI_Control;
    public GameObject UI_button;


    private void Start()
    {
        UI_Control.SetActive(false);
    }

    public void Comfirm()
    {
        GetComponent<AnchorCreator>().isComfirmed = true;
        UI_Control.SetActive(true) ;
        UI_button.SetActive(false);
    }

}
