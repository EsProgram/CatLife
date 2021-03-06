﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 魚の生成を管理する
/// </summary>
public class ManagedFishInstantiate : MonoBehaviour
{
    //魚を生成する水場
    [SerializeField]
    private WaterPlace waterPlace;
    public int tryInstanceNum = 5;
    public Fish[] fishesPrefab;

    [HideInInspector]
    public List<Fish> fishes = new List<Fish>();//現在ゲームシーンに存在する魚。AimTrigger付近にいるかを確かめる用

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
    public void CreateFish(Fish fish, WaterPlace waterPlace)
    {
        //不正検知
        if(fish == null || waterPlace == null || fish.gameObject == null || waterPlace.gameObject == null)
        {
            Debug.LogError("CreateFishの引数に渡されたオブジェクトがNullでした");
            return;
        }

        if(fish.gameObject.tag != "Fish")
        {
            Debug.LogError("Fishにタグが不適切なオブジェクトが渡されました\nタグに\"Fish\"を設定してください\nオブジェクト名 : " + fish.name);
            return;
        }

        if(waterPlace.gameObject.tag != "WaterPlace")
        {
            Debug.LogError("WaterPlaceにタグが不適切なオブジェクトが渡されました\nタグに\"WaterPlace\"を設定してください\nオブジェクト名 : " + waterPlace.name);
            return;
        }

        //生成場所の計算
        Vector3 waterRange = waterPlace.gameObject.transform.lossyScale;
        waterRange.y = 0;
        waterRange.x = Random.Range(-waterRange.x, waterRange.x) / 5;
        waterRange.z = Random.Range(-waterRange.z, waterRange.z) / 5;
        //実体化
        Fish f = new Fish();
        f.fishPrefab = Instantiate(fish.gameObject,
                             Vector3.up + waterPlace.gameObject.transform.position + waterRange,
                             Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))) as GameObject;
        //参照フィールドへの追加
        fishes.Add(f);
    }

    private void Start()
    {
        //魚生成
        for(int i = 0; i < tryInstanceNum; ++i)
            for(int j = 0; j < fishesPrefab.Length; ++j)
                if(Random.Range(1, 100) < fishesPrefab[j].Appearance)
                    CreateFish(fishesPrefab[j], waterPlace);
    }
}