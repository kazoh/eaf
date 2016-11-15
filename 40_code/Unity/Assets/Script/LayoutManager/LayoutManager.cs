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
            GameProcess.ShowError(e, "오류", "확인");
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

    protected virtual void Quit()
    {
        string title = TableManager.GetString("STR_TITLE_EXIT");
        string msg = TableManager.GetString("STR_MSG_EXIT");
        string text1 = TableManager.GetString("STR_UI_YES");
        string text2 = TableManager.GetString("STR_UI_NO");
        GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
        {
            try
            {
                GameProcess.Instance.Quit();
            }
            catch (GameException e)
            {
                GameProcess.ShowError(e);
            }
            catch (Exception e)
            {
                GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            }

        }, null);
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
