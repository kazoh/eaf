using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_CharacterSelect : GameComponent
{
    public Action<int> ClickedEvent;

    public UILabel TitleLabel;
    public UILabel DescLabel;
    public UILabel GradeLabel;
    public UILabel GradeValueLabel;
    public UILabel PowLabel;
    public UILabel DexLabel;
    public UILabel IntLabel;
    public UILabel ConLabel;
    public UILabel LucLabel;
    public UILabel PowValueLabel;
    public UILabel DexValueLabel;
    public UILabel IntValueLabel;
    public UILabel ConValueLabel;
    public UILabel LucValueLabel;
    public UILabel SkillLabel;
    public UILabel SkillDescLabel;
    public UILabel BtnLabel;

    public UIGrid ChaGrid;
    public GameObject PrefCharacterItem;

    private Component_Item_Character curCha;
    public Component_Item_Character SelectedCharacter
    {
        get { return curCha; }
        private set
        {
            if (curCha != null) curCha.IsSelected = false;
            curCha = value;
            UpdateUI();
        }
    }

    public override void Init()
    {
        base.Init();

        /* 기본 캐릭터 리스트 획득 */
        List<TableData_Character> list = new List<TableData_Character>();
        list.Add(TableManager.GetGameData(10000) as TableData_Character);
        list.Add(TableManager.GetGameData(10001) as TableData_Character);

        /* 캐릭터 리스트 설정 */
        foreach (TableData_Character item in list)
        {
            if (item == null) continue;
            GameObject go = NGUITools.AddChild(ChaGrid.gameObject, PrefCharacterItem);
            Component_Item_Character component = go.GetComponent<Component_Item_Character>();
            if (component != null)
            {
                component.Init(item);
                component.ClickedEvent += OnSelect;
                if (SelectedCharacter == null) component.IsSelected = true;
                else component.IsSelected = false;
            }
        }
        ChaGrid.Reposition();

        SetConstantUI();
    }

    void SetConstantUI()
    {
        TitleLabel.text = TableManager.GetString("STR_TITLE_CHA_SELECT");
        GradeLabel.text = TableManager.GetString("STR_UI_GRADE");
        PowLabel.text = TableManager.GetString("STR_UI_POW");
        DexLabel.text = TableManager.GetString("STR_UI_DEX");
        IntLabel.text = TableManager.GetString("STR_UI_INT");
        ConLabel.text = TableManager.GetString("STR_UI_CON");
        LucLabel.text = TableManager.GetString("STR_UI_LUC");
        BtnLabel.text = TableManager.GetString("STR_UI_SELECT");
    }

    void UpdateUI()
    {
        DescLabel.text = TableManager.GetString(SelectedCharacter.Data.Desc);
        GradeValueLabel.text = ((GameEnum.Grade)SelectedCharacter.Data.Grade).ToString();
        PowValueLabel.text = "" + SelectedCharacter.Data.Pow;
        DexValueLabel.text = "" + SelectedCharacter.Data.Dex;
        IntValueLabel.text = "" + SelectedCharacter.Data.Int;
        ConValueLabel.text = "" + SelectedCharacter.Data.Con;
        LucValueLabel.text = "" + SelectedCharacter.Data.Luc;
        SkillLabel.text = TableManager.GetString("STR_UI_SKILL");
        SkillDescLabel.text = TableManager.GetString("STR_UI_NOSKILL");
    }

    void OnSelect(Component_Item_Character cha)
    {
        SelectedCharacter = cha;
    }

    public void OnClick_Select()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (SelectedCharacter != null && ClickedEvent != null) ClickedEvent(SelectedCharacter.Data.Id);
    }
}
