using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ʵ�ʵ���Ʒ��������
/// </summary>
[CreateAssetMenu(fileName = "New Object", menuName = "Inventory/New Object")]
public class ObjectDisplay : ScriptableObject
{
    public string objectName;
    public Sprite objectImage;
    public int objectHeld;

}
