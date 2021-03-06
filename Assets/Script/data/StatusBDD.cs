using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;
[CreateAssetMenu(fileName = "NewDatabaseStatus", menuName = "Base de données Status")]
public class StatusBDD : ScriptableObject
{
    private const string DataObjectPath = "/Prefabs/Status"; // Chemin vers le répertoire d'Assets de Données
    public StatusData[] dataAsArray;
    public StatusData Get(int id)
    {
        for (var i = 0; i < dataAsArray.Length; ++i)
        {
            if (dataAsArray[i].Id.Equals(id))
            {
                return dataAsArray[i];
            }
        }
        Debug.LogError("objet non trouvé");
        return null;
    }
#if UNITY_EDITOR
    // Charge la base de données
    [ContextMenu("Recharger la base de données")] // Permet de créer un menu contextuel sur l'Asset
    public void FillDatabase()
    {
        var paths = Directory.GetFiles(Application.dataPath + DataObjectPath, "*.asset", SearchOption.AllDirectories);
        dataAsArray = new StatusData[paths.Length];
        for (var i = 0; i < paths.Length; ++i)
        {
            var path = paths[i].Replace(Application.dataPath, "Assets");
            Debug.Log("Path to load : " + path);
            var asset = AssetDatabase.LoadAssetAtPath<StatusData>(path);
            dataAsArray[i] = asset;
        }
    }
#endif
}