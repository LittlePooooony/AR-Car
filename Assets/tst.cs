using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Mx.Log;
using UnityEngine.UI;

public class tst : MonoBehaviour
{
    public GameObject UI;
    public GameObject ima;
    private void Start()
    {
        DebugManager.Instance.RemoteDebug();
        UI = GameObject.Find("UI");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/Custom0.png", FileMode.Open);

            byte[] bytes = new byte[file.Length];
            file.Read(bytes, 0, (int)file.Length);

            file.Close();
            file.Dispose();

            Texture2D texture = new Texture2D(0,0);
            texture.LoadImage(bytes);
            float width = texture.width;
            float height = texture.height;
            ima.GetComponent<Image>().sprite = Sprite.Create(texture,new Rect(0,0, Screen.width, Screen.height),new Vector2(0.5f,0.5f));
            ima.GetComponent<RectTransform>().sizeDelta = new Vector2(width/height*100,100);
        }
    }
    
}
