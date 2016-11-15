using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_InputPopup : GameComponent
{
    Action<int> BtnClickedEvent1;
    Action BtnClickedEvent2;

    public UIInput NumInput;
    public UILabel LabelTitle;
    public UILabel LabelMsg;
    public UILabel LabelYes;
    public UILabel LabelNo;

    private int maxNum;

    public void Show(string title, string msg, string text1, string text2, int num, Action<int> callback1, Action callback2)
    {
        LabelTitle.text = title;
        LabelMsg.text = msg;
        LabelYes.text = text1;
        LabelNo.text = text2;
        NumInput.value = num.ToString();

        BtnClickedEvent1 = callback1;
        BtnClickedEvent2 = callback2;
        maxNum = num;

        Show();
    }

    public void OnClick_Yes()
    {
        try
        {
            int num = System.Convert.ToInt32(NumInput.value);
            if (num > maxNum || num < 1) throw new GameException(GameException.ErrorCode.InvalidParam);
            if (BtnClickedEvent1 != null) BtnClickedEvent1(num);
            BtnClickedEvent1 = null;
            BtnClickedEvent2 = null;
            Hide();
        }
        catch(GameException e)
        {
            GameProcess.ShowError(e);
        }
        catch(Exception e)
        {
            GameProcess.ShowError(new GameException(GameException.ErrorCode.Unknown, e.Message));
            BtnClickedEvent1 = null;
            BtnClickedEvent2 = null;
            Hide();
        }
    }

    public void OnClick_No()
    {
        if (BtnClickedEvent2 != null) BtnClickedEvent2();
        BtnClickedEvent1 = null;
        BtnClickedEvent2 = null;
        Hide();
    }
}
