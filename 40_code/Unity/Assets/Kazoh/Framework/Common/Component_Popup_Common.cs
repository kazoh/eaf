using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Component_Popup_Common : GameComponent {

    public class PopupInfo
    {
        public NoticeType Type;
        public string Title;
        public string Msg;
        public string Text1;
        public string Text2;
        public Action BtnCallback1;
        public Action BtnCallback2;
        public NGUIText.Alignment Alignment;

        public PopupInfo(NoticeType type, string title, string msg, string text1, string text2, Action callback1, Action callback2, NGUIText.Alignment alignment)
        {
            Type = type;
            Title = title;
            Msg = msg;
            Text1 = text1;
            Text2 = text2;
            BtnCallback1 = callback1;
            BtnCallback2 = callback2;
            Alignment = alignment;
        }
    }

    Action ClickEvent_Btn1;
    Action ClickEvent_Btn2;

    public GameObject Root;
    public GameObject RootOkButton;
    public GameObject RootYesNoButton;
    public UILabel LabelTitle;
    public UILabel LabelMsg;
    public UILabel LabelOk;
    public UILabel LabelYes;
    public UILabel LabelNo;

    private Animation anim;
    private bool isLock = false;

    private Queue<PopupInfo> popupQueue;
    private PopupInfo curPopup;

    const string DEFAULT_TITLE = "▶ N O T I C E";

    public override void Init()
    {
        base.Init();
        anim = GetComponent<Animation>();
        popupQueue = new Queue<PopupInfo>();
        Root.SetActive(false);
        StartCoroutine(CheckPopup());
    }

    IEnumerator CheckPopup()
    {
        while(true)
        {
            yield return null;
            if (curPopup == null && popupQueue.Count > 0)
            {
                curPopup = popupQueue.Dequeue();
                Show(curPopup);
            }
        }
    }

    public override void Show()
    {
        Root.SetActive(true);
        //if(anim != null) anim.Play("anim_ui_popup_01");
    }

    public void Show(NoticeType type, string title, string msg, string text, Action callback, NGUIText.Alignment alignment = NGUIText.Alignment.Automatic)
    {
        Show(type, title, msg, text, callback, "", null, alignment);
    }

    public void Show(NoticeType type, string title, string msg, string text1, Action callback1, string text2, Action callback2, NGUIText.Alignment alignment = NGUIText.Alignment.Automatic)
    {
        popupQueue.Enqueue(new PopupInfo(type, title, msg, text1, text2, callback1, callback2, alignment));
    }

    public void Show(PopupInfo info)
    {
        LabelTitle.text = string.IsNullOrEmpty(info.Title) ? DEFAULT_TITLE : info.Title;
        LabelMsg.alignment = info.Alignment;
        LabelMsg.text = info.Msg;

        ClickEvent_Btn1 = info.BtnCallback1;
        ClickEvent_Btn2 = info.BtnCallback2;

        switch (info.Type)
        {
            case NoticeType.NONE:
                RootYesNoButton.SetActive(false);
                RootOkButton.SetActive(false);
                gameObject.SetActive(true);
                StartCoroutine(DisablePopup());
                return;
            case NoticeType.OK:
                LabelOk.text = info.Text1;
                RootYesNoButton.SetActive(false);
                RootOkButton.SetActive(true);
                break;
            case NoticeType.YES_NO:
                LabelYes.text = info.Text1;
                LabelNo.text = info.Text2;
                RootYesNoButton.SetActive(true);
                RootOkButton.SetActive(false);
                break;
        }

        Show();
    }

    public override void Hide()
    {
        //if (anim != null && !anim.isPlaying)
        //{
        //    anim.Play("anim_ui_popup_02");
        //}
        Root.SetActive(false);
        HideCallback();
    }

    public void HideCallback()
    {
        curPopup = null;
    }

    //IEnumerator CheckAnim()
    //{
    //    while(anim.isPlaying)
    //    {
    //        yield return null;
    //    }

    //    base.Hide();
    //}

    IEnumerator DisablePopup()
    {
        yield return new WaitForSeconds(1f);
        if (ClickEvent_Btn1 != null)
        {
            ClickEvent_Btn1();
            ClickEvent_Btn1 = null;
            ClickEvent_Btn2 = null;
        }
        Hide();
    }

    public void OnClick_Btn1()
    {
        if (isLock) return;
        isLock = true;
        Hide();
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickEvent_Btn1 != null)
        {
            ClickEvent_Btn1();
            ClickEvent_Btn1 = null;
            ClickEvent_Btn2 = null;
        }
        isLock = false;
    }

    public void OnClick_Btn2()
    {
        if (isLock) return;
        isLock = true;
        Hide();
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (ClickEvent_Btn2 != null)
        {
            ClickEvent_Btn2();
            ClickEvent_Btn1 = null;
            ClickEvent_Btn2 = null;
        }
        isLock = false;
    }
}

public enum NoticeType
{
    NONE,
    OK,
    YES_NO,
}
