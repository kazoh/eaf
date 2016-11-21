using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Component_Mob_Roaming : Component_Map_Monster
{
    public List<Component_Map_Cell> PassList;
    public bool IsRoop;

    private int passIdx;

    protected override void Wait()
    {
        curCell = GetNextCell(GameEnum.Direction.none);
        curDir = GetDirection(curCell.Pos);

#if DEBUG_MODE
        Debug.Log("Path " + curCell.Idx + "/ Dir "+curDir.ToString());
#endif
    }

    protected override Component_Map_Cell GetNextCell(GameEnum.Direction dir)
    {
        Component_Map_Cell cell = null;
        if (PassList == null || PassList.Count == 0) return cell;
        if (passIdx == PassList.Count && IsRoop)
        {
            passIdx = 0;
        }
        if (passIdx < PassList.Count)
        {
            cell = PassList[passIdx];
            passIdx++;
        }

        return cell;
    }
}
