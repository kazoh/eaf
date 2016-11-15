 using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

using Kazoh.Table;

public class LayoutManager_UI_LogIn : LayoutManager {

    public Component_UI_LogIn LogInUI;

    private string keyId;
    private string keyPw;

    protected override IEnumerator Loading()
    {
        yield return base.Loading();

        /* 키 설정 */
        keyId = EncryptedPlayerPrefs.keys[0];
        keyPw = EncryptedPlayerPrefs.keys[1];

        /* 배경음 적용 */
        GameProcess.PlaySound(SOUND_EFFECT.BGM, "bgm_01");

        /* 로그인 UI 초기화 */
        LogInUI.Init(keyId, keyPw);
        LogInUI.SignInEvent += SignIn;
        LogInUI.CreateEvent += CreateAccount;
        LogInUI.FindEvent += FindPw;

        OnFinishLoading();
    }

    void SignIn(string _email, string _pw)
    {
        try
        {
            DBManager.SignIn(_email, _pw, delegate (int _guid, string _timeStr)
            {
                if (_guid > 0)
                {
                    EncryptedPlayerPrefs.SetString(keyId, _email);
                    EncryptedPlayerPrefs.SetString(keyPw, _pw);
                    GameProcess.ShowLoading();
                    StartCoroutine(LoadingDate(_guid, _timeStr));
                }
                else GameProcess.ShowError(new GameException(GameException.ErrorCode.InvalidEmailOrPassward));
            });
        }
        catch (GameException e)
        {
            GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
        }
    }

    void CreateAccount(string _email, string _pw)
    {
        try
        {
            DBManager.CreateAccount(_email, _pw, delegate (int _guid, string _timeStr)
            {
                if (_guid > 0)
                {
                    GameProcess.ShowLoading();
                    EncryptedPlayerPrefs.SetString(keyId, _email);
                    EncryptedPlayerPrefs.SetString(keyPw, _pw);
                    StartCoroutine(LoadingDate(_guid, _timeStr));
                }
                else GameProcess.ShowError(new GameException(GameException.ErrorCode.ExistEmail));
            });
        }
        catch (GameException e)
        {
            GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown,e.Message));
        }
    }

    void FindPw(string _email)
    {
        try
        {
            string title = TableManager.GetString("STR_TITLE_FIND_PW");
            string msg = TableManager.GetString("STR_MSG_FIND_PW");
            string text1 = TableManager.GetString("STR_UI_YES");
            string text2 = TableManager.GetString("STR_UI_NO");

            GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1,text2, delegate ()
            {
                string tempPw = "";
                for (int i = 0; i < 4; ++i)
                {
                    tempPw += (char)UnityEngine.Random.Range('A', 'Z');
                    tempPw += (char)UnityEngine.Random.Range('a', 'z');
                }

                title = TableManager.GetString("STR_MAIL_SUB");
                msg = TableManager.GetString("STR_MAIL_MSG");

                DBManager.FindPw(_email, tempPw, title, msg, delegate (bool _isFail)
                {
                    if (_isFail)
                    {
                        GameProcess.ShowError(new GameException(GameException.ErrorCode.NotExistEmail));
                    }
                    else
                    {
                        title = TableManager.GetString("STR_TITLE_MAIL_PW");
                        msg = TableManager.GetString("STR_MSG_MAIL_PW");
                        text1 = TableManager.GetString("STR_UI_OK");
                        GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                    }
                });

            }, null);
        }
        catch (GameException e)
        {
            GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
        }
    }

    IEnumerator LoadingDate(int _guid,string _timeStr)
    {
        yield return null;

        /* 테이블 데이터 로딩 */
        try
        {
            GameProcess.GetGameDataManager().Load(_guid,_timeStr,delegate()
            {
                GameProcess.GetMissionManager().Load();

                if (GameProcess.GetGameDataManager().HasNoCharacter) LoadScene("CharacterScene");
                else LoadScene("GameScene");
            });
        }
        catch (GameException e)
        {
            GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
        }
    }

    protected override void Quit()
    {
        base.Quit();
    }
}
