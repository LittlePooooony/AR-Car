using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObject : MonoBehaviour
{

    private static SelectObject _instance;
    public static SelectObject Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private bool isClicking = false;
    private bool isWayPointDraging = false;
    private bool isMovingY = false;
    private bool isRotating = false;

    //鼠标是否有移动
    private bool isDraging = false;
    private GameObject selectObject = null;
    private Vector3 lastPos;

    private Vector2 lasta;
    private Vector2 lastb;
    private float lastY;

    private bool isDocking = false;
    private Transform minTransform = null;
    private Transform m_transform = null;
    private Transform[] transformList = null;
    private float minDis = 0;
    private int m_transformNum = 0;

    private bool isClickTimerCounting = false;
    private float doubleClickTime = 0.5f;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            //Click(Input.mousePosition);
            StartCoroutine(ClickTimer(Input.mousePosition));
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDraging && isClicking && !isWayPointDraging)
                ClickRelease();
            isWayPointDraging = false;
            isClicking = false;
            isDraging = false;
        }

#endif

#if UNITY_ANDROID
        if (Input.touchCount == 1)
        {

            if (Input.touches[0].phase == TouchPhase.Began)
            {
                //Click(Input.touches[0].position);
                StartCoroutine(ClickTimer(Input.touches[0].position));
            }

            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                if (isDraging && isClicking && !isWayPointDraging)
                    ClickRelease();
                isWayPointDraging = false;
                isClicking = false;
                isDraging = false;
            }
        }
        if (selectObject)
            if (Input.touchCount == 2)
            {
                if (Input.touches[0].phase == TouchPhase.Began || Input.touches[1].phase == TouchPhase.Began)
                {
                    lasta = Input.touches[0].position;
                    lastb = Input.touches[1].position;
                    lastY = ((lasta + lastb) / 2).y;
                }
                if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Moved)
                {
                    Vector2 a = Input.touches[0].position;
                    Vector2 b = Input.touches[1].position;
                    Vector2 mid = (a + b) / 2;
                    float angle = Vector2.Angle(b - a, lastb - lasta);
                    float deltaY = mid.y - lastY;
                    if (selectObject && !isMovingY && !isRotating)
                    {
                        if (Input.touches[0].phase == TouchPhase.Stationary && Input.touches[1].phase == TouchPhase.Moved
                                || Input.touches[1].phase == TouchPhase.Stationary && Input.touches[0].phase == TouchPhase.Moved) isRotating = true;
                        else
                        {
                            if (angle > 4) isRotating = true;
                            else if (Mathf.Abs(deltaY) > 3.5) isMovingY = true;

                        }
                        print(angle + "   " + deltaY);
                    }
                    print(isRotating+"  "+isMovingY);
                    if (isRotating)
                    {

                        angle *= Mathf.Sign(Vector3.Cross(b - a, lastb - lasta).z);
                        selectObject.transform.Rotate(new Vector3(0, angle, 0));
                    }
                    if (isMovingY)
                    {

                        selectObject.transform.Translate(new Vector3(0, (mid - (lasta + lastb) / 2).y, 0));
                    }
                    if (isRotating || isMovingY)
                    {
                        lasta = a; lastb = b;
                    }

                }

                if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Ended)
                {
                    ClickRelease();
                    isRotating = false;
                    isMovingY = false;
                }

            }
#endif

        if (isClicking && !isWayPointDraging)
        {

            var hitDis = Camera.main.WorldToScreenPoint(selectObject.transform.position).z;

            //targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, hitDis));
            var targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, hitDis));
            var temp = new Vector3(targetPos.x, selectObject.transform.position.y, targetPos.z);
            var offset = temp - lastPos;
            if (isDraging == false && offset.magnitude != 0)
            {
                isDraging = true;
                SetOpenListOnStartDraging();
            }
            selectObject.transform.position += offset;
            lastPos = temp;
            if (isDraging && !selectObject.GetComponent<ModifyPoints>())
                CheckNearest(temp);
        }

    }

    private void SetSelected(bool value)
    {
        if (selectObject != null)
        {
            ModifyPoints modifyPoints;
            if ((modifyPoints = selectObject.GetComponent<ModifyPoints>()))
            {
                modifyPoints.SetPointsVisibility(value);
            }
            if (selectObject.tag == "WayPoint")
            {
                selectObject.transform.parent.gameObject.GetComponent<ModifyPoints>().SetPointsVisibility(value);
            }
        }
    }

    private void Click(Vector3 inputPos)
    {
        if (!isClicking)
        {
            Ray ray = Camera.main.ScreenPointToRay(inputPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var tempObj = hit.collider.gameObject;
                Debug.Log(tempObj + "  " + selectObject);
                if (tempObj.tag == "WayPoint")
                {
                    tempObj.transform.parent.gameObject.GetComponent<ModifyPoints>().MovePoint(tempObj);
                    isClicking = true;
                    isWayPointDraging = true;
                    selectObject = tempObj;
                }
                if (SpawnManager.Instance.InList(tempObj))
                {
                    if (tempObj != selectObject)
                    {
                        if (selectObject)
                            SetSelected(false);
                        selectObject = tempObj;
                        SetSelected(true);
                    }
                    var hitDis = Camera.main.WorldToScreenPoint(selectObject.transform.position).z;
                    var targetPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, hitDis));
                    var temp = new Vector3(targetPos.x, selectObject.transform.position.y, targetPos.z);
                    lastPos = temp;
                    isClicking = true;
                    isWayPointDraging = false;
                }
                else
                {
                    if (selectObject != null && !isWayPointDraging)
                    {
                        SetSelected(false);
                        selectObject = null;
                    }
                }
            }
            else
            {
                SetSelected(false);
                selectObject = null;
            }
        }
    }
    private void ClickRelease()
    {
        print("ClickRelease");
        if (isDocking == true)
        {
            Vector3 offsetVec = selectObject.transform.position - m_transform.position;
            float angleOffset = Vector3.SignedAngle(m_transform.forward, -minTransform.forward, Vector3.up);
            var rot = Quaternion.Euler(0, angleOffset, 0);
            selectObject.transform.rotation *= rot;
            selectObject.transform.position = minTransform.position + rot * offsetVec;

            SpawnManager.Instance.RemoveOpenList(minTransform);
            SpawnManager.Instance.AddOpenList(transformList[(m_transformNum + 1) % 2]);
            minTransform.parent.GetComponent<DockingPoint>().Docking(minTransform, m_transform);
            m_transform.parent.GetComponent<DockingPoint>().Docking(m_transform, minTransform);
        }
        else
        {
            if (selectObject)
            {
                SpawnManager.Instance.AddOpenListByObj(selectObject);
            }
        }
        ResetNearest();
        isClicking = false;
    }
    private void SetOpenListOnStartDraging()
    {
        SpawnManager.Instance.RemoveObjFromOpenList(selectObject);
    }
    private void ResetNearest()
    {
        minTransform = null;
        m_transform = null;
        minDis = -1;
        transformList = null;
        m_transformNum = 0;
        isDocking = false;
    }
    private void CheckNearest(Vector3 pos)
    {
        ResetNearest();
        List<Transform> list = SpawnManager.Instance.getOpenList();
        foreach (Transform item in list)
        {
            if (minTransform == null)
            {
                minTransform = item;
                minDis = Vector3.Distance(pos, item.position);
                continue;
            }
            float temp = Vector3.Distance(pos, item.position);
            if (temp < minDis)
            {
                minDis = temp;
                minTransform = item;
            }
        }
        if (minDis > 0 && minDis < 30f)
        {
            transformList = selectObject.GetComponent<DockingPoint>().getDockingPoints();
            m_transform = transformList[0];
            float m_min = Vector3.Distance(transformList[0].position, minTransform.position);
            if (Vector3.Distance(transformList[1].position, minTransform.position) < m_min)
            {
                m_transform = transformList[1];
                m_transformNum = 1;
            }
            Vector3 offsetVec = selectObject.transform.position - m_transform.position;

            float angleOffset = Vector3.SignedAngle(m_transform.forward, -minTransform.forward, Vector3.up);
            var rot = Quaternion.Euler(0, angleOffset, 0);
            selectObject.transform.rotation *= rot;

            selectObject.transform.position = minTransform.position + rot * offsetVec;
            isDocking = true;
        }
        else isDocking = false;
    }
    public void SetSelectObject(GameObject obj)
    {
        selectObject = obj;
    }

    IEnumerator ClickTimer(Vector3 inputPos)
    {
        if (isClickTimerCounting) yield break;
        isClickTimerCounting = true;
        float timer = 0;
        Vector3 firstPos = inputPos;
        while (timer < doubleClickTime)
        {
            if (Input.GetMouseButtonUp(0) || Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended)
            {
                Click(inputPos);

                if (isDraging && isClicking && !isWayPointDraging)
                    ClickRelease();
                isWayPointDraging = false;
                isClicking = false;
                isDraging = false;

                isClickTimerCounting = false;
                yield break;
            }
            if (Input.mousePosition != firstPos || Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Moved)
            {
                Click(inputPos);
                isClickTimerCounting = false;
                yield break;
            }
            yield return 0;
            timer += Time.deltaTime;
        }
        Click(inputPos);
        isClickTimerCounting = false;
    }
}
