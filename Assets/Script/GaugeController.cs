using System.Collections;
using UnityEngine;

public class GaugeController
{
    private static GaugeController _singleton;
    private GUITexture gauge;
    private GUITexture frame;
    private float gaugeWidth;

    private const float MAX = 255f;//ゲージの最大値

    public static GaugeController GetInstance(GUITexture gauge, GUITexture frame)
    {
        if(_singleton == null)
            _singleton = new GaugeController(gauge, frame);
        return _singleton;
    }

    /// <summary>
    /// プライベートコンストラクタ
    /// </summary>
    private GaugeController()
    {
    }

    /// <summary>
    /// プライベートコンストラクタ
    /// </summary>
    /// <param name="gauge">ゲージ</param>
    /// <param name="frame">ゲージの枠</param>
    private GaugeController(GUITexture gauge, GUITexture frame)
    {
        this.gauge = gauge;
        this.frame = frame;
    }

    private void Start()
    {
        GaugeEnabled(false);
    }

    /// <summary>
    /// ゲージの表示非表示を設定する
    /// </summary>
    /// <param name="d">ゲージを表示するならtrue,非表示にするならfalse</param>
    public void GaugeEnabled(bool d)
    {
        gauge.enabled = d;
        frame.enabled = d;
    }

    /// <summary>
    /// ゲージをスタートする
    /// </summary>
    /// <param name="gaugeSpeed">
    /// ゲージ速度
    /// 大きいほど速くなる
    /// 偶数に近いと難易度が上がる
    /// 奇数に近いと難易度が下がる
    /// </param>
    public void GaugeMove(float gaugeSpeed)
    {
        gaugeWidth = Mathf.PingPong(MAX * gaugeSpeed * Mathf.Sin(Time.time), MAX);
        gauge.pixelInset = new Rect(gauge.pixelInset.xMin,
                                    gauge.pixelInset.yMin,
                                    gaugeWidth,
                                    gauge.pixelInset.height);
    }

    public float GetLastGaugeValue()
    {
        return gaugeWidth;
    }
}