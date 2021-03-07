using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotsUpdate : MonoBehaviour
{
    private static SaveSlotsUpdate _instance;

    public static SaveSlotsUpdate Instance
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
    public GameObject[] saveSlots;
    // Start is called before the first frame update
    void Start()
    {
        UpdataImage();
    }

    public void UpdataImage()
    {
        for (int i = 0; i < 4; i++)
        {
            if (File.Exists(Application.persistentDataPath + "/Custom" + i + ".png"))
            {
                FileStream file = File.Open(Application.persistentDataPath + "/Custom" + i + ".png", FileMode.Open);

                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);

                file.Close();
                file.Dispose();

                Texture2D texture = new Texture2D(0, 0);
                texture.LoadImage(bytes);
                float width = texture.width;
                float height = texture.height;
                float temp = Mathf.Min(width, height);
                saveSlots[i].GetComponent<Image>().sprite = Sprite.Create(texture, new Rect((width - temp) / 2, (height - temp) / 2, (width + temp) / 2, (height + temp) / 2), new Vector2(0.5f, 0.5f));
                saveSlots[i].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            }
        }
    }
}
