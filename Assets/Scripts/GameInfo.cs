using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    private static GameInfo _instance;

    public static GameInfo Instance
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

    public bool[] saveSlots = new bool[4];

    public int saveSlot = 0;

    private void Start()
    {
        DontDestroyOnLoad(this);

        for(int i = 0; i < 4; i++)
        {
            if (File.Exists(Application.persistentDataPath + "/gamesave.save" + i)) saveSlots[i] = true;
        }
    }

    public bool GetSaveSlotBool(int n)
    {
        return saveSlots[n];
    }
    public void UpdateSlot()
    {
        for (int i = 0; i < 4; i++)
        {
            if (File.Exists(Application.persistentDataPath + "/gamesave.save" + i)) saveSlots[i] = true;
        }
    }
}
