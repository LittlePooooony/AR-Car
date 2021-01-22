using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum trackType
{
    bend,
    whiteStraight,
    redStraid,
    curved
};
[System.Serializable]
public struct m_transform
{
    public Vector3 position;
    public Quaternion rotation;

    public m_transform(Vector3 pos,Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
}
[System.Serializable]
public struct track
{
    public trackType type;
    public m_transform transfrom;
    public List<m_transform> extraList;
}
[System.Serializable]
public class SaveFile
{
    public List<track> trackList;

}
