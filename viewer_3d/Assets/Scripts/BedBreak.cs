using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedBreak : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BreakBed(Vector3 position)
    {
        if(transform.position == position)
        {
            Destroy(gameObject);
        }
        
    }
}
