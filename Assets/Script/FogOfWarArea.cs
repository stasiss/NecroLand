using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogOfWarArea : MonoBehaviour
{
    public GameObject[] voxelFog;
    public bool[] voxelLooked;
    // Start is called before the first frame update
    void Start()
    {
        Transform[] transforms = Array.FindAll(GetComponentsInChildren<Transform>(), child => child != this.transform);
        voxelFog = new GameObject[transforms.Length];
        voxelLooked = new bool[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            voxelFog[i] = transforms[i].gameObject;
        }
    }
    public void StartFrame()
    {
        for (int i = 0; i < voxelLooked.Length; i++)
        {
            voxelLooked[i] = false;
        }
    }
    public void ControlVoxel(Vector3 pos, float fov)
    {
        for (int i = 0; i < voxelFog.Length; i++)
        {
            if (!voxelLooked[i] && Vector3.Distance(pos, voxelFog[i].transform.position) < fov)
                voxelLooked[i] = true;
        }
    }

    public void HideVoxel()
    {
        foreach (GameObject area in voxelFog.Where(v => v.GetComponent<MeshRenderer>().enabled == false))
            area.GetComponent<MeshRenderer>().enabled = true;
    }

    public void EnabledVoxel()
    {
        for (int i = 0; i < voxelFog.Length; i++)
        {
            if (voxelLooked[i] && voxelFog[i].GetComponent<MeshRenderer>().enabled)
                voxelFog[i].GetComponent<MeshRenderer>().enabled = false;
            else if (!voxelLooked[i] && !voxelFog[i].GetComponent<MeshRenderer>().enabled)
                voxelFog[i].GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
