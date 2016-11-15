using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_Character : GameComponent
{
    public Action<Slot_Character> AddCharacterEvent;
    public Action<Slot_Character> SelectCharacterEvent;
    public Action<Slot_Character> DownGradeCharacterEvent;
    public Action<Slot_Character> EnchantCharacterEvent;
    public Action<Slot_Character> DeleteCharacterEvent;
    public Action<Slot_Item> DismountEvent;
    public Action AddSlotEvent;

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
    public UILabel SkillLabel;
    public UILabel SkillDescLabel;
    public UISprite ChaSprite;
    public UISprite HpSprite;
    public UITexture ChaBg;
    public UILabel SlotNumLabel;
    public UILabel BtnSelectLabel;
    public UILabel BtnDGradeLabel;
    public UILabel BtnEnchantLabel;
    public UILabel BtnDelLabel;
    public UIButton BtnLeft;
    public UIButton BtnRight;
    public UIButton BtnPlus;

    public UIGrid EquipGrid;
    public GameObject PrefEquipSlot;

    private int idx;
    private List<Slot_Character> chaList;
    private List<Component_Item_EquipSlot> equipList;

    public Slot_Character Data { get { return chaList != null && chaList.Count > idx ? chaList[idx] : null; } }

    public override void Init()
    {
        base.Init();
        idx = GameProcess.GetGameDataManager().CurCharacterSlotId;
        SetConstantUI();

        equipList = new List<Component_Item_EquipSlot>();
        for (int i=0; i<4; ++i)
        {
            GameObject go = NGUITools.AddChild(EquipGrid.gameObject, PrefEquipSlot);
            Component_Item_EquipSlot component = go.GetComponent<Component_Item_EquipSlot>();
            if (component != null)
            {
                component.Init(i);
                component.ClickedEvent += OnSelectEquip;
                equipList.Add(component);
            }
        }
        EquipGrid.Reposition();

        chaList = new List<Slot_Character>();
        GameProcess.GetGameDataManager().ChangedChaSlotEvent += OnChangeChaList;
        OnChangeChaList();

        OnChange();
    }

    public void UpdateUI(Slot_Character _data = null)
    {
        SetInfo(Data);
        SetButton();
    }

    void SetConstantUI()
    {
        GradeLabel.text = TableManager.GetString("STR_UI_GRADE");
        PowLabel.text = TableManager.GetString("STR_UI_POW");
        DexLabel.text = TableManager.GetString("STR_UI_DEX");
        IntLabel.text = TableManager.GetString("STR_UI_INT");
        ConLabel.text = TableManager.GetString("STR_UI_CON");
        LucLabel.text = TableManager.GetString("STR_UI_LUC");
        SkillLabel.text = TableManager.GetString("STR_UI_SKILL");
        BtnSelectLabel.text = TableManager.GetString("STR_UI_SELECT");
        BtnDGradeLabel.text = TableManager.GetString("STR_UI_DOWN");
        BtnEnchantLabel.text = TableManager.GetString("STR_UI_CHA_ENCHANT");
        BtnDelLabel.text = TableManager.GetString("STR_UI_DELETE");
    }

    void SetInfo(Slot_Character _data)
    {
        if(_data.IsEmpty || _data == null)
        {
            NameLabel.text = TableManager.GetString("STR_UI_EMPTY_SLOT");
            DescLabel.text = "---";
            GradeValueLabel.text = "-";
            PowValueLabel.text = string.Format("{0}", 0);
            DexValueLabel.text = string.Format("{0}", 0);
            IntValueLabel.text = string.Format("{0}", 0);
            ConValueLabel.text = string.Format("{0}", 0);
            LucValueLabel.text = string.Format("{0}", 0);
            SkillDescLabel.text = TableManager.GetString("STR_UI_NOSKILL");
            ChaSprite.spriteName = "cha_empty";
            ChaBg.alpha = 0f;
            HpSprite.fillAmount = 0f;

            for (int i = 0; i < equipList.Count; ++i)
            {
                equipList[i].SetData(null);
            }
        }
        else
        {
            NameLabel.text = string.Format("+{0} {1}", _data.Data.EnchantNum, _data.Data.ChaName);
            DescLabel.text = _data.Data.ChaDesc;
            GradeValueLabel.text = GetGradeStr(_data.Data.Grade);
            PowValueLabel.text = string.Format("{0}", _data.Data.Pow);
            DexValueLabel.text = string.Format("{0}", _data.Data.Dex);
            IntValueLabel.text = string.Format("{0}", _data.Data.Int);
            ConValueLabel.text = string.Format("{0}", _data.Data.Con);
            LucValueLabel.text = string.Format("{0}", _data.Data.Luc);

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
            HpSprite.fillAmount = (Data.Data.CurHp + 0f) / Data.Data.MaxHp;
            ChaBg.alpha = 1f;

            for (int i = 0; i < equipList.Count; ++i)
            {
                equipList[i].SetData(_data.Data.EquipSlot[i]);
            }
        }

        SlotNumLabel.text = string.Format("{0}/{1}", idx + 1, chaList.Count);
    }

    void SetButton()
    {
        if(chaList != null && chaList.Count > 1)
        {
            if(idx == 0) BtnLeft.gameObject.SetActive(false);
            else BtnLeft.gameObject.SetActive(true);
            if (idx + 1 == chaList.Count)
            {
                BtnRight.gameObject.SetActive(false);
                BtnPlus.gameObject.SetActive(true);
            }
            else
            {
                BtnRight.gameObject.SetActive(true);
                BtnPlus.gameObject.SetActive(false);
            }
        }
        else
        {
            BtnLeft.gameObject.SetActive(false);
            BtnRight.gameObject.SetActive(false);
            BtnPlus.gameObject.SetActive(false);
        }
    }

    void OnChange()
    {
        UpdateUI(Data);
    }

    void OnSelectEquip(Component_Item_EquipSlot _equip)
    {
        if (DismountEvent != null) DismountEvent(_equip.Data);
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

    public void OnClick_Next()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (idx + 1 < chaList.Count)
        {
            idx++;
            UpdateUI(Data);
        }
        else AddSlot();
    }

    public void OnClick_Prev()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (idx > 0)
        {
            idx--;
            UpdateUI(Data);
        }
    }

    public void OnClick_Select()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (AddCharacterEvent != null) SelectCharacterEvent(Data);
    }

    public void OnClick_DGrade()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (AddCharacterEvent != null) DownGradeCharacterEvent(Data);
    }

    public void OnClick_Enchant()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (AddCharacterEvent != null) EnchantCharacterEvent(Data);
    }

    public void OnClick_Delete()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (AddCharacterEvent != null) DeleteCharacterEvent(Data);
    }

    public void OnClick_Add()
    {
        if (Data.IsEmpty)
        {
            GameProcess.PlaySound(SOUND_EFFECT.CLICK);
            if (AddCharacterEvent != null) AddCharacterEvent(Data);
        }
    }

    void AddSlot()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (AddSlotEvent != null) AddSlotEvent();
    }

    void OnChangeChaList(Data_User data = null)
    {
        List<Slot_Character> list = GameProcess.GetGameDataManager().GetCharacterSlot();
        for(int i=0; i < list.Count; ++i)
        {
            if (chaList.Contains(list[i])) continue;
            list[i].ChangedEvent += OnChange;
            chaList.Add(list[i]);
        }

        OnChange();
    }
}
