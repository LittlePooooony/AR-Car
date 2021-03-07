using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private List<GameObject> trackList;
    private List<int> typeList;
    private List<Transform> openList;

    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        trackList = new List<GameObject>();
        typeList = new List<int>();
        openList = new List<Transform>();
        var origin = GameObject.Find("origin");
        var temp = origin.GetComponent<DockingPoint>().getDockingPoints();
        openList.Add(temp[0]);
        openList.Add(temp[1]);
    }
    public void SetOriginDocking()
    {
        var origin = GameObject.Find("origin");
        var temp = origin.GetComponent<DockingPoint>().getDockingPoints();
        openList.Add(temp[0]);
        openList.Add(temp[1]);
    }
    public void addTracks(GameObject gameObject,int type)
    {
        trackList.Add(gameObject);
        typeList.Add(type);
    }
    public void printlist()
    {
        foreach(GameObject a in trackList)
        {
            print(a.name);
        }
    }
    public bool InList(GameObject o)
    {
        foreach(var a in trackList)
        {
            if (o.Equals(a))
            {
                return true;
            }
        }
        return false;
    }
    public void ClearTrackList()
    {
        trackList.Clear();
        typeList.Clear();
        openList.Clear();
    }
    public List<GameObject> getAllTrack()
    {
        return trackList;
    }
    public List<int> getTypeList()
    {
        return typeList;
    }

    public void AddOpenList(Transform o)
    {
        openList.Add(o);
    }
    public void AddOpenListByObj(GameObject obj)
    {
        var dockingPoints = obj.GetComponent<DockingPoint>().getDockingPoints();
        openList.Add(dockingPoints[0]);
        openList.Add(dockingPoints[1]);
    }
    public List<Transform> getOpenList()
    {
        return openList;
    }

    public void RemoveOpenList(Transform o)
    {
        openList.Remove(o);
    }

    public void PrintOpenList()
    {
        foreach (var a in openList)
        {
            print(a);
        }
    }

    public void RemoveObjFromOpenList(GameObject obj)
    {
        var objDockPoints = obj.GetComponent<DockingPoint>().getDockingPoints();
        foreach (var dockingPoint in objDockPoints)
        {
            var opposite = obj.GetComponent<DockingPoint>().GetOpposite(dockingPoint);
            if (opposite != null)
            {
                openList.Add(opposite);
                obj.GetComponent<DockingPoint>().UnDocking(dockingPoint);
                opposite.parent.GetComponent<DockingPoint>().UnDocking(opposite);
            }
            openList.Remove(dockingPoint);
        }
    }
}
