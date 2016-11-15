using UnityEngine;
using System.Collections;
using System;

public class Component_Map_Timer : GameComponent
{
    public Action TimeOverEvent;
    protected Component_Map map;

    public UILabel SecLabel;
    public UILabel DotLabel;
    public UILabel MilisceLabel;

    public float TotTime { get; private set; }
    public float CurTime { get; private set; }

    private bool isStop;
    private bool isPause = true;

    public void Init(Component_Map _map, int time = 10)
    {
        Init();
        map = _map;
        TotTime = time;
        CurTime = 0f;
        SetTimer();
    }

    public void Stop()
    {
        isStop = true;
    }

    public void Pause(bool _pause)
    {
        isPause = _pause;
    }

    void LateUpdate()
    {
        if (isStop) return;
        if (isPause) return;

        CurTime += Time.deltaTime;
        SetTimer();
        if (TotTime < CurTime) TimeOver();
    }

    void TimeOver()
    {
        Stop();
        if (TimeOverEvent != null) TimeOverEvent();
    }

    void SetTimer()
    {
        float t = TotTime - CurTime;
        int t1 = (int)t;
        int t2 = (int)(t * 100) % 100;
        if(t1 > 5)
        {
            SecLabel.color = Color.cyan;
            DotLabel.color = Color.cyan;
            MilisceLabel.color = Color.cyan;
        }
        else
        {
            SecLabel.color = Color.red;
            DotLabel.color = Color.red;
            MilisceLabel.color = Color.red;
        }
        SecLabel.text = "" + t1;
        MilisceLabel.text = string.Format("{0:00}", Mathf.Max(t2, 0));
    }
}
