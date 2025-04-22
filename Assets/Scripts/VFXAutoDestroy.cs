using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXAutoDestroy : MonoBehaviour
{
    public float lifetime = 2.0f; // Duration before destruction
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
