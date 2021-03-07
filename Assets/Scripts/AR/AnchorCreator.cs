using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Mx.Log;


[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class AnchorCreator : MonoBehaviour
{
    private bool isDone = false;
    private bool isCreated = false;
    public GameObject prefab;
    public GameObject carPrefab;
    private float scaler;

    private float lastDis;
    private float distance;
    private Vector2 lasta;
    private Vector2 lastb;
    private float finalAngle=0;

    private ARSessionOrigin m_SessionOrigin;
    private GameObject spawnedObject;
    private ARRaycastHit hit;
    public bool isComfirmed { get; set; }
    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_Anchors)
        {
            Destroy(anchor);
        }
        m_Anchors.Clear();
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_Anchors = new List<ARAnchor>();
        DebugManager.Instance.RemoteDebug();
        isComfirmed = false;
        scaler = m_SessionOrigin.transform.localScale.x;
    }

    void Update()
    {
        if (isDone) return;

        if (!isCreated)
        {
            if (Input.touchCount == 0)
                return;

            var touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
                return;

            if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                hit = s_Hits[0];
                var hitPose = s_Hits[0].pose;
                spawnedObject = CreateObj(s_Hits[0]);
                isCreated = true;

            }
        }else
        {
            if (!isComfirmed) 
            {
                if (Input.touchCount == 2)
                {
                    if (Input.touches[0].phase == TouchPhase.Began || Input.touches[1].phase == TouchPhase.Began)
                    {
                        lasta = Input.touches[0].position;
                        lastb = Input.touches[1].position;
                        lastDis = Vector2.Distance(lasta, lastb);
                    }

                    if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Moved)
                    {
                        Vector2 a = Input.touches[0].position;
                        Vector2 b = Input.touches[1].position;
                        distance = Vector2.Distance(a, b);
                        float factor = distance - lastDis;
                        scaler -= factor*Time.deltaTime; 
                        m_SessionOrigin.transform.localScale = Vector3.one * scaler;

                        float angle = Vector2.Angle(b - a, lastb - lasta);
                        angle *= Mathf.Sign(Vector3.Cross(b - a, lastb - lasta).z);
                        finalAngle+=angle;
                        Debug.Log(scaler);
                        m_SessionOrigin.MakeContentAppearAt(spawnedObject.transform, spawnedObject.transform.position, Quaternion.AngleAxis(finalAngle, Vector3.up));
                        //spawnedObject.transform.Rotate(new Vector3(0, angle, 0));

                        lasta = a;
                        lastb = b;
                        lastDis= Vector2.Distance(lasta, lastb);
                    }
                }
            }
            else
            {
                if (spawnedObject == null)
                {
                    Debug.Log("Error creating anchor");
                }
                else
                {
                    var tempCar = Instantiate(carPrefab, spawnedObject.transform.position + new Vector3(0, 0.1f, 0), spawnedObject.transform.rotation);
                    //tempCar.transform.localScale = scaler;
                    //tempCar.GetComponent<WheelDrive>().ChangeSize(scaler);

                    var anchor = spawnedObject.GetComponent<ARAnchor>();
                    if (anchor == null)
                    {
                        anchor = spawnedObject.AddComponent<ARAnchor>();
                    }
                    m_Anchors.Add(anchor);
                }
                isDone = true;
            }
        }
    }

    GameObject CreateObj(in ARRaycastHit hit)
    {
        //var gameObject = Instantiate(prefab, hit.pose.position + new Vector3(0, 0.01f, 0), hit.pose.rotation);
        //gameObject.transform.localScale = scaler;

        var gameObject = Instantiate(prefab); 
        m_SessionOrigin.MakeContentAppearAt(gameObject.transform, hit.pose.position + new Vector3(0, 0.1f, 0), hit.pose.rotation);
        return gameObject;
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_Anchors;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;
}