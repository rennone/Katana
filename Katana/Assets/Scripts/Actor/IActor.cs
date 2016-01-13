using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ActorInitializeFinalize))]
public class IActor : MonoBehaviour
{
    public virtual void Damage(int val) { }
    public virtual void Recover(int val) { }
}
