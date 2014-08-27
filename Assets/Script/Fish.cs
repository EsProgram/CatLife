using System.Collections;
using UnityEngine;

[System.Serializable]
public class Fish
{
    public GameObject fishPrefab;

    public string name { get { return fishPrefab.name; } }
    public GameObject gameObject { get { return fishPrefab; } }
}