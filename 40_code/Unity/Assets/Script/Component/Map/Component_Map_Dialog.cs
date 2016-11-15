using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_Map_Dialog : GameComponent {

    public Action CloseEvent;

    public UILabel TitleLabel;
    public UILabel MsgLabel;
    public UILabel NextLabel;

    private Animation anim;
    private Queue<string> msgQueue;
    private Queue<string> msgBuffer;
    private string next;
    private string end;
    private bool isLock;

    public override void Init()
    {
        base.Init();
        isLock = true;
        transform.localPosition = new Vector3(0, 500, 0);
        anim = GetComponent<Animation>();
        msgQueue = new Queue<string>();
        msgBuffer = new Queue<string>();
        next = TableManager.GetString("STR_UI_NEXT");
        end = TableManager.GetString("STR_UI_CLOSE");
    }

    public void Show(string name, List<string> msgs)
    {
        msgQueue.Clear();
        TitleLabel.text = name;
        MsgLabel.text = "";
        foreach(string msg in msgs)
        {
            msgQueue.Enqueue(msg);
        }

        isEnd = false;
        GetMsg();
        if(anim != null) anim.Play("anim_map_dialog_01");
    }

    public override void Hide()
    {
        if (anim != null) anim.Play("anim_map_dialog_02");
    }

    public void OnHide()
    {
        if (CloseEvent != null) CloseEvent();
    }

    void SetMsg(string msg)
    {
        MsgLabel.text = msg;
    }

    void OnClick()
    {
        if (isLock) return;
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        GetMsg();
    }

    void LateUpdate()
    {
        if (isLock) return;
        ShowMsg();
    }

    void ShowMsg()
    {
        if (msgBuffer.Count > 0)
        {
            string msg = msgBuffer.Dequeue();
            SetMsg(msg);
            if (msgQueue.Count > 0 || msgBuffer.Count > 0) NextLabel.text = next;
            else NextLabel.text = end;
        }
        else if (isEnd)
        {
            isLock = true;
            Hide();
        }
    }

    bool isEnd = false;
    void GetMsg()
    {
        if (msgQueue.Count > 0) msgBuffer.Enqueue(msgQueue.Dequeue());
        else
        {
            isEnd = true;
        }
    }

    public void OnShow()
    {
        isLock = false;
    }
}
