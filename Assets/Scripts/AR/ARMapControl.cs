using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Mx.Log;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
public class ARMapControl : MonoBehaviour
{
    private ARSessionOrigin m_SessionOrigin;
    private ARRaycastHit hit;
    private bool isOriginCreated = false;
    private bool isComfirmed = false;
    private bool isDone { get; set; }

    public GameObject startPrefab;
    public GameObject startObject;
    public GameObject car;
    public GameObject UI;
    public GameObject m_base;
    public GameObject comfirmButton;

    private float lastDis;
    private float distance;
    private Vector2 lasta;
    private Vector2 lastb;

    private float scaler;

    // Start is called before the first frame update
    void Start()
    {
        DebugManager.Instance.RemoteDebug();
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_base.SetActive(false);
        UI.SetActive(false);
        car.SetActive(false);
        
        scaler = m_SessionOrigin.transform.localScale.x;
        isDone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDone) return;
        if (!isOriginCreated)
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

                //if (hit.trackable is ARPlane plane)
                //{
                //    var oldPrefab = m_AnchorManager.anchorPrefab;
                //    m_AnchorManager.anchorPrefab = startPrefab;
                //    var temp = m_AnchorManager.AttachAnchor(plane, hit.pose);
                //    startObject = temp.gameObject;
                //    m_base.transform.position = hitPose.position - new Vector3(0, 0.01f, 0);
                //    m_AnchorManager.anchorPrefab = oldPrefab;

                //    isOriginCreated = true;
                //}
                startObject = Instantiate(startPrefab, hit.pose.position, hit.pose.rotation);
                m_SessionOrigin.MakeContentAppearAt(startObject.transform, hit.pose.position + new Vector3(0, 0.1f, 0), hit.pose.rotation);
                startObject.name = "origin";
                var temp = GameObject.Find("SpawnManager");
                temp.GetComponent<SpawnManager>().SetOriginDocking();
                m_base.transform.position = startObject.transform.position - new Vector3(0, 0.01f, 0);
                isOriginCreated = true;
            }
        }
        else
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
                        scaler -= factor * Time.deltaTime;
                        m_SessionOrigin.transform.localScale = Vector3.one * scaler;

                        lasta = a;
                        lastb = b;
                        lastDis = Vector2.Distance(lasta, lastb);
                        Debug.Log(scaler);
                    }
                }
            }
        }
    }

    public void Comfirm()
    {
        if (startObject.GetComponent<ARAnchor>() == null)
        {
            startObject.AddComponent<ARAnchor>();
        }

        isComfirmed = true;
        UI.SetActive(true);
        car.transform.position = startObject.transform.position + new Vector3(0,0.5f,0);
        car.SetActive(true);
        m_base.SetActive(true);
        comfirmButton.SetActive(false);
        isDone = true;
    }


    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;
}
