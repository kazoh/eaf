using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kazoh.Table;

public class Component_Map_Test : Component_Map
{

    public override void GameStart()
    {
        StartCoroutine(TestGame());
    }

    IEnumerator TestGame()
    {
        yield return new WaitForSeconds(1);
        OnClear();
    }

    void OnClear()
    {
        int dice = UnityEngine.Random.Range(0, 100);
        if (dice % 2 == 0) OnGameClear();
        else OnGameOver();
    }
}
