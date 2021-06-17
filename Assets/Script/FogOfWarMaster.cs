using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
public class FogOfWarMaster : MonoBehaviour
{
    public bool isNecro;
    public GameObject[] childFog;
    // Start is called before the first frame update
    void Start()
    {
        Transform[] transforms = Array.FindAll(GetComponentsInChildren<Transform>(), child => child != this.transform &&
                                                                                     child.GetComponent<FogOfWarFather>() != null);
        childFog = new GameObject[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            childFog[i] = transforms[i].gameObject;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<GameObject> unites = isNecro ? GameManager.instance.armeeNecro : GameManager.instance.armeeHuman;
        if (unites.Count > 0)
        {
            foreach (var item in childFog.Where(area => area.GetComponent<FogOfWarFather>().isLooked))
            {
                item.GetComponent<FogOfWarFather>().StartFrame();
            }
            foreach (GameObject unit in unites.Where(u => u != null && u.GetComponent<Unit>() != null && u.GetComponent<Unit>().isAlive))
            {
                Vector3 posUnit = unit.transform.position;
                float visionUnit = unit.GetComponent<Unit>().fieldOfView;
                foreach (var item in childFog.OrderBy(f => Vector3.Distance(posUnit, f.transform.position)).ToList().GetRange(0, 4))
                {
                    item.GetComponent<FogOfWarFather>().ControlArea(posUnit, visionUnit);
                }
            }
            foreach (var go in childFog.Where(e => e.GetComponent<FogOfWarFather>().isLastLooked && !e.GetComponent<FogOfWarFather>().isLooked))
            {
                go.GetComponent<FogOfWarFather>().HideArea();
            }
            foreach (var go in childFog.Where(e => e.GetComponent<FogOfWarFather>().isLooked))
            {
                go.GetComponent<FogOfWarFather>().DisplayVoxel();
            }
        }
    }
}
