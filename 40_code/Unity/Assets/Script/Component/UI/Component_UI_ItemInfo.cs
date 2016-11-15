using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Kazoh.Table;

public class Component_UI_ItemInfo : GameComponent
{
    public Action<Slot_Item> SellItemEvent;
    public Action<Slot_Item> EquipItemEvent;
    public Action<Slot_Item> EnchantItemEvent;
    public Action<Slot_Item> UseItemEvent;

    public UILabel NameLabel;
    public UISprite IconSprite;
    public UISprite GradeSprite;
    public UILabel EquipLabel;
    public UILabel NumLabel;
    public UISprite MoneySprite;
    public UILabel PriceLabel;
    public UILabel DescLabel;
    public UILabel AtkLabel;
    public UILabel AspLabel;
    public UILabel DefLabel;
    public UILabel SpdLabel;
    public UILabel HpLabel;
    public UILabel AtkValueLabel;
    public UILabel AspValueLabel;
    public UILabel DefValueLabel;
    public UILabel SpdValueLabel;
    public UILabel HpValueLabel;
    public UILabel SkillLabel;
    public Transform BtnGrid;
    public int CellWidth;
    public UIButton UseBtn;
    public UIButton EquipBtn;
    public UIButton EnchantBtn;
    public UIButton SellBtn;
    public UILabel BtnUseLabel;
    public UILabel BtnEquipLabel;
    public UILabel BtnEnchantLabel;
    public UILabel BtnSaleLabel;

    private List<Transform> BtnList;

    public Slot_Item Data { get; private set; }

    public override void Init()
    {
        base.Init();
        SetConstantUI();
        UpdateUI();
    }

    public void UpdateUI(Slot_Item _data = null)
    {
        Data = _data;
        SetIconInfo(_data);
        SetItemInfo(_data);
        SetButton(_data);
    }

    void SetConstantUI()
    {
        MoneySprite.spriteName = "coin_01";
        AtkLabel.text = TableManager.GetString("STR_UI_ATK");
        AspLabel.text = TableManager.GetString("STR_UI_ASPD");
        DefLabel.text = TableManager.GetString("STR_UI_DEF");
        SpdLabel.text = TableManager.GetString("STR_UI_SPD");
        HpLabel.text = TableManager.GetString("STR_UI_HP");
        BtnUseLabel.text = TableManager.GetString("STR_UI_USE");
        BtnEnchantLabel.text = TableManager.GetString("STR_UI_ENCHANT");
        BtnSaleLabel.text = TableManager.GetString("STR_UI_SELL");

        BtnList = new List<Transform>();
        for(int i=0; i < BtnGrid.childCount; ++i)
        {
            BtnList.Add(BtnGrid.GetChild(i));
        }
    }

    void SetIconInfo(Slot_Item _data)
    {
        if (Data == null || Data.IsEmpty)
        {
            IconSprite.spriteName = "icon_item_0000";
            GradeSprite.spriteName = "";
            EquipLabel.text = "";
            NumLabel.text = "";
        }
        else
        {
            Data_UserItem item = Data.Data;
            IconSprite.spriteName = item.IconName;
            GradeSprite.spriteName = "grade_0" + item.Data.Grade;
            EquipLabel.text = Data.IsEquipped ? "E" : "";
            NumLabel.text = item.CanStack ? "" + item.Num : "";
        }
    }

    void SetItemInfo(Slot_Item _data)
    {
        TableData_Item item = _data == null || _data.Data == null ? null : TableManager.GetGameData(_data.ItemId) as TableData_Item;
        if (item != null)
        {
            NameLabel.text = string.Format("+{0} {1}",_data.Data.EnchantNum, TableManager.GetString(item.Str));
            PriceLabel.text = string.Format("{0:###,###,##0}", item.SalePrice);
            DescLabel.text = TableManager.GetString(item.Desc);

            AtkValueLabel.text = string.Format("{0}/{1}", _data.Data.SAtk, _data.Data.LAtk);

            if (_data.Data.Data.SlotIdx == 1) AspValueLabel.text = string.Format("{0:0.00}", _data.Data.ASpd * 0.001f);
            else AspValueLabel.text = string.Format("{0}", _data.Data.ASpd);

            DefValueLabel.text = string.Format("{0}", _data.Data.Def);
            SpdValueLabel.text = string.Format("{0}", _data.Data.Spd);
            HpValueLabel.text = string.Format("{0}", _data.Data.Hp);

            if (item.Type == TableEnum.ItemType.Equip)
            {
                if (_data.Data.OptionList.Count > 0)
                {
                    for (int i = 0; i < _data.Data.OptionList.Count; ++i)
                    {
                        TableData_Option option = TableManager.GetGameData(_data.Data.OptionList[i]) as TableData_Option;
                        SkillLabel.text = string.Format("[916F24]{0}[-] [0C79D3]{1} +{2}[-]", TableManager.GetString("STR_UI_SKILL"), TableManager.GetString("STR_OPTION_" + option.Type.ToString()), option.Value);
                    }
                }
                else SkillLabel.text = "";
            }
            else SkillLabel.text = "";
        }
        else
        {
            NameLabel.text = TableManager.GetString("STR_UI_EMPTY_SLOT");
            PriceLabel.text = string.Format("{0:###,###,##0}", 0);
            DescLabel.text = "";
            AtkValueLabel.text = string.Format("{0}/{1}", 0, 0);
            AspValueLabel.text = string.Format("{0}", 0);
            DefValueLabel.text = string.Format("{0}", 0);
            SpdValueLabel.text = string.Format("{0}", 0);
            HpValueLabel.text = string.Format("{0}", 0);
            SkillLabel.text = "";
        }
    }

    void SetButton(Slot_Item _data)
    {
        TableData_Item item = _data == null || _data.Data == null ? null : TableManager.GetGameData(_data.ItemId) as TableData_Item;
        if (item != null)
        {
            if(item.IsUse) UseBtn.gameObject.SetActive(true);
            else UseBtn.gameObject.SetActive(false);
            if (item.IsEquip)
            {
                if(_data.IsEquipped) BtnEquipLabel.text = TableManager.GetString("STR_UI_DISMOUNT");
                else BtnEquipLabel.text = TableManager.GetString("STR_UI_MOUNT");
                EquipBtn.gameObject.SetActive(true);
            }
            else EquipBtn.gameObject.SetActive(false);
            if (item.IsEnchant && _data.CanEnchant) EnchantBtn.gameObject.SetActive(true);
            else EnchantBtn.gameObject.SetActive(false);
            if (item.IsSell) SellBtn.gameObject.SetActive(true);
            else SellBtn.gameObject.SetActive(false);
        }
        else
        {
            UseBtn.gameObject.SetActive(false);
            EquipBtn.gameObject.SetActive(false);
            EnchantBtn.gameObject.SetActive(false);
            SellBtn.gameObject.SetActive(false);
        }

        List<Transform> list = new List<Transform>();
        for(int i=0; i < BtnList.Count; ++i)
        {
            if (BtnList[i].gameObject.activeSelf) list.Add(BtnList[i]);
        }
        if(list.Count > 0)
        {
            float pos_x = 0f - (list.Count - 1) * (CellWidth * 0.5f);
            for(int i=0; i < list.Count; ++i)
            {
                Vector3 pos = list[i].localPosition;
                pos.x = pos_x;
                list[i].localPosition = pos;
                pos_x += CellWidth;
            }

        }
    }

    public void OnClick_Sell()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (SellItemEvent != null) SellItemEvent(Data);
    }

    public void OnClick_Use()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (UseItemEvent != null) UseItemEvent(Data);
    }

    public void OnClick_Enchant()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (EnchantItemEvent != null) EnchantItemEvent(Data);
    }

    public void OnClick_Equip()
    {
        GameProcess.PlaySound(SOUND_EFFECT.CLICK);
        if (EquipItemEvent != null) EquipItemEvent(Data);
    }
}
