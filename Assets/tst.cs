using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class tst : MonoBehaviour
{
    public GameObject pre_bend;
    public GameObject pre_whiteStraight;
    public GameObject pre_redStraight;
    public GameObject pre_curved;

    private void Start()
    {
        pre_bend = (GameObject)Resources.Load("Prefabs/Tracks/MT_Turn");
        pre_whiteStraight = (GameObject)Resources.Load("Prefabs/Tracks/MT_Road_02");
        pre_redStraight = (GameObject)Resources.Load("Prefabs/Tracks/MT_Road_01");
        pre_curved = (GameObject)Resources.Load("Prefabs/Tracks/CustomCurve");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnManager.Instance.printlist();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1)){
            print("gamesave");
            string json;
            SaveFile save = new SaveFile();
            save.trackList = new List<track>();
            var m_trackList = SpawnManager.Instance.getAllTrack();
            var m_typeList = SpawnManager.Instance.getTypeList();
            int i = 0;
            for (i = 0; i < m_trackList.Count; i++)
            {
                track temp;
                temp.type = (trackType)m_typeList[i];
                temp.transfrom = new m_transform();
                temp.transfrom.position = m_trackList[i].transform.position;
                temp.transfrom.rotation = m_trackList[i].transform.rotation;
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
            FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
            bf.Serialize(file, json);
            file.Close();
            
            foreach (var item in m_trackList)
            {
                Destroy(item);
            }
            SpawnManager.Instance.ClearTrackList();
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);

                string json = (string)bf.Deserialize(file);
                SaveFile save = JsonUtility.FromJson<SaveFile>(json);
                file.Close();

                foreach(track item in save.trackList)
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
                    obj = Instantiate(prefabObject);
                    if(item.type == trackType.curved)
                    {
                        obj.GetComponent<ModifyPoints>().setAllWayPoints(item.extraList);
                    }
                    obj.transform.position = item.transfrom.position;
                    obj.transform.rotation = item.transfrom.rotation;
                    SpawnManager.Instance.addTracks(obj, (int)item.type);
                }

                Debug.Log("Game Loaded");
            }
            else
            {
                Debug.Log("No game saved!");
            }
            
        }
    }
}
