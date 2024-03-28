using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Inventory", menuName = "Inventory/New Inventory")]
public class Inventory : ScriptableObject
{
    public List<ObjectDisplay> objectList = new List<ObjectDisplay>();



    public void ObjectManager(ObjectDisplay objectDisplay)
    {
        if (objectList.Find(p => p.objectName == objectDisplay.objectName) != null) 
        {
            objectList.Find(p => p.objectName == objectDisplay.objectName).objectHeld = objectDisplay.objectHeld;
            if (objectDisplay.objectHeld == 0)
            {
                objectList.Remove(objectDisplay);
            }
        }
        else
        {
            objectList.Add(objectDisplay);
        }
    }
    public void AddObject(ObjectDisplay objectDisplay)
    {
        if(objectList.Contains(objectDisplay))
        {
            objectList.Find(p => p.objectName == objectDisplay.objectName).objectHeld += objectDisplay.objectHeld;
        }
        else
        {
            objectList.Add(objectDisplay);
        }
    }

    public void ReduceObject(ObjectDisplay objectDisplay)
    {
        if(objectList.Find(p => p.objectName == objectDisplay.objectName).objectHeld <= objectDisplay.objectHeld)
        {
            objectList.Remove(objectList.Find(p => p.objectName == objectDisplay.objectName));
        }
        else
        {
            objectList.Find(p => p.objectName == objectDisplay.objectName).objectHeld -= objectDisplay.objectHeld;
        }

    }
}
