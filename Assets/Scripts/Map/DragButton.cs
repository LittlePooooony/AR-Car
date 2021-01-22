using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragButton : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public GameObject prefabObject;
    private GameObject _objDragSpawning;
    private bool _isDragSpawning = false;
    public GameObject m_base;

    private bool isDocking = false;
    private Transform minTransform = null;
    private Transform m_transform = null;
    private Transform[] transformList = null;
    private float minDis = 0;
    private int m_transformNum = 0;

    void Start()
    {
        m_base.SetActive(false);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        ResetNearest();
        if (prefabObject != null)
        {
            _objDragSpawning = Instantiate(prefabObject);
            int type=0;
            switch (prefabObject.name) 
            {
                case "MT_Turn": type = 0;break;
                case "MT_Road_02": type = 1;break;
                case "MT_Road_01": type = 2;break;
                case "CustomCurve":type = 3;break;
            }
            SpawnManager.Instance.addTracks(_objDragSpawning,type);
            //SelectObject.Instance.SetSelectObject(_objDragSpawning);
            _isDragSpawning = true;
            m_base.SetActive(true);

            transformList = _objDragSpawning.GetComponent<DockingPoint>().getDockingPoints();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
#if UNITY_EDITOR
        if (_isDragSpawning)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _objDragSpawning.transform.position = new Vector3(hit.point.x,0,hit.point.z);
            }

            if (!_objDragSpawning.GetComponent<ModifyPoints>()) 
                CheckNearest(new Vector3(hit.point.x, 0, hit.point.z));
        }
#endif

#if UNITY_ANDROID
        if (_isDragSpawning)
        {
            if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    Vector3 ptMouse = Input.touches[0].position;
                    Ray ray = Camera.main.ScreenPointToRay(ptMouse);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        _objDragSpawning.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
                    }
                    if (!_objDragSpawning.GetComponent<ModifyPoints>())
                        CheckNearest(new Vector3(hit.point.x, 0, hit.point.z));
                }
 
                if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    _isDragSpawning = false;
                    _objDragSpawning = null;
                }
            }
        }
#endif
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDocking == true)
        {
            Vector3 offsetVec = _objDragSpawning.transform.position - m_transform.position;
            float angleOffset = Vector3.SignedAngle(m_transform.forward, -minTransform.forward, Vector3.up);
            var rot = Quaternion.Euler(0, angleOffset, 0);
            _objDragSpawning.transform.rotation *= rot;
            _objDragSpawning.transform.position = minTransform.position + rot * offsetVec;

            SpawnManager.Instance.RemoveOpenList(minTransform);
            SpawnManager.Instance.AddOpenList(transformList[(m_transformNum + 1) % 2]);
            minTransform.parent.GetComponent<DockingPoint>().Docking(minTransform, m_transform);
            m_transform.parent.GetComponent<DockingPoint>().Docking(m_transform, minTransform);
        }
        else
        {
            SpawnManager.Instance.AddOpenList(transformList[0]);
            SpawnManager.Instance.AddOpenList(transformList[1]);
        }

        _isDragSpawning = false;
        _objDragSpawning = null;
        m_base.SetActive(false);
    }
    private void ResetNearest()
    {
        minTransform = null;
        m_transform = null;
        minDis = -1;
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
            m_transform = transformList[0];
            float m_min = Vector3.Distance(transformList[0].position, minTransform.position);
            if (Vector3.Distance(transformList[1].position, minTransform.position) < m_min)
            {
                m_transform = transformList[1];
                m_transformNum = 1;
            }
            Vector3 offsetVec = _objDragSpawning.transform.position - m_transform.position;

            float angleOffset = Vector3.SignedAngle(m_transform.forward, -minTransform.forward, Vector3.up);
            var rot = Quaternion.Euler(0, angleOffset, 0);
            _objDragSpawning.transform.rotation *= rot;
            _objDragSpawning.transform.position = minTransform.position + rot * offsetVec;
            isDocking = true;
        }
        else isDocking = false;
    }
}
