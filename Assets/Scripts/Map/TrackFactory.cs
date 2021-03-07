using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class TrackFactory : MonoBehaviour
{
    private static TrackFactory _instance;
    public static TrackFactory Instance
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

    public GameObject UI;

    public GameObject pre_bend;
    public GameObject pre_whiteStraight;
    public GameObject pre_redStraight;
    public GameObject pre_curved;
    private void Start()
    {
        pre_bend = (GameObject)Resources.Load("Prefabs/Tracks/Bend");
        pre_whiteStraight = (GameObject)Resources.Load("Prefabs/Tracks/WhiteStraight");
        pre_redStraight = (GameObject)Resources.Load("Prefabs/Tracks/RedStraight");
        pre_curved = (GameObject)Resources.Load("Prefabs/Tracks/CustomCurve");

        UI = GameObject.Find("UI");
    }
    public void SaveAllTracks(int num = 0)
    {
        print("gamesave");
        string json;
        SaveFile save = new SaveFile();
        save.trackList = new List<track>();
        List<GameObject> m_trackList = SpawnManager.Instance.getAllTrack();
        var m_typeList = SpawnManager.Instance.getTypeList();
        int i = 0;
        for (i = 0; i < m_trackList.Count; i++)
        {
            track temp;
            temp.type = (trackType)m_typeList[i];
            temp.transfrom = new m_transform();
            temp.transfrom.position = m_trackList[i].transform.localPosition;
            temp.transfrom.rotation = m_trackList[i].transform.localRotation;
            if (temp.type == trackType.curved)
            {
                List<m_transform> templist = new List<m_transform>();
                foreach (Transform item in m_trackList[i].transform)
                {
                    templist.Add(new m_transform(item.localPosition, item.rotation));
                }
                temp.extraList = templist;
            }
            else temp.extraList = new List<m_transform>();
            save.trackList.Add(temp);
        }
        json = JsonUtility.ToJson(save);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save"+num);
        bf.Serialize(file, json);
        file.Close();

        StartCoroutine(CaptureScreenshot(num, m_trackList));

    }


    public Texture2D ScreenShot(Camera camera, Rect rect,int n)
    {
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.persistentDataPath + "/Custom" + n+".png";//存放路径
        System.IO.File.WriteAllBytes(filename, bytes);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();//刷新，这步很关键，否则后面调用图片时没有。
#endif
        return screenShot;
    }
    IEnumerator CaptureScreenshot(int n, List<GameObject> m_trackList)
    {
        foreach(CanvasGroup item in UI.GetComponentsInChildren<CanvasGroup>())
        {
            item.alpha = 0;
        }
        yield return new WaitForEndOfFrame();
        ScreenShot(Camera.main, new Rect(0, 0, (int)Screen.width, (int)Screen.height),n);
        foreach (CanvasGroup item in UI.GetComponentsInChildren<CanvasGroup>())
        {
            item.alpha = 1;
        }
        foreach (var item in m_trackList)
        {
            Destroy(item);
        }
        SpawnManager.Instance.ClearTrackList();

        SaveSlotsUpdate.Instance.UpdataImage();
    }

    public void LoadTracks(int num = 0)
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save" + num))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save" + num, FileMode.Open);

            string json = (string)bf.Deserialize(file);
            SaveFile save = JsonUtility.FromJson<SaveFile>(json);
            file.Close();

            foreach (track item in save.trackList)
            {
                GameObject obj;
                GameObject prefabObject = pre_bend;
                switch (item.type)
                {
                    case trackType.bend: prefabObject = pre_bend; break;
                    case trackType.whiteStraight: prefabObject = pre_whiteStraight; break;
                    case trackType.redStraid: prefabObject = pre_redStraight; break;
                    case trackType.curved: prefabObject = pre_curved; break;
                }
                GameObject origin = GameObject.Find("origin");
                obj = Instantiate(prefabObject);
                if (origin) obj.transform.parent = origin.transform;
                if (item.type == trackType.curved)
                {
                    obj.GetComponent<ModifyPoints>().setAllWayPoints(item.extraList);
                }
                obj.transform.localPosition = item.transfrom.position;
                obj.transform.localRotation = item.transfrom.rotation;
                if (SpawnManager.Instance) SpawnManager.Instance.addTracks(obj, (int)item.type);
            }

            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }
}
