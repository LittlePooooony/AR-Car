using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenu : Menu
{
    public void ResetCar()
    {
        car.transform.rotation = carOriDir;
        car.transform.position = carOriPos;
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        settingControl.UnShowMenu();
    }
    public void Restart()
    {
        settingControl.UnShowMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Tips()
    {

    }
    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
