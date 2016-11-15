using UnityEngine;
using System.Collections;

public class Component_Map_Object : GameComponent
{
    protected Component_Map map;

    public string Name { get; protected set; }
    public Vector3 Pos { get { return transform.localPosition; } }

    public void Init(Component_Map _map)
    {
        map = _map;
        Init();
    }
}
