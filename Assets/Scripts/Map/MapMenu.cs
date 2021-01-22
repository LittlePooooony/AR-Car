using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMenu : MonoBehaviour
{
    public GameObject car;
    public GameObject mapCreateUI;
    public Button testButton;
    public Text text;
    public SettingControl settingControl;

    private Vector3 originPos;
    private Vector3 showPos ;
    private Vector3 carOriPos;
    private Quaternion carOriDir;

    private float lastTime = 0;
    private float m_deltaTime=0;

    private void Start()
    {
        originPos = transform.position;
        GameObject buttonObj = GameObject.Find("TestButton");
        car = GameObject.FindGameObjectWithTag("Player");
        carOriDir = car.transform.rotation;
        carOriPos = car.transform.position;
        testButton = buttonObj.GetComponent<Button>();
        text = buttonObj.GetComponentInChildren<Text>();
    }
    private void Update()
    {
        m_deltaTime = Time.realtimeSinceStartup - lastTime;
        lastTime = Time.realtimeSinceStartup;
    }

    public void ShowMenu()
    {
        showPos = originPos + new Vector3(-210, 0, 0);
        StopAllCoroutines();
        StartCoroutine(IE_ShowMenu());
    }
    public void UnShowMenu()
    {
        StopAllCoroutines();
        StartCoroutine(IE_UnShowMenu());
    }

    IEnumerator IE_ShowMenu()
    {
        while(Vector3.Distance(transform.position, showPos) > 0.2f)
        {
            transform.position = Vector3.Lerp(transform.position,showPos,4*m_deltaTime);
            yield return 0;
        }
    }
    IEnumerator IE_UnShowMenu()
    {
        while (Vector3.Distance(transform.position, originPos) > 0.2f)
        {
            transform.position = Vector3.Lerp(transform.position, originPos, 4 * m_deltaTime);
            yield return 0;
        }
        transform.position = originPos;
    }

    public void SaveMap()
    {
        TrackFactory.Instance.SaveAllTracks();
    }
    public void LoadMap()
    {
        TrackFactory.Instance.LoadTracks();
    }
    public void TestMap()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DriftCamera>().SetFollowCar(true);
        testButton.onClick.RemoveAllListeners();
        testButton.onClick.AddListener(UnTextMap);
        settingControl.UnShowMenu();
        text.text = "退出测试";
        car.transform.SetPositionAndRotation(carOriPos,carOriDir);
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        mapCreateUI.SetActive(false);
    }

    public void UnTextMap()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DriftCamera>().SetFollowCar(false);
        testButton.onClick.RemoveAllListeners();
        testButton.onClick.AddListener(TestMap);
        text.text = "测试";
        car.transform.SetPositionAndRotation(carOriPos, carOriDir);
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        mapCreateUI.SetActive(true);
    }

    public void ExitMap()
    {
        Application.Quit();
    }
}
