using UnityEngine;
using System.Collections;
using System;

public class Component_Map_Cell : GameComponent {

    public Action<Component_Map_Cell> SelectEvent;

    public UISprite SelectSprite;

    public GameEnum.CellType Type;
    public bool IsBlock;
    public bool IsWall;

    private bool isSelect;
    public bool IsSelect
    {
        get { return isSelect; }
        set
        {
            isSelect = value;
            if (isSelect) SelectSprite.alpha = 1f;
            else SelectSprite.alpha = 0f;
        }
    }

    public Vector3 Pos { get { return transform.localPosition; } }
    public int Idx { get; private set; }
    public int ColNum { get; private set; }
    public int Row { get { return Idx / ColNum; } }
    public int Col { get { return Idx % ColNum; } }

    public void Init(int idx, int colNum)
    {
        Init();
        Idx = idx;
        ColNum = colNum;
        IsSelect = false;
    }

    void OnClick()
    {
        if (IsSelect) return;
        StartCoroutine(SelectCell());
        if (SelectEvent != null) SelectEvent(this);
    }

    IEnumerator SelectCell()
    {
        IsSelect = true;
        yield return new WaitForSeconds(0.5f);
        IsSelect = false;
    }
}
