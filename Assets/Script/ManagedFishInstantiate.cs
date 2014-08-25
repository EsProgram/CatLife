using System.Collections;
using UnityEngine;

/// <summary>
/// 魚の生成を管理する
/// </summary>
public class ManagedFishInstantiate : MonoBehaviour
{
    public GameObject[] fishesPrefab;
    private ManagedWaterPlace managedWaterPlaces;

    /// <summary>
    /// デフォルトでアタッチされるオブジェクト以外のところで
    /// このクラスが生成されないようにする
    /// </summary>
    private ManagedFishInstantiate()
    {
    }

    private void Awake()
    {
        managedWaterPlaces = FindObjectOfType<ManagedWaterPlace>();
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    /// <summary>
    /// 水場に魚を1匹配置する
    /// </summary>
    /// <param name="fish">魚</param>
    /// <param name="waterPlace">水場</param>
    public void CreateFish(GameObject fish, GameObject waterPlace)
    {
        if(fish.tag != "Fish")
        {
            Debug.LogError("Fishにタグが不適切なオブジェクトが渡されました\nタグに\"Fish\"を設定してください\nオブジェクト名 : " + fish.name);
            return;
        }

        if(waterPlace.tag != "WaterPlace")
        {
            Debug.LogError("WaterPlaceにタグが不適切なオブジェクトが渡されました\nタグに\"WaterPlace\"を設定してください\nオブジェクト名 : " + waterPlace.name);
            return;
        }

        Vector3 waterRange = waterPlace.transform.lossyScale;
        waterRange.y = 0;
        waterRange.x = Random.Range(-waterRange.x, waterRange.x) / 3;
        waterRange.z = Random.Range(-waterRange.z, waterRange.z) / 3;
        Instantiate(fish,
                    Vector3.up + waterPlace.transform.position + waterRange,
                    Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
    }
}