using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarDrift : MonoBehaviour
{
    private Rigidbody m_rigidbody=null;
    private bool isDrifting = false;
    private float pressTime = 0;
    private Vector3 inertia;
    private Vector3 dir;
    public float frictionForce = 5f;

    private float timer = 0;

    public WheelCollider[] m_Wheels;
    private WheelFrictionCurve[] preFrictionCurve = new WheelFrictionCurve[4];

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        m_Wheels = GetComponentsInChildren<WheelCollider>();
        int i = 0;
        foreach (WheelCollider wheel in m_Wheels)
        {
            preFrictionCurve[i++] = wheel.sidewaysFriction;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("HandBrake")!=0)
        {
            pressTime += Time.deltaTime;
        }
        else if(!isDrifting&&pressTime>0&&pressTime<1f)
        {
            pressTime = 0;

            inertia = m_rigidbody.velocity;
            dir = inertia.normalized;
            isDrifting = true;
            ChangeFriction();
        }
        //print(Vector3.Angle(m_rigidbody.velocity.normalized, m_rigidbody.transform.forward));

        if (isDrifting && Vector3.Angle(m_rigidbody.velocity.normalized, m_rigidbody.transform.forward) < 5f)
        {
            timer += Time.deltaTime;
            if (timer >= 0.3f)
            {
                RevertFriction();
                isDrifting = false;
            }
        }
        else timer = 0;

    }
    
    private void ChangeFriction()
    {
        foreach (WheelCollider wheel in m_Wheels)
        {
            //steer wheels
            if (wheel.transform.localPosition.z > 0)
            {
                WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();
                wheelFrictionCurve.extremumSlip = 0.1f;
                wheelFrictionCurve.extremumValue = 1f;
                wheelFrictionCurve.asymptoteSlip = 0.5f;
                wheelFrictionCurve.asymptoteValue = 0.9f;
                wheelFrictionCurve.stiffness = 1f;

                wheel.sidewaysFriction = wheelFrictionCurve;
            }
            //rear wheels
            if (wheel.transform.localPosition.z < 0)
            {
                WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();
                wheelFrictionCurve.extremumSlip = 0.2f;
                wheelFrictionCurve.extremumValue = 1f;
                wheelFrictionCurve.asymptoteSlip = 0.5f;
                wheelFrictionCurve.asymptoteValue = 0.75f;
                wheelFrictionCurve.stiffness = 1f;

                wheel.sidewaysFriction = wheelFrictionCurve;
            }
        }
    }

    private void RevertFriction()
    {
        int i = 0;
        foreach (WheelCollider wheel in m_Wheels) wheel.sidewaysFriction = preFrictionCurve[i++];
    }

    private void FixedUpdate()
    {

        if (isDrifting)
        {
            
            //print(m_Wheels[0].sidewaysFriction.asymptoteValue + "    " + m_Wheels[0].sidewaysFriction.extremumValue);

        }
        else
        {
            m_rigidbody.drag = m_rigidbody.velocity.magnitude / 150;
            
        }
        //m_rigidbody.AddForce(Vector3.down* m_rigidbody.velocity.magnitude , ForceMode.Force);
    }
    //void OnGUI()
    //{
    //    if(isDrifting)
    //    GUILayout.Label("angle: " + Vector3.Angle(m_rigidbody.velocity.normalized, m_rigidbody.transform.forward));
    //}
}
