using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 参考
// http://notargs.com/blog/?p=313

/// <summary>
/// Rigidbodyの速度を保存しておくクラス
/// </summary>
public class RigidbodyPauser
{
    public Vector3 velocity;
    public Vector3 angularVeloccity;
    public Rigidbody rigidbody;

    public RigidbodyPauser(Rigidbody rigid)
    {
        rigidbody = rigid;
        Sleep();
    }

    public void Sleep()
    {

        velocity = rigidbody.velocity;
        angularVeloccity = rigidbody.angularVelocity;
        rigidbody.Sleep();
    }

    public void WakeUp()
    {
        rigidbody.WakeUp();
        rigidbody.velocity = velocity;
        rigidbody.angularVelocity = angularVeloccity;
    }
}

public class Pausable : MonoBehaviour
{

    /// <summary>
    /// 現在Pause中か？
    /// </summary>
    public bool pausing;

    /// <summary>
    /// 無視するGameObject
    /// </summary>
    public GameObject[] ignoreGameObjects;

    /// <summary>
    /// ポーズ状態が変更された瞬間を調べるため、前回のポーズ状況を記録しておく
    /// </summary>
    bool prevPausing;

    /// <summary>
    /// Rigidbodyのポーズ前の速度の配列
    /// </summary>
    RigidbodyPauser[] rigidbodyPauser;

    /// <summary>
    /// ポーズ中のMonoBehaviourの配列
    /// </summary>
    MonoBehaviour[] pausingMonoBehaviours;

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // ポーズ状態が変更されていたら、Pause/Resumeを呼び出す。
        if (prevPausing != pausing)
        {
            if (pausing) Pause();
            else Resume();
            prevPausing = pausing;
        }
    }

    /// <summary>
    /// 中断
    /// </summary>
    void Pause()
    {
        // Rigidbodyの停止
        // 子要素から、スリープ中でなく、IgnoreGameObjectsに含まれていないRigidbodyを抽出
        Predicate<Rigidbody> rigidbodyPredicate =
            obj => !obj.IsSleeping() &&
                   Array.FindIndex(ignoreGameObjects, gameObject => gameObject == obj.gameObject) < 0;
        var pausingRigidbodies = Array.FindAll(transform.GetComponentsInChildren<Rigidbody>(), rigidbodyPredicate);
        rigidbodyPauser = new RigidbodyPauser[pausingRigidbodies.Length];
        for (int i = 0; i < pausingRigidbodies.Length; i++)
        {
            // 速度、角速度も保存しておく
            rigidbodyPauser[i] = new RigidbodyPauser(pausingRigidbodies[i]);
        }

        // MonoBehaviourの停止
        // 子要素から、有効かつこのインスタンスでないもの、IgnoreGameObjectsに含まれていないMonoBehaviourを抽出
        Predicate<MonoBehaviour> monoBehaviourPredicate =
            obj => obj.enabled &&
                   obj != this &&
                   Array.FindIndex(ignoreGameObjects, gameObject => gameObject == obj.gameObject) < 0;
        pausingMonoBehaviours = Array.FindAll(transform.GetComponentsInChildren<MonoBehaviour>(), monoBehaviourPredicate);
        foreach (var monoBehaviour in pausingMonoBehaviours)
        {
            monoBehaviour.enabled = false;
        }

    }

    /// <summary>
    /// 再開
    /// </summary>
    void Resume()
    {
        // Rigidbodyの再開
        for (int i = 0; i < rigidbodyPauser.Length; i++)
        {
            rigidbodyPauser[i].WakeUp();
        }

        // MonoBehaviourの再開
        foreach (var monoBehaviour in pausingMonoBehaviours)
        {
            monoBehaviour.enabled = true;
        }
    }
}