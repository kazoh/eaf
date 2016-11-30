using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_InputName : GameComponent
{
    Action<string> BtnClickedEvent1;
    Action BtnClickedEvent2;

    public UIInput Input;
    public UILabel LabelTitle;
    public UILabel LabelMsg;
    public UILabel LabelYes;
    public UILabel LabelNo;

    private int maxNum;

    public void Show(string title, string msg, string text1, string text2, string curName, Action<string> callback1, Action callback2)
    {
        LabelTitle.text = title;
        LabelMsg.text = msg;
        LabelYes.text = text1;
        LabelNo.text = text2;
        Input.value = curName;

        BtnClickedEvent1 = callback1;
        BtnClickedEvent2 = callback2;

        Show();
    }

    public void OnClick_Yes()
    {
        try
        {
            if (Input.value.Length != 3) throw new GameException(GameException.ErrorCode.InvalidInicial);
            if (BtnClickedEvent1 != null) BtnClickedEvent1(Input.value);
            BtnClickedEvent1 = null;
            BtnClickedEvent2 = null;
            Hide();
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

    public void OnClick_No()
    {
        if (BtnClickedEvent2 != null) BtnClickedEvent2();
        BtnClickedEvent1 = null;
        BtnClickedEvent2 = null;
        Hide();
    }
}
