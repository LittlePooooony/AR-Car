using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class FirstSceneMenu : MonoBehaviour
{
    private enum Mode
    {
        normal,
        AR
    }

    private Animator animator;
    private Mode mode = Mode.normal;
    private bool isChangingMode = false;

    public GameObject CanvasSelect;
    public GameObject WarningPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        CanvasSelect = GameObject.Find("CanvasSelect");
        CanvasSelect.SetActive(false);
        WarningPanel.GetComponent<CanvasGroup>().alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isChangingMode && Input.GetMouseButtonDown(0))
        {
            animator.SetBool("out", false);
            StartCoroutine(ChangingModeTime(false));
        }
    }
    public void StartGame()
    {
        CanvasSelect.SetActive(true);
    }
    public void CoustomTracks()
    {
        if (mode == Mode.normal) SceneManager.LoadScene(3);
        else SceneManager.LoadScene(4);
    }
    public void Setting()
    {

    }
    public void ChangeMode()
    {
        if (!isChangingMode)
        {
            animator.SetBool("out", true);
            isChangingMode = true;
        }
        
    }
    public void SetModeNor()
    {
        mode = Mode.normal;
    }
    public void SetModeAR()
    {
        mode = Mode.AR;

    }
    public void GameQuit()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

        Debug.Log("编辑状态游戏退出");

#else

            Application.Quit();

#endif
    }

    private IEnumerator ChangingModeTime(bool value)
    {
        yield return new WaitForSeconds(0.1f);
        isChangingMode = value;
    }

    public void PlayOfficalTrack(int n)
    {
        if (mode == Mode.normal)
            switch (n)
        {
            case 0: SceneManager.LoadScene("PlayScene"); break;
            case 1: SceneManager.LoadScene("PlayScene 1"); break;
        }
    }

    public void PlayCustomTrack(int n)
    {
        if(mode == Mode.normal)
        if (GameInfo.Instance.GetSaveSlotBool(n))
        {
            GameInfo.Instance.saveSlot = n;
            if (mode == Mode.normal) SceneManager.LoadScene("CustomPlay");
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(Warning());
        }
    }
    public void PlayReturn()
    {
        CanvasSelect.SetActive(false);
    }
    private IEnumerator Warning()
    {
        float temp = 0;
        var a = WarningPanel.GetComponent<CanvasGroup>();
        while (temp <= 1)
        {
            temp += Time.deltaTime;
            a.alpha = temp;
            yield return 0;
        }
        while (temp >= 0)
        {
            temp -= Time.deltaTime;
            a.alpha = temp;
            yield return 0;
        }
    }
}
