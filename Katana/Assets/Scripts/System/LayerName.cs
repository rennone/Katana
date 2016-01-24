using UnityEngine;
/// <summary>
/// タグ名を定数で管理するクラス
/// </summary>
public static class LayerName
{
	public static int Default { get{ return LayerMask.NameToLayer("Default"); } }
	public static int TransparentFX { get{ return LayerMask.NameToLayer("TransparentFX"); } }
	public static int IgnoreRaycast { get{ return LayerMask.NameToLayer("Ignore Raycast"); } }
	public static int Water { get{ return LayerMask.NameToLayer("Water"); } }
	public static int UI { get{ return LayerMask.NameToLayer("UI"); } }
	public static int Player { get{ return LayerMask.NameToLayer("Player"); } }
	public static int PlayerDamaged { get{ return LayerMask.NameToLayer("PlayerDamaged"); } }
	public static int Enemy { get{ return LayerMask.NameToLayer("Enemy"); } }
}
