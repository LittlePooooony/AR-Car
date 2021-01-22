using PathCreation;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyPoints : MonoBehaviour
{

    PathCreator pathCreator;
    public RoadMeshCreator roadMeshCreator;
    private bool isDraging = false;
    private GameObject selectObject;
    private List<GameObject> wayPoints = new List<GameObject>();
    private List<int> pointIndex = new List<int>();
    private int nowIndex;
    private int numPoints;

    private bool isDocking = false;
    private Transform minTransform = null;
    private Transform m_transform = null;
    private float minDis = 0;

    private Vector3 vecHead;
    private Vector3 vecTail;

    // Start is called before the first frame update
    void Awake()
    {
        print("start");
        pathCreator = gameObject.GetComponent<PathCreator>();
        roadMeshCreator = GetComponent<RoadMeshCreator>();
        numPoints = pathCreator.bezierPath.NumPoints;
        int i = 0,j = 0;
        foreach (Transform child in transform)
        {
            wayPoints.Add(child.gameObject);
            while (j < numPoints)
            {
                if (pathCreator.bezierPath.GetPoint(j) == child.localPosition)
                {
                    pointIndex.Add(j);
                    j++;
                    break;
                }
                j++;
            }
            i++;
        }
        vecHead = (pathCreator.bezierPath.GetPoint(0) - pathCreator.bezierPath.GetPoint(1)).normalized;
        vecTail = (pathCreator.bezierPath.GetPoint(numPoints-1) - pathCreator.bezierPath.GetPoint(numPoints-2)).normalized;
        wayPoints[0].transform.rotation = Quaternion.LookRotation(vecHead);
        wayPoints[wayPoints.Count-1].transform.rotation = Quaternion.LookRotation(vecTail);
        SetPointsVisibility(false);
    }

    // Update is called once per frame
    void Update()
    {
        //print(isDraging);
        //if (Input.GetMouseButton(0))
        //{
        //    if (!isDraging)
        //    {
        //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        RaycastHit hit;
        //        if (Physics.Raycast(ray, out hit))
        //        {
        //            if (hit.collider.gameObject.tag == "WayPoint" &&
        //                hit.collider.gameObject.transform.parent.gameObject == gameObject)
        //            {
        //                selectObject = hit.collider.gameObject;
        //                isDraging = true;
        //                for (int i = 0; i < wayPoints.Count; i++)
        //                {
        //                    if (selectObject == wayPoints[i])
        //                    {
        //                        nowIndex = pointIndex[i];
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        if (Input.GetMouseButtonUp(0))
        {
            if (isDraging && (nowIndex == 0 || nowIndex == (pathCreator.bezierPath.NumPoints - 1)))
                ClickRelease();
            isDraging = false;
        }
        if (isDraging)
        {
            var hitDis = Camera.main.WorldToScreenPoint(selectObject.transform.position).z;
            var temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, hitDis));
            var targetPos = new Vector3(temp.x, selectObject.transform.position.y, temp.z);
            selectObject.transform.position =  targetPos;
            pathCreator.bezierPath.MovePoint(nowIndex,targetPos-transform.position);
            vecHead = (pathCreator.bezierPath.GetPoint(0) - pathCreator.bezierPath.GetPoint(1)).normalized;
            vecTail = (pathCreator.bezierPath.GetPoint(numPoints - 1) - pathCreator.bezierPath.GetPoint(numPoints - 2)).normalized;
            wayPoints[0].transform.rotation = Quaternion.LookRotation(vecHead);
            wayPoints[wayPoints.Count - 1].transform.rotation = Quaternion.LookRotation(vecTail);

            if (roadMeshCreator) roadMeshCreator.TriggerUpdate();

            if(nowIndex==0||nowIndex== (pathCreator.bezierPath.NumPoints-1))
                CheckNearest(targetPos);
        }
    }
    public void MovePoint(GameObject o)
    {
        selectObject = o;
        isDraging = true;
        for (int i = 0; i < wayPoints.Count; i++)
        {
            if (selectObject == wayPoints[i])
            {
                nowIndex = pointIndex[i];
                break;
            }
        }
        SpawnManager.Instance.RemoveOpenList(o.transform);

        var opposite = GetComponent<DockingPoint>().GetOpposite(selectObject.transform);
        if (opposite != null)
        {
            SpawnManager.Instance.AddOpenList(opposite);
            GetComponent<DockingPoint>().UnDocking(selectObject.transform);
            opposite.parent.GetComponent<DockingPoint>().UnDocking(opposite);
        }
    }
    public void SetPointsVisibility(bool value)
    {
        foreach(var p in wayPoints)
        {
            p.SetActive(value);
        }
    }

    private void ClickRelease()
    {
        if (isDocking == true)
        {
            selectObject.transform.position = minTransform.position;
            pathCreator.bezierPath.MovePoint(nowIndex, selectObject.transform.position - transform.position);
            pathCreator.bezierPath.MovePoint(nowIndex, selectObject.transform.position - transform.position);
            if (nowIndex == 0)
            {
                pathCreator.bezierPath.addDockedList(1);
                pathCreator.bezierPath.SetPoint(1, selectObject.transform.position - transform.position + minTransform.forward*20);
            }
            if (nowIndex == (pathCreator.bezierPath.NumPoints - 1))
            {
                print(nowIndex);
                pathCreator.bezierPath.addDockedList(nowIndex - 1);
                pathCreator.bezierPath.SetPoint(nowIndex - 1, selectObject.transform.position - transform.position + minTransform.forward*20);
            }
            selectObject.transform.rotation = Quaternion.LookRotation(-minTransform.forward);
            if (roadMeshCreator) roadMeshCreator.TriggerUpdate();

            SpawnManager.Instance.RemoveOpenList(minTransform);
            minTransform.parent.GetComponent<DockingPoint>().Docking(minTransform, m_transform);
            m_transform.parent.GetComponent<DockingPoint>().Docking(m_transform, minTransform);
        }
        else
        {
            if (nowIndex == 0) pathCreator.bezierPath.removeDockedList(1);
            if (nowIndex == (pathCreator.bezierPath.NumPoints - 1)) pathCreator.bezierPath.removeDockedList(nowIndex - 1);
            var opposite = GetComponent<DockingPoint>().GetOpposite(selectObject.transform);
            if (opposite != null)
            {
                SpawnManager.Instance.AddOpenList(opposite);
                GetComponent<DockingPoint>().UnDocking(selectObject.transform);
                opposite.parent.GetComponent<DockingPoint>().UnDocking(opposite);
            }
            SpawnManager.Instance.AddOpenList(selectObject.transform);
        }
        ResetNearest();
        selectObject = null;
    }

    private void ResetNearest()
    {
        minTransform = null;
        m_transform = null;
        minDis = -1;
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
            m_transform = selectObject.transform;
            selectObject.transform.position = minTransform.position;
            isDocking = true;

            pathCreator.bezierPath.MovePoint(nowIndex, selectObject.transform.position - transform.position);
            if (nowIndex == 0)
            {
                pathCreator.bezierPath.SetPoint(1, selectObject.transform.position - transform.position + minTransform.forward*20);
            }
            if (nowIndex == (pathCreator.bezierPath.NumPoints - 1))
            {
                pathCreator.bezierPath.SetPoint(nowIndex - 1, selectObject.transform.position - transform.position + minTransform.forward*20);
            }

            if (roadMeshCreator) roadMeshCreator.TriggerUpdate();
        }
        else
        {
            isDocking = false;
        }
    }

    public GameObject[] getHeadAndTail()
    {
        GameObject[] headAndTail = new GameObject[2];
        headAndTail[0] = wayPoints[0];
        headAndTail[1] = wayPoints[wayPoints.Count - 1];
        return headAndTail;
    }
    public void setAllWayPoints(List<m_transform> list)
    {
        print("load way points");
        int i = 0,pointIndex=0;
        for (i = 0; i < wayPoints.Count; i++,pointIndex+=3)
        {
            wayPoints[i].transform.SetPositionAndRotation(list[i].position - transform.position, list[i].rotation);
            pathCreator.bezierPath.MovePoint(pointIndex, wayPoints[i].transform.position-transform.position);
            print(list[i].position + "\n" + list[i].rotation);
        }
        pathCreator.bezierPath.addDockedList(1);
        pathCreator.bezierPath.addDockedList(8);
        pathCreator.bezierPath.SetPoint(1, wayPoints[0].transform.position - transform.position - wayPoints[0].transform.forward * 20);
        pathCreator.bezierPath.SetPoint(8, wayPoints[3].transform.position - transform.position - wayPoints[3].transform.forward * 20);
        roadMeshCreator.TriggerUpdate();
        roadMeshCreator.UpdateCollider();
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawCube(pathCreator.bezierPath.GetPoint(0) + transform.position, new Vector3(2, 2, 2));
    //    Gizmos.DrawCube(pathCreator.bezierPath.GetPoint(1) + transform.position, new Vector3(2, 2, 2));
    //}
}
