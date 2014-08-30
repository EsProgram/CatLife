﻿using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// アタッチしたBoxCollider(IsTrriger)オブジェクトにプレイヤータグをもつオブジェクトが
/// 侵入した際にカメラを指定したものに切り替える
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class ChangeCameraTrigger : MonoBehaviour
{
    [SerializeField]
    //変更するカメラへの参照
    private Camera targetCamera;

    //現在有効なカメラ
    private static Camera currentCamera;

    private void Start()
    {
        //アタッチされたコライダーのisTriggerをtrueにする
        collider.isTrigger = true;

        //ターゲットカメラを設定していなかったら破棄する
        if(targetCamera == null)
            Destroy(this.gameObject);

        //メインカメラ以外のカメラがターゲットになっていた場合、初期では無効にしておく
        if(targetCamera != Camera.main)
            targetCamera.enabled = false;

        //最初はメインカメラをcurrentCameraに設定する
        currentCamera = Camera.main;

        //メインカメラが存在しない場合は念のためどれかしらを設定するが、エラーログを出力する
        if(currentCamera == null)
        {
            Debug.LogError("メインカメラが存在しませんでした\nシーン開始時に最初に有効にするカメラはメインカメラに設定してください");
            currentCamera = targetCamera;
            currentCamera.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            currentCamera.enabled = false;
            targetCamera.enabled = true;
            currentCamera = targetCamera;
        }
    }
}