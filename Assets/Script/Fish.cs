using System.Collections;
using UnityEngine;

[System.Serializable]
public class Fish
{
    public GameObject fishPrefab;

    public string name { get { return fishPrefab.name; } }
    public GameObject gameObject { get { return fishPrefab; } }

    //出現確率
    [SerializeField]
    private int appearance;

    //魚の出現確率
    public int Appearance { get { return appearance; } }
}