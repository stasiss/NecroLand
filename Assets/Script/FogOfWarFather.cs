using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FogOfWarFather : MonoBehaviour
{
    [HideInInspector] public bool isLooked;
    [HideInInspector] public bool isLastLooked;
    public GameObject[] childFog;
    // Start is called before the first frame update
    void Start()
    {
        Transform[] transforms = Array.FindAll(GetComponentsInChildren<Transform>(), child => child != this.transform &&
                                                                                     child.GetComponent<FogOfWarArea>() != null);
        childFog = new GameObject[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            childFog[i] = transforms[i].gameObject;
        }

    }
    public void StartFrame()
    {
        isLooked = false;
        foreach (GameObject area in childFog)
        {
            area.GetComponent<FogOfWarArea>().StartFrame();
        }
    }
    public void ControlArea(Vector3 pos, float fov)
    {
        foreach (GameObject area in childFog.OrderBy(f => Vector3.Distance(pos, f.transform.position)).ToList().GetRange(0, 12))
        {
            area.GetComponent<FogOfWarArea>().ControlVoxel(pos, fov);
        }
        isLooked = true;
    }

    public void DisplayVoxel()
    {
        isLastLooked = isLooked;
        foreach (var go in childFog)
        {
            go.GetComponent<FogOfWarArea>().EnabledVoxel();
        }
    }
    public void HideArea()
    {
        foreach (var go in childFog)
        {
            go.GetComponent<FogOfWarArea>().HideVoxel();
        }
    }
}
