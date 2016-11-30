using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_PopupChaInfo : GameComponent
{
    public UILabel TitleLabel;
    public UILabel NameLabel;
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
    public UILabel AddedPowValueLabel;
    public UILabel AddedDexValueLabel;
    public UILabel AddedIntValueLabel;
    public UILabel AddedConValueLabel;
    public UILabel AddedLucValueLabel;
    public UILabel SkillLabel;
    public UILabel SkillDescLabel;
    public UILabel AtkLabel;
    public UILabel DefLabel;
    public UILabel AspdLabel;
    public UILabel SpdLabel;
    public UILabel HpLabel;
    public UILabel CriLabel;
    public UILabel AtkValueLabel;
    public UILabel DefValueLabel;
    public UILabel AspdValueLabel;
    public UILabel SpdValueLabel;
    public UILabel HpValueLabel;
    public UILabel CriValueLabel;
    public UISprite ChaSprite;
    public UISprite HpSprite;
    public UILabel CloseBtnLabel;

    public UIGrid EquipGrid;
    public GameObject PrefEquipSlot;

    private List<Component_Item_EquipSlot> equipList;

    public override void Init()
    {
        base.Init();

        equipList = new List<Component_Item_EquipSlot>();
        for (int i=0; i<4; ++i)
        {
            GameObject go = NGUITools.AddChild(EquipGrid.gameObject, PrefEquipSlot);
            Component_Item_EquipSlot component = go.GetComponent<Component_Item_EquipSlot>();
            if (component != null)
            {
                component.Init(i);
                equipList.Add(component);
            }
        }
        EquipGrid.Reposition();

        SetConstantUI();
    }

    public void Show(Slot_Character _data)
    {
        if (_data == null || _data.IsEmpty) throw new GameException(GameException.ErrorCode.CharacterSlotIsEmpty);
        SetInfo(_data);
        Show();
    }

    void SetConstantUI()
    {
        TitleLabel.text = TableManager.GetString("STR_TITLE_CHA_INFO");
        GradeLabel.text = TableManager.GetString("STR_UI_GRADE");
        PowLabel.text = TableManager.GetString("STR_UI_POW");
        DexLabel.text = TableManager.GetString("STR_UI_DEX");
        IntLabel.text = TableManager.GetString("STR_UI_INT");
        ConLabel.text = TableManager.GetString("STR_UI_CON");
        LucLabel.text = TableManager.GetString("STR_UI_LUC");
        SkillLabel.text = TableManager.GetString("STR_UI_SKILL");
        AtkLabel.text = TableManager.GetString("STR_UI_ATK");
        AspdLabel.text = TableManager.GetString("STR_OPTION_ASPD");
        DefLabel.text = TableManager.GetString("STR_UI_DEF");
        SpdLabel.text = TableManager.GetString("STR_OPTION_SPD");
        HpLabel.text = TableManager.GetString("STR_OPTION_HP");
        CriLabel.text = TableManager.GetString("STR_OPTION_CRI");
        CloseBtnLabel.text = TableManager.GetString("STR_UI_CLOSE");
    }

    void SetInfo(Slot_Character _data)
    {
        TableData_Character rawData = TableManager.GetGameData(_data.ChaId) as TableData_Character;
        if (rawData == null) throw new GameException(GameException.ErrorCode.InvalidParam);

        NameLabel.text = string.Format("+{0} {1}", _data.Data.EnchantNum, _data.Data.ChaName);
        DescLabel.text = _data.Data.ChaDesc;
        GradeValueLabel.text = GetGradeStr(_data.Data.Grade);

        PowValueLabel.text = string.Format("{0}", rawData.Pow);
        DexValueLabel.text = string.Format("{0}", rawData.Dex);
        IntValueLabel.text = string.Format("{0}", rawData.Int);
        ConValueLabel.text = string.Format("{0}", rawData.Con);
        LucValueLabel.text = string.Format("{0}", rawData.Luc);

        int pow = _data.Data.Pow - rawData.Pow;
        int dex = _data.Data.Dex - rawData.Dex;
        int mp = _data.Data.Int - rawData.Int;
        int con = _data.Data.Con - rawData.Con;
        int luc = _data.Data.Luc - rawData.Luc;

        if(pow < 0) AddedPowValueLabel.text = string.Format("[ff2166]- {0}[-]", -pow); 
        else AddedPowValueLabel.text = string.Format("[21d1ff]+ {0}[-]", pow);
        if (dex < 0) AddedDexValueLabel.text = string.Format("[ff2166]- {0}[-]", -dex); 
        else AddedDexValueLabel.text = string.Format("[21d1ff]+ {0}[-]", dex);
        if (mp < 0) AddedIntValueLabel.text = string.Format("[ff2166]- {0}[-]", -mp); 
        else AddedIntValueLabel.text = string.Format("[21d1ff]+ {0}[-]", mp);
        if (con < 0) AddedConValueLabel.text = string.Format("[ff2166]- {0}[-]", -con); 
        else AddedConValueLabel.text = string.Format("[21d1ff]+ {0}[-]", con);
        if (luc < 0) AddedLucValueLabel.text = string.Format("[ff2166]- {0}[-]", -luc); 
        else AddedLucValueLabel.text = string.Format("[21d1ff]+ {0}[-]", luc);

        if (_data.Data.OptionList.Count > 0)
        {
            for (int i = 0; i < _data.Data.OptionList.Count; ++i)
            {
                TableData_Option option = TableManager.GetGameData(_data.Data.OptionList[i]) as TableData_Option;
                SkillDescLabel.text = string.Format("{0} +{1}", TableManager.GetString("STR_OPTION_" + option.Type.ToString()), option.Value);
            }
        }
        else SkillDescLabel.text = TableManager.GetString("STR_UI_NOSKILL");
        ChaSprite.spriteName = _data.Data.SpriteName + "_02";
        HpSprite.fillAmount = (_data.Data.CurHp + 0f) / _data.Data.MaxHp;

        for (int i = 0; i < equipList.Count; ++i)
        {
            equipList[i].SetData(_data.Data.EquipSlot[i]);
        }

        AtkValueLabel.text = string.Format("{0} / {1}", _data.Data.SAtk, _data.Data.LAtk);
        DefValueLabel.text = string.Format("{0}", _data.Data.Def);
        AspdValueLabel.text = string.Format("{0:0.00}", _data.Data.ASpd * 0.001f);
        SpdValueLabel.text = string.Format("{0}", _data.Data.Spd);
        HpValueLabel.text = string.Format("{0}", _data.Data.MaxHp);
        CriValueLabel.text = string.Format("{0}%", _data.Data.Critical);
    }

    string GetGradeStr(int _grade)
    {
        if (_grade == 1) return "[bbbbbb]D[-]";
        if (_grade == 2) return "[3AFB16]C[-]";
        if (_grade == 3) return "[1692FB]B[-]";
        if (_grade == 4) return "[C816FB]A[-]";
        if (_grade == 5) return "[FB162D]S[-]";
        if (_grade == 6) return "[FBA916]SS[-]";
        return "[bbbbbb]-[-]";
    }

    public void OnClick_Close()
    {
        if (gameObject.activeSelf)
        {
            GameProcess.PlaySound(SOUND_EFFECT.CLICK);
            Hide();
        }
    }
}
