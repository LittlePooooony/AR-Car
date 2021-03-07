using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARPlayMenu : Menu
{
    public new void Start()
    {
        base.Start();
    }
    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
