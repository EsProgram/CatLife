using System.Collections;
using UnityEngine;

/// <summary>
/// テスト用。使用しない。
/// </summary>
[System.Obsolete("使用しないでください")]
public class CreatureBehaviour : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime);
    }
}