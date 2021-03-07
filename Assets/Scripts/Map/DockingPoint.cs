using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingPoint : MonoBehaviour
{
    private Transform[] DockingPoints = new Transform[2];
    private Transform[] DockingObj = new Transform[2];

    private void Awake()
    {
        bool isCurve = false;
        if (GetComponent<ModifyPoints>()) isCurve = true;
        int i = 0;
        if (!isCurve)
        {
            foreach (Transform item in GetComponentsInChildren<Transform>(true))
            {
                if (i > 1) break;
                if (item.tag == "DockingPoint")
                {
                    DockingPoints[i++] = item;
                }
            }
        }
        else
        {
            PathCreator pathCreator = gameObject.GetComponent<PathCreator>();
            var numPoints = pathCreator.bezierPath.NumPoints;
            Vector3 head = pathCreator.bezierPath.GetPoint(0);
            Vector3 tail = pathCreator.bezierPath.GetPoint(numPoints-1);
            foreach (Transform item in GetComponentsInChildren<Transform>(true))
            {
                if (item.tag == "WayPoint")
                {
                    if(item.localPosition==head|| item.localPosition == tail)
                        DockingPoints[i++] = item;
                }
            }
        }
    }

    public Transform[] getDockingPoints()
    {
        return DockingPoints;
    }

    public void Docking(Transform a,Transform b)
    {
        int i = (a == DockingPoints[0]) ? 0 : 1;
        DockingObj[i] = b;
    }

    public void UnDocking(Transform a)
    {
        int i = (a == DockingPoints[0]) ? 0 : 1;
        DockingObj[i] = null;
    }

    public Transform GetOpposite(Transform a)
    {
        int i = (a == DockingPoints[0]) ? 0 : 1;
        return DockingObj[i];
    }
}
