using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_SettingPopup : GameComponent
{
    public Action<string> EditInitialEvent;
    public Action EditPwEvent;
    public Action SavedEvent;

    public UILabel UserInfoTitleLabel;
    public UILabel InitialLabel;
    public UIInput InitialInput;
    public UILabel EditBtnLabel;
    public UILabel MembershipLabel;
    public UILabel MembershipValueLabel;
    public UIInput CurPwInput;
    public UIInput NewPwInput;
    public UIInput ConformPwInput;
    public UILabel CurPwLabel;
    public UILabel NewPwLabel;
    public UILabel ConformPwLabel;
    public UILabel ChangeBtnLabel;

    public UILabel SettingTitleLabel;
    public UILabel BgmLabel;
    public UILabel EffectLabel;
    public UILabel SaveBtnLabel;
    public UILabel CancelBtnLabel;

    public UISlider BgmSlider;
    public UISlider EffectSlider;

    private bool isLock;

    public override void Init()
    {
        base.Init();
        SetConstantUI();
    }

    public override void Show()
    {
        SetSettingPopup();
        base.Show();
    }

    public void SetVolumnBgm()
    {
        if (UIProgressBar.current != null) GameProcess.Instance.SetVolumnBGM(UIProgressBar.current.value);
    }

    public void SetVolumnEffect()
    {
        if (UIProgressBar.current != null) GameProcess.Instance.SetVolumnEffect(UIProgressBar.current.value);
    }

    public void OnClick_SaveSetting()
    {
        if (!gameObject.activeSelf) return;

        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        GameSettingManager gsManager = GameProcess.GetSettingManager();
        if(gsManager != null)
        {
            float bgmVol = BgmSlider.value;
            float effectVol = EffectSlider.value;

            gsManager.SetGameSetting(bgmVol, effectVol);
        }

        OnSave();
        Hide();
    }

    public void OnClick_Close()
    {
        if (gameObject.activeSelf)
        {
            Hide();
            GameProcess.PlaySound(SOUND_EFFECT.CLICK);

            if (PlayerPrefs.HasKey("BGM_VOLUME")) GameProcess.Instance.SetVolumnBGM(PlayerPrefs.GetFloat("BGM_VOLUME"));
            if (PlayerPrefs.HasKey("EFFECT_VOLUME")) GameProcess.Instance.SetVolumnEffect(PlayerPrefs.GetFloat("EFFECT_VOLUME"));
        }
    }

    public void OnClick_EditInitial()
    {
        if (isLock) return;
        isLock = true;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        string initial = InitialInput.value;
        if (!initial.Equals(GameProcess.GetGameDataManager().GetUserInitial()))
        {
            GameProcess.GetGameDataManager().SetUserInitial(initial);
            GameProcess.GetGameDataManager().Save();
        }

        string title = TableManager.GetString("STR_TITLE_NOTICE");
        string msg = TableManager.GetString("STR_MSG_EDIT_INITIAL");
        string text = TableManager.GetString("STR_UI_OK");
        GameProcess.ShowPopup(NoticeType.OK, title, msg, text, null);
        isLock = false;
    }

    public void OnClick_ChangePw()
    {
        try
        {
            if (isLock) return;
            isLock = true;
            GameProcess.PlaySound(SOUND_EFFECT.CLICK);
            string curPw = CurPwInput.value;
            string newPw = NewPwInput.value;
            if (string.IsNullOrEmpty(curPw)) throw new GameException(GameException.ErrorCode.EmptyCurPw);
            if (string.IsNullOrEmpty(newPw)) throw new GameException(GameException.ErrorCode.EmptyNewPw);
            if (string.IsNullOrEmpty(ConformPwInput.value)) throw new GameException(GameException.ErrorCode.EmptyConformPw);
            if (curPw.Equals(newPw)) throw new GameException(GameException.ErrorCode.EqualsNewPw);
            if (!newPw.Equals(ConformPwInput.value)) throw new GameException(GameException.ErrorCode.NotEqualsNewPw);
            if (!CheckPw(newPw)) throw new GameException(GameException.ErrorCode.InvalidPwFormat);

            string title = TableManager.GetString("STR_TITLE_CHANGE_PW");
            string msg = TableManager.GetString("STR_MSG_CHANGE_PW");
            string text1 = TableManager.GetString("STR_UI_YES");
            string text2 = TableManager.GetString("STR_UI_NO");
            GameProcess.ShowPopup(NoticeType.YES_NO, title, msg, text1, text2, delegate()
            {
                int id = GameProcess.GetGameDataManager().GetGUID();
                DBManager.ChangePw(id, curPw, newPw, delegate(bool _isFail)
                {
                    if (_isFail)
                    {
                        GameProcess.ShowError(new GameException(GameException.ErrorCode.InvalidPassward));
                    }
                    else
                    {
                        EncryptedPlayerPrefs.SetString(EncryptedPlayerPrefs.userKeys[1], newPw);
                        title = TableManager.GetString("STR_TITLE_CHANGE_PW");
                        msg = TableManager.GetString("STR_MSG_CHANGED_PW");
                        text1 = TableManager.GetString("STR_UI_OK");
                        GameProcess.ShowPopup(NoticeType.OK, title, msg, text1, null);
                    }
                    SetSettingPopup();
                });
            }, null);            
        }
        catch (GameException e)
        {
            GameProcess.ShowError(e);
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e);
#endif
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown,e.Message));
        }
        finally
        {
            isLock = false;
        }

    }

    void SetConstantUI()
    {
        UserInfoTitleLabel.text = TableManager.GetString("STR_TITLE_USER_INFO");
        InitialLabel.text = TableManager.GetString("STR_UI_INITIAL");
        EditBtnLabel.text = TableManager.GetString("STR_UI_CHANGE");
        MembershipLabel.text = TableManager.GetString("STR_UI_MEMBERSHIP");
        ChangeBtnLabel.text = TableManager.GetString("STR_UI_CHANGE_PW");

        SettingTitleLabel.text = TableManager.GetString("STR_TITLE_SETTING");
        BgmLabel.text = TableManager.GetString("STR_UI_BGM");
        EffectLabel.text = TableManager.GetString("STR_UI_EFFECT");
        SaveBtnLabel.text = TableManager.GetString("STR_UI_SAVE");
        CancelBtnLabel.text = TableManager.GetString("STR_UI_CANCEL");

        CurPwLabel.text = TableManager.GetString("STR_UI_CUR_PW");
        NewPwLabel.text = TableManager.GetString("STR_UI_NEW_PW");
        ConformPwLabel.text = TableManager.GetString("STR_UI_CONFORM_PW");
    }

    void SetSettingPopup()
    {
        GameSettingManager gsManager = GameProcess.GetSettingManager();
        if (gsManager != null)
        {
            BgmSlider.value = gsManager.BgmVolume;
            EffectSlider.value = gsManager.EffectVolume;
        }
        else
        {
#if UNITY_EDITOR || DEBUG_MODE
            Debug.LogWarning("GameSettingManager가 생성되지 않아 초기 값으로 설정합니다.");
#endif
            BgmSlider.value = 1;
            EffectSlider.value = 1;
        }

        InitialInput.value = GameProcess.GetGameDataManager().GetUserInitial();
        string memStr = "STR_UI_MEM_GRADE_" + GameProcess.GetGameDataManager().GetMembership();
        MembershipValueLabel.text = TableManager.GetString(memStr);

        CurPwInput.value = "";
        NewPwInput.value = "";
        ConformPwInput.value = "";
    }

    void OnSave()
    {
        if (SavedEvent != null) SavedEvent();
    }

    bool CheckPw(string _pw)
    {
        if (_pw.Length < 6) return false;
        char[] cArray = _pw.ToCharArray();
        for (int i = 0; i < cArray.Length; ++i)
        {
            if ((cArray[i] >= '0' && cArray[i] <= '9') || (cArray[i] >= 'A' && cArray[i] <= 'z')) continue;
            return false;
        }

        return true;
    }
}
