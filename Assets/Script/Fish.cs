using System.Collections;
using UnityEngine;

[System.Serializable]
public class Fish
{
    public GameObject fish;

    public string name { get { return fish.name; } }

    public GameObject gameObject { get { return fish; } }
}