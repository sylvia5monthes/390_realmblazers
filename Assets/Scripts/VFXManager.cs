using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;
    public GameObject slashEffectPrefab;
    public GameObject shootEffectPrefab;
    public GameObject holyEffectPrefab;
    public GameObject cureEffectPrefab;
    public GameObject poundEffectPrefab;
    public GameObject fireballEffectPrefab;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayActionVFX(string actionName, Vector3 worldPosition)
    {
        switch (actionName)
        {
            // knight 
            case "Slash":
                Debug.Log("Playing Slash VFX");
                Instantiate(slashEffectPrefab, worldPosition, Quaternion.identity);
                break;

            // archer
            case "Shoot":
                Debug.Log("Playing Shoot VFX");
                Instantiate(shootEffectPrefab, worldPosition, Quaternion.identity);
                break;

            case "Snipe":
                Debug.Log("Playing Snipe VFX");
                Instantiate(shootEffectPrefab, worldPosition, Quaternion.identity);
                break;
            
            // white mage 
            case "Holy":
                Debug.Log("Playing Holy VFX");
                Instantiate(holyEffectPrefab, worldPosition, Quaternion.identity);
                break;
            
            case "Cure":
                Debug.Log("Playing Cure VFX");
                Instantiate(cureEffectPrefab, worldPosition, Quaternion.identity);
                break;

            // black mage 
            case "Fire":
                Debug.Log("Playing Fire VFX");
                Instantiate(fireballEffectPrefab, worldPosition, Quaternion.identity);
                break;
            
            case "Fireball":
                Debug.Log("Playing Fireball VFX");
                Instantiate(fireballEffectPrefab, worldPosition, Quaternion.identity);
                break;

            // goblin
            case "Pound":
                Debug.Log("Playing Pound VFX");
                Instantiate(poundEffectPrefab, worldPosition, Quaternion.identity);
                break;

            // ice demon
            case "Smash":
                Debug.Log("Playing Smash VFX");
                Instantiate(poundEffectPrefab, worldPosition, Quaternion.identity);
                break;


            // Add more cases here for other abilities
        }
    }
}
