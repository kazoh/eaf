using UnityEngine;
using System.Collections;
using System;

public class Slot_Item
{
    public Action ChangedEvent;

    public int SlotId { get; private set; }
    public Data_UserItem Data { get; private set; }
    public int ItemId { get { return Data == null ? 0 : Data.DataId; } }
    public int EquipSlotId { get { return CanEquip ? Data.Data.SlotIdx : -1; } }
    public bool IsEmpty { get { return Data == null; } }
    public bool IsEquipped { get { return Data == null ? false : Data.IsEquipped; } }
    public bool CanStack { get { return Data == null ? false : Data.CanStack; } }
    public bool CanEnchant { get { return Data == null ? false : Data.CanEnchant; } }
    public bool CanSell { get { return Data == null ? false : Data.CanSell; } }
    public bool CanEquip { get { return Data == null ? false : Data.CanEquip; } }
    public bool CanUse { get { return Data == null ? false : Data.CanUse; } }
    public int RestCount { get { return Data == null ? 99 : 99 - Data.Num; } }
    public bool IsNew { get; private set; }

    public Slot_Item(int _slotId)
    {
        SlotId = _slotId;
    }

    public void Put(int num)
    {
        Data.Add(num);
        OnChanged();
    }

    public void Put(TableData_Item item, int num)
    {
        Data = new Data_UserItem(item, num);
        Data.ChangedEvent += OnChanged;
        IsNew = true;
        OnChanged();
    }

    public void Put(IDictionary dict)
    {
        Data = new Data_UserItem(dict);
        Data.ChangedEvent += OnChanged;
        IsNew = false;
        OnChanged();
    }

    public void UseItem(int num = 1)
    {
        if (num > Data.Num) throw new GameException(GameException.ErrorCode.CanNotUseItem);
        DeleteItem(num);
    }

    public void SellItem(int num)
    {
        if (num > Data.Num) throw new GameException(GameException.ErrorCode.NotEnoughItem);
        DeleteItem(num);
    }

    void DeleteItem(int num)
    {
        Data.Add(-num);
        if (Data.Num == 0) Data = null;
        OnChanged();
    }

    public void Equip(bool isEquip)
    {
        if (CanEquip)
        {
            Data.Equip(isEquip);
        }
        else throw new GameException(GameException.ErrorCode.CanNotEquipItem);
    }

    public void Enchant()
    {
        if (Data.CanEnchant)
        {
            if(Data.EnchantNum > 3)
            {
                int dice = UnityEngine.Random.Range(0, 100) % 2;
                if (dice == 0) Data.Enchant(1);
                else Data = null;
            }
            else
            {
                Data.Enchant(1);
            }
            OnChanged();
        }
        else throw new GameException(GameException.ErrorCode.CanNotEnchantItem);
    }

    public void SetNew()
    {
        IsNew = false;
        OnChanged();
    }

    void OnChanged()
    {
        if (ChangedEvent != null) ChangedEvent();
    }
}
