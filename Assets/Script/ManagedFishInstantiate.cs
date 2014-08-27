using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 魚の生成を管理する
/// </summary>
public class ManagedFishInstantiate : MonoBehaviour
{
    public Fish[] fishesPrefab;

    [HideInInspector]
    public List<Fish> fishes = new List<Fish>();//現在ゲームシーンに存在する魚。AimTrigger付近にいるかを確かめる用

    private ManagedWaterPlace managedWaterPlaces;

    /// <summary>
    /// デフォルトでアタッチされるオブジェクト以外のところで
    /// このクラスが生成されないようにする
    /// </summary>
    private ManagedFishInstantiate()
    {
    }

    /// <summary>
    /// 水場に魚を1匹生成する
    /// 生成した魚はリストに格納される
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
        Fish f = new Fish();
        f.fishPrefab = Instantiate(fish,
                             Vector3.up + waterPlace.transform.position + waterRange,
                             Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))) as GameObject;
        fishes.Add(f);
    }

    private void Awake()
    {
        managedWaterPlaces = FindObjectOfType<ManagedWaterPlace>();
    }

    private void Start()
    {
        //テスト生成
        CreateFish(fishesPrefab[0].gameObject, managedWaterPlaces.FindWithName("Mizuba1").gameObject);
    }

    private void Update()
    {
    }
}