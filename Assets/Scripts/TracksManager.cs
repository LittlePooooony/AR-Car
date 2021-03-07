using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracksManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int n = 0;
        if(GameInfo.Instance) n = GameInfo.Instance.saveSlot;
        GetComponent<TrackFactory>().LoadTracks(n);
    }
}
