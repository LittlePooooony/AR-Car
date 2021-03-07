using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public SettingControl settingControl;
    public GameObject car;
    private RectTransform rectTransform;

    public Vector2 originPos;
    public Vector2 showPos;
    private float scale;
    public float m_deltaTime = 0;
    private float lastTime = 0;

    protected Vector3 carOriPos;
    protected Quaternion carOriDir;

    public void Start()
    {
        settingControl = GameObject.Find("SettingButton").GetComponent<SettingControl>();
        rectTransform = transform as RectTransform;
        originPos = rectTransform.anchoredPosition;
        scale = rectTransform.localScale.x;
        car = GameObject.FindGameObjectWithTag("Player");
        if (car)
        {
            carOriDir = car.transform.rotation;
            carOriPos = car.transform.position;
        }
    }
    public void SetCar()
    {
        car = GameObject.FindGameObjectWithTag("Player");
        carOriDir = car.transform.rotation;
        carOriPos = car.transform.position;
    }
    private void Update()
    {
        m_deltaTime = Time.realtimeSinceStartup - lastTime;
        lastTime = Time.realtimeSinceStartup;
    }
    public void ShowMenu()
    {
        showPos = originPos -new Vector2(rectTransform.rect.width*scale,0);
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
        while (Vector2.Distance(rectTransform.anchoredPosition, showPos) > 0.2f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, showPos, 4 * m_deltaTime);
            yield return 0;
        }
        rectTransform.anchoredPosition = showPos;
    }
    IEnumerator IE_UnShowMenu()
    {
        while (Vector2.Distance(rectTransform.anchoredPosition, originPos) > 0.2f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, originPos, 4 * m_deltaTime);
            yield return 0;
        }
        rectTransform.anchoredPosition = originPos;
    }
}
