using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实际的物品及其数量
/// </summary>
[CreateAssetMenu(fileName = "New Object", menuName = "Inventory/New Object")]
public class ObjectDisplay : ScriptableObject
{
    public string objectName;
    public Sprite objectImage;
    public int objectHeld;

}
