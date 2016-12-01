using System;
using System.Collections;

using Kazoh.Table;

public class GameException : Exception
{
    public enum ErrorCode
    {
        // 치명적 오류, 시스템 종료됨.
        Unknown = 0,
        UnknownServerError = 1,
        NoNetwork = 2,
        ServerShutDown = 100,
        CloseTest = 101,
        InvalidToken = 403,
        CanNotInitTableManager = 1000,
        CanNotLoadTable = 1001,
        FailToGetGameData = 1100,
        FailToUpdateData = 1101,
        FailToGetGameConfig = 1102,
        InvalidConfigVer = 1103,

        // 캐릭터 및 아이템 관련 오류.
        ChaSlotIsFull = 2000,
        InvalidParam = 2001,
        NotEnoughCash = 2002,
        NotEnoughCoin = 2003,
        NotEnoughKey = 2004,
        NotEnoughGold = 2005,
        OverMaxGold = 2006,
        OverMaxCoin = 2007,
        OverMaxKey = 2008,
        OverMaxCash = 2009,
        OverMaxInventory = 2010,
        OverMaxCharacter = 2011,
        CharacterSlotIsEmpty = 2100,
        CanNotSelectCharacter = 2101,
        CanNotDeleteCharacter = 2102,
        CanNotEnchantCharacter = 2103,
        CanNotDGradeCharacter = 2104,
        CanNotHeal = 2105,
        CanNotDeleteDefaultCharacter = 2106,
        ItemSlotIsEmpty = 2200,
        CanNotSellItem = 2201,
        NotEnoughItem = 2202,
        CanNotUseItem = 2203,
        CanNotEquipItem = 2204,
        CanNotEnchantItem = 2205,
        CanNotEnchantEquippedItem = 2206,
        CanNotStackItem = 2207,
        CanNotSellEquippedItem = 2208,
        CanNotUseMembershipService = 2209,
        OnlySilverMembership = 2210,
        OnlyGoldMembership = 2211,
        OnlyDiaMembership = 2212,
        OnlyPlatinumMembership = 2213,
        OverPurchaseLimit = 2214,

        // 맵 관련 오류.
        NoMapPrefabs = 5000,
        HpIsZero = 5001,
        OverGrade = 5002,
        NotEnoughItemSlot = 5003,
        NoStartPoint = 5100,
        NoPlayer = 5101,
        NoTimer = 5102,

        // IAP 관련 오류.
        FailInitialization = 7000,
        NotInitializedPurchaser = 7001,
        NotPurchasingProduct = 7002,
        FailRestorePurchases = 7003,

        // 서버 관련 오류.
        InvalidEmailOrPassward = 8000,
        DuplicatedEmail = 8001,
        CanNotConnectServer = 8002,
        NotExistEmail = 8003,
        ExistEmail = 8004,
        CanNotGetRankData = 8005,

        // 공용 오류.
        NoGameData = 9000,
        NoEmail = 9001,
        NoPw = 9002,
        InvalidEmailFormat = 9003,
        InvalidPwFormat = 9004,
        NoService = 9005,
        UserDataIsNull = 9006,
        EmptyCurPw = 9007,
        EmptyNewPw = 9008,
        EmptyConformPw = 9009,
        EqualsNewPw = 9010,
        NotEqualsNewPw = 9011,
        InvalidPassward = 9012,
        InvalidInicial = 9013,

    }

    public ErrorCode Code;
    public readonly string Msg;
    public bool IsCritical
    {
        get
        {
            if ((int)Code < 2000) return true;
            return false;
        }
    }

    public GameException()
    {
        Code = ErrorCode.Unknown;
        Msg = GetMsg(Code);
    }

    public GameException(ErrorCode _code)
    {
        Code = _code;
        Msg = GetMsg(Code);
    }

    public GameException(ErrorCode _code, string message) : base(message)
    {
        Code = _code;
        Msg = message;
    }

    public GameException(string message)
        : base(message)
    {
    }

    public GameException(string message, Exception inner)
        : base(message, inner)
    {
    }

    string GetMsg(ErrorCode _code)
    {
        return TableManager.GetString("STR_ERROR_" + (int)_code);
    }
}
