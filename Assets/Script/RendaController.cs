using System.Collections;
using UnityEngine;

public class RendaController
{
    private static RendaController _singleton;
    private GUITexture renda = default(GUITexture);
    private int rendaCount;//連打テクスチャが有効になってからの連打数

    private RendaController()
    {
    }

    private RendaController(GUITexture renda)
    {
        this.renda = renda;
    }

    public static RendaController GetInstance(GUITexture renda)
    {
        if(_singleton == null)
            _singleton = new RendaController(renda);
        return _singleton;
    }

    /// <summary>
    /// 連打テクスチャが有効かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsEnabled()
    {
        return renda == null ? false : renda.enabled;
    }

    /// <summary>
    /// 有効/無効を設定する
    /// 連打カウントはリセットされる
    /// </summary>
    /// <param name="d"></param>
    public void SetEnabled(bool d)
    {
        rendaCount = 0;
        renda.enabled = d;
    }

    /// <summary>
    /// 連打カウントをインクリメント
    /// </summary>
    public void Increment()
    {
        ++rendaCount;
    }

    /// <summary>
    /// カウント数を返す
    /// </summary>
    /// <returns></returns>
    public int GetCount()
    {
        return rendaCount;
    }
}