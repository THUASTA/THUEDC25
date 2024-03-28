using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public delegate void BlockAddEvent(Vector3 position);
    public delegate void BlockRemoveEvent(Vector3 position);
    public delegate void IronOreFormation(Vector3 position);
    public delegate void GoldMineFormation(Vector3 position);
    public delegate void DiamondMineFormation(Vector3 position);
    public delegate void IronOreDisappear(Vector3 position);
    public delegate void GoldMineDisappear(Vector3 position);
    public delegate void DiamondMineDisappear(Vector3 position);
    public delegate void BreakBed(Vector3 position);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
