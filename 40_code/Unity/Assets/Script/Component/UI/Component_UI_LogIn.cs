using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_LogIn : GameComponent
{
    public Action<string,string> SignInEvent;
    public Action<string,string> CreateEvent;
    public Action<string> FindEvent;

    public UILabel LabelTitle;
    public UILabel LabelMsg;
    public UILabel LabelEmail;
    public UILabel LabelPw;
    public UIInput InputEmail;
    public UIInput InputPw;
    public UILabel LabelFind;
    public UILabel LabelSignIn;
    public UILabel LabelCreate;

    private bool isLock;
    private string keyId;
    private string keyPw;

    public void Init(string _keyId, string _keyPw)
    {
        base.Init();
        keyId = _keyId;
        keyPw = _keyPw;
        SetConstantUI();
        Show();
    }

    public void OnClick_SignIn()
    {
        if (isLock) return;
        isLock = true;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);

        if (InputEmail.value.Equals(string.Empty))
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.NoEmail));
        }
        else if (InputPw.value.Equals(string.Empty))
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.NoPw));
        }
        else if (SignInEvent != null)
        {
            SignInEvent(InputEmail.value, InputPw.value);
        }

        isLock = false;
    }

    public void OnClick_Create()
    {
        if (isLock) return;
        isLock = true;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);

        if (InputEmail.value.Equals(string.Empty))
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.NoEmail));
        }
        else if (InputPw.value.Equals(string.Empty))
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.NoPw));
        }
        else if (!CheckEmail(InputEmail.value))
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.InvalidEmailFormat));
        }
        else if (!CheckPw(InputPw.value))
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.InvalidPwFormat));
        }
        else if (CreateEvent != null)
        {
            string title = TableManager.GetString("STR_TITLE_NOTICE");
            string msg = TableManager.GetString("STR_MSG_WARNING");
            string text1 = TableManager.GetString("STR_UI_YES");
            string text2 = TableManager.GetString("STR_UI_NO");

            GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate ()
            {
                CreateEvent(InputEmail.value, InputPw.value);
            }, null, NGUIText.Alignment.Left);
        }

        isLock = false;
    }

    public void OnClick_Find()
    {
        if (isLock) return;
        isLock = true;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);

        if (InputEmail.value.Equals(string.Empty))
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.NoEmail));
        }
        else if (!CheckEmail(InputEmail.value))
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.InvalidEmailFormat));
        }
        else if (CreateEvent != null)
        {
            FindEvent(InputEmail.value);
        }

        isLock = false;
    }

    void SetConstantUI()
    {
        LabelTitle.text = TableManager.GetString("STR_TITLE_LOGIN");
        LabelMsg.text = TableManager.GetString("STR_MSG_LOGIN");
        LabelSignIn.text = TableManager.GetString("STR_UI_LOGIN");
        LabelCreate.text = TableManager.GetString("STR_UI_NEW_ACCOUNT");
        LabelFind.text = TableManager.GetString("STR_UI_FIND_PW");

        if (EncryptedPlayerPrefs.HasKey(keyId)) InputEmail.value = EncryptedPlayerPrefs.GetString(keyId);
        if (EncryptedPlayerPrefs.HasKey(keyPw)) InputPw.value = EncryptedPlayerPrefs.GetString(keyPw);
    }

    bool CheckEmail(string _email)
    {
        int idx_1 = _email.IndexOf("@");
        int idx_2 = _email.IndexOf(".");

        if (idx_1 > 0 && idx_2 > 0 && idx_2 > idx_1) return true;
        return false;
    }

    bool CheckPw(string _pw)
    {
        if (_pw.Length < 6) return false;
        char[] cArray = _pw.ToCharArray();
        for(int i=0; i<cArray.Length; ++i)
        {
            if ((cArray[i] >= '0' && cArray[i] <= '9') || (cArray[i] >= 'A' && cArray[i] <= 'z')) continue;
            return false;
        }

        return true;
    }
}
