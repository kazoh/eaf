using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Slot_Character
{
    public Action ChangedEvent;

    public Data_UserCharacter Data { get; private set; }
    public int ChaId { get { return IsEmpty ? 0 : Data.DataId; } }
    public bool IsEmpty { get { return Data == null; } }
    public bool IsDefaultCha
    {
        get
        {
            if (IsEmpty) return false;
            if (Data.DataId != 10000 && Data.DataId != 10001) return false;
            return true;
        }
    }
    public bool CanDelete
    {
        get
        {
            if (IsEmpty) return false;
            if (this == GameProcess.GetGameDataManager().GetCurCharacter()) return false;
            if (GameProcess.GetGameDataManager().GetCharacterNum() < 2) return false;
            return true;
        }
    }
    public bool CanEnchant { get { return IsEmpty ? false : Data.EnchantNum < 5; } }
    public bool CanDownGrade { get { return IsEmpty ? false : Data.Grade > 1; } }
    public bool IsNew { get; private set; }

    public Slot_Character()
    {
        IsNew = false;
    }

    public void Put(TableData_Character data)
    {
        Data = new Data_UserCharacter(data);
        IsNew = true;
        OnChanged();
    }

    public void Put(IDictionary dict)
    {
        Data = new Data_UserCharacter(dict);
        IsNew = false;
    }

    public void SetNew()
    {
        IsNew = false;
    }

    public string Enchant()
    {
        if (Data.CanEnchant)
        {
            string str = Data.Enchant();
            OnChanged();
            return str;
        }
        else throw new GameException(GameException.ErrorCode.CanNotEnchantCharacter);
    }

    public string DownGrade()
    {
        if (Data.CanDownGrade)
        {
            string str = Data.DownGrade();
            OnChanged();
            return str;
        }
        else throw new GameException(GameException.ErrorCode.CanNotDGradeCharacter);
    }

    public void Discard()
    {
        if (CanDelete)
        {
            foreach(KeyValuePair<int,Slot_Item> es in Data.EquipSlot)
            {
                if (es.Value != null) es.Value.Equip(false);
            }
            Data = null;
            OnChanged();
        }
        else throw new GameException(GameException.ErrorCode.CanNotDeleteCharacter);
    }

    public void AddHp(int value)
    {
        Data.CurHp += value;
        OnChanged();
    }

    public void Equip(Slot_Item _item)
    {
        if (Data.EquipSlot.ContainsKey(_item.EquipSlotId))
        {
            if(Data.EquipSlot[_item.EquipSlotId] != null) Data.EquipSlot[_item.EquipSlotId].Equip(false);
            Data.EquipSlot[_item.EquipSlotId] = _item;
            _item.Equip(true);
            OnChanged();
        }
        else throw new GameException(GameException.ErrorCode.InvalidParam);
    }

    public void Dismount(int _slotIdx)
    {
        if (Data.EquipSlot.ContainsKey(_slotIdx))
        {
            Data.EquipSlot[_slotIdx] = null;
            OnChanged();
        }
        else throw new GameException(GameException.ErrorCode.InvalidParam);
    }

    void OnChanged()
    {
        if (ChangedEvent != null) ChangedEvent();
    }
}
