using UnityEngine;
using System.Collections;

public class GameEnum
{
    public enum Menu
    {
        None = 0,
        CHARACTER,
        INVENTORY,
        MAP,
        SHOP,
        RECHARGE,
        EXCHANGE,
        CASHSHOP,
        SETUP,
        SUMMON,
        RANK,
        ACHIEVEMENT,
        EXIT,
        DAILY,
        REGION,
        REVIEW,
        BUG,
        LIKE,
        ECT,
    }

    public enum Grade
    {
        None = 0,
        D,
        C,
        B,
        A,
        S,
        SS,
    }

    public enum CellType
    {
        None,
        StartPoint,
        GoalPoint,
    }

    public enum Direction
    {
        none = 0,
        down = 1,
        left = 4,
        right = 7,
        up = 10,
    }

    #region // old
    public enum PositionType
    {
        None = 0,
        Leader,
        Chongmu,
        Sundo,
        Haksub,
        Bongsa,
        Member,
    }

    public enum CommandType
    {
        ATTACK,
        ALLY,
        APPOINT,
        DISMISS,
        FIND,
        ADD_OPTION,
        TRAIN, 
        SUPPORT,
        COLLECTE,
        BREAK,
        PROTEST,
        BUILD,
        RECRUIT,
        WIN,
        LOSE,
        JOIN_SUCCESS,
        JOIN_FAIL,
        PROTEST_SUCCESS,
        PROTEST_FAIL,
        ALLY_SUCCESS,
        ALLY_FAIL,
        BREAK_SUCCESS,
        MEMBER,
        MISSION,
        SAVE,
    }

    public enum Strategy
    {
        None = 0,
        Attack,
        Neutral,
        Development,
        Dependent,
    }

    public enum ExpType
    {
        NONE,
        POWER,
        WISDOM,
        CHARM,
    }

    public enum MissionSort
    {
        NONE = 0,
        GRADE,
        TYPE,
    }
    #endregion // old
}
