using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TransportCreation : MonoBehaviour
{
    public Canvas canvas;
    public PlayerController player;
    public GameObject prefabCharrette;
    [HideInInspector] public GameObject villageTarget;
    [HideInInspector] public GameObject[] villages;
    public Button[] buttonsVillages;
    // Start is called before the first frame update
    public void InitGui()
    {
        int y = 0;
        Vector3 castlePos = GameObject.Find("Castle").transform.position;
        villages = GameObject.FindGameObjectsWithTag("Village").Where(v => v.GetComponent<Village>()).OrderBy(v => Vector3.Distance(v.transform.position, castlePos)).ToArray();
        if (villages.Count() != 10)
            Debug.LogError("Le nombre de village n'est pas le bon");

        foreach (GameObject item in villages)
        {
            buttonsVillages[y].GetComponentInChildren<Text>().text = item.name;
            y++;
        }
        villageTarget = villages[0];
        //for (int i = 0; i < 4; i++)
        //{
        //    WriteText(i);
        //}
        canvas.enabled = true;
    }

    public void ChoiceVillage(int village)
    {
        villageTarget = villages[village];
        Validate();
        //for (int i = 0; i < 4; i++)
        //{
        //    WriteText(i);
        //}
    }

    //public void MaxRessource(int ressource)
    //{
    //    int actualOrder = orders[0] + orders[1] + orders[2] - orders[ressource];
    //    orders[ressource] = maximumOrder - actualOrder;
    //    WriteText(ressource);
    //}
    //public void UpRessource(int ressource)
    //{
    //    if(orders[0] + orders[1] + orders[2] < maximumOrder)
    //        orders[ressource] += 1;
    //    WriteText(ressource);
    //}
    //public void DownRessource(int ressource)
    //{
    //    if (orders[0] + orders[1] + orders[2] > -maximumOrder)
    //        orders[ressource] -= 1;
    //    WriteText(ressource);
    //}
    //public void WriteText(int ressource)
    //{
    //    string ressText;
    //    int ressArrivalVillage;
    //    switch (ressource)
    //    {
    //        case 0:
    //            ressText = "food";
    //            ressArrivalVillage = villageTarget.GetComponent<Village>().food;
    //            break;
    //        case 1:
    //            ressText = "wood";
    //            ressArrivalVillage = villageTarget.GetComponent<Village>().wood;
    //            break;
    //        case 2:
    //            ressText = "iron";
    //            ressArrivalVillage = villageTarget.GetComponent<Village>().iron;
    //            break;
    //        case 3:
    //        default:
    //            ressText = "colon";
    //            ressArrivalVillage = villageTarget.GetComponent<Village>().popPaysan.Count;
    //            break;
    //    }
    //    textsStart[ressource].text = $"{orders[ressource]} {ressText}";
    //    textsArrival[ressource].text = $"{ressArrivalVillage - orders[ressource]} {ressText}";
    //}
    public void Cancel()
    {
        canvas.enabled = false;
    }

    public void Validate()
    {
        player.ValideCreateCharette(villageTarget);

        canvas.enabled = false;
    }
}
