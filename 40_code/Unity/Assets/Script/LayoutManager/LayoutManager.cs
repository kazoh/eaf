using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

using Kazoh.Table;

public class LayoutManager : MonoBehaviour
{
    private static bool isLock;

    void Awake()
    {
        Init();
    }

    public virtual void Init()
    {
        /* 테이블 데이터 로딩 */
        try
        {
            GameProcess.LoadTable();
            StartCoroutine(Loading());
        }
        catch (GameException e)
        {
            Debug.LogError(e.Msg);
            GameProcess.ShowError(e);
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
            Quit();
        }
    }

    protected virtual IEnumerator Loading()
    {
        yield return null;        
    }

    public void OnFinishLoading()
    {
        GameProcess.HideLoading();
    }

    protected void Quit()
    {
        Application.Quit();
    }

    protected void LoadScene(string scene)
    {
        StartCoroutine(LoadingScene(scene));
    }

    IEnumerator LoadingScene(string scene)
    {
        GameProcess.ShowLoading();

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(scene);
    }

    public static void Lock()
    {
        isLock = true;
    }

    public static void UnLock()
    {
        isLock = false;
    }

    public static bool IsLock()
    {
        return isLock;
    }

}
