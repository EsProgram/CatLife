using System.Collections;
using UnityEngine;

/// <summary>
/// 水場を表す
/// </summary>
[System.Serializable]
public class WaterPlace
{
    public GameObject place;

    public string name { get { return place.name; } }

    public GameObject gameObject { get { return place; } }
}