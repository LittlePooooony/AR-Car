using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class GenRoad : MonoBehaviour
{
    public PathCreator path;
    public RoadMeshCreator roadMeshCreator;
    public List<Vector3> waypoints;
    // Start is called before the first frame update
    void Awake()
    {
        path = gameObject.AddComponent<PathCreator>();
        if(!(roadMeshCreator = GetComponent<RoadMeshCreator>()))
        roadMeshCreator = gameObject.AddComponent<RoadMeshCreator>();
        roadMeshCreator.pathCreator = path;
        foreach(Transform child in transform)
        {
            if (child.tag == "WayPoint")
            {
                waypoints.Add(child.localPosition);
            }
        }
        if (waypoints.Count > 0)
        {
            // Create a new bezier path from the waypoints.
            BezierPath bezierPath = new BezierPath(waypoints, false, PathSpace.xyz);
            path.bezierPath = bezierPath;
            int i = 0;
            foreach(var points in waypoints)
            {
                path.bezierPath.SetAnchorNormalAngle(i++,0);
            }
            roadMeshCreator.TriggerUpdate();
        }
    }
    
}
