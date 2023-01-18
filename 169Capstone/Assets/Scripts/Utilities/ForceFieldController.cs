using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldController : MonoBehaviour
{
    public List<GameObject> forceFields = new List<GameObject>();
    
    public List<GameObject> guardRails = new List<GameObject>();

    public void EnableForceFields()
    {
        // Enables the force field object
        foreach(GameObject ff in forceFields){
            ff.SetActive(true);
            ff.GetComponent<SFXTrigger>().PlaySFX();
        }
    }
    
    public void DisableForceFields()
    {
        foreach(GameObject ff in forceFields){
            ff.SetActive(false);
            ff.GetComponent<SFXTrigger>().PlaySecondarySFX();
        }
    }

    public void EnableGuardRails()
    {
        // Enables the force field object
        foreach(GameObject gr in guardRails){
            gr.SetActive(true);
        }
    }
    
    public void DisableGuardRails()
    {
        foreach(GameObject gr in guardRails){
            gr.SetActive(false);
        }
    }
}
