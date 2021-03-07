using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapMenu : Menu
{
    
    public GameObject mapCreateUI;
    public Button testButton;
    public Text text;
    public GameObject saveSlots;
    private int selectSlot = 0;
    public GameObject WarningPanel;

    private new void Start()
    {
        base.Start();
        car.SetActive(false);
        GameObject buttonObj = GameObject.Find("TestButton");
        testButton = buttonObj.GetComponent<Button>();
        text = buttonObj.GetComponentInChildren<Text>();
        saveSlots = GameObject.Find("UI/Canvas");
        WarningPanel = GameObject.Find("UI/Canvas/Warning");
        saveSlots.SetActive(false);
        WarningPanel.SetActive(false);
    }




    public void SaveMap()
    {
        saveSlots.SetActive(true);
    }
    public void LoadMap()
    {
        TrackFactory.Instance.LoadTracks();
    }
    public void TestMap()
    {
        var temp = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DriftCamera>();
        if (temp) temp.SetFollowCar(true);
        testButton.onClick.RemoveAllListeners();
        testButton.onClick.AddListener(UnTestMap);
        settingControl.UnShowMenu();
        text.text = "退出测试";
        car.SetActive(true);
        car.transform.SetPositionAndRotation(carOriPos,carOriDir);
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        mapCreateUI.SetActive(false);
    }

    public void UnTestMap()
    {
        var temp = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DriftCamera>();
        if (temp) temp.SetFollowCar(false);
        testButton.onClick.RemoveAllListeners();
        testButton.onClick.AddListener(TestMap);
        text.text = "测试";
        car.SetActive(false);
        car.transform.SetPositionAndRotation(carOriPos, carOriDir);
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        mapCreateUI.SetActive(true);
    }

    public void ExitMap()
    {
        settingControl.UnShowMenu();
        SceneManager.LoadScene(0);
    }

    public void Save(int n)
    {
        selectSlot = n;
        if (GameInfo.Instance.GetSaveSlotBool(n))
        {
            WarningPanel.SetActive(true);
        }
        else
        {
            TrackFactory.Instance.SaveAllTracks(n);
            saveSlots.SetActive(false);
            GameInfo.Instance.UpdateSlot();
        }
    }
    public void SaveMapReturn()
    {
        saveSlots.SetActive(false);
    }

    public void SaveComfirm()
    {
        TrackFactory.Instance.SaveAllTracks(selectSlot);
        WarningPanel.SetActive(false);
        saveSlots.SetActive(false);
        GameInfo.Instance.UpdateSlot();
    }

    public void SaveReturn()
    {
        WarningPanel.SetActive(false);
    }
}
