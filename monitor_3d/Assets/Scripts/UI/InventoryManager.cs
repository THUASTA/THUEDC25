using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manage the Inventory
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public Inventory myBag;
    public GameObject slotGrid;
    public Slot slotPrefab;
    public ObjectDisplay Emerald;
    public ObjectDisplay Wool;
    public delegate void ManageObject(ObjectDisplay objectDiaplay);
    public int playerId = -1;

    public void InventoryUpdate(int emeraldHeld, int woolHeld)
    {
        Emerald.objectHeld = emeraldHeld;
        Wool.objectHeld = woolHeld;
        myBag.ObjectManager(Emerald);
        myBag.ObjectManager(Wool);
        RefreshObject();
    }
    public void CreatNewObject(ObjectDisplay objectDiaplay)
    {
        Slot newObject = Instantiate(slotPrefab, slotGrid.transform.position, Quaternion.identity);
        newObject.gameObject.transform.SetParent(slotGrid.transform);
        newObject.slotObject = objectDiaplay;
        newObject.slotImage.sprite = objectDiaplay.objectImage;
        newObject.slotNum.text = objectDiaplay.objectHeld.ToString();
    }

    /// <summary>
    /// �������ظ���Ʒ�����±�����ʾ
    /// </summary>
    public void RefreshObject()
    {
        //for (int i = 0; i < instance.transform.childCount; i++) 
        //{
        //    if (instance.slotGrid.transform.childCount == 0) 
        //    {
        //        break;
        //    }
        //    Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
        //}

        //for (int i = 0; i < instance.myBag.objectList.Count; i++)
        //{
        //    CreatNewObject(instance.myBag.objectList[i]);
        //}
    }
}
