using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scrip that attaches to a VFX prefab and destroys it after a certain time
public class VFXAutoDestroy : MonoBehaviour
{
    public float lifetime = 2.0f; // Duration before destruction
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy this GameObject after the specified lifetime
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
