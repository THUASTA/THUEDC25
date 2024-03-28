using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreRotate : MonoBehaviour
{
    // Start is called before the first frame update
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, 0.4f);
    }
}
