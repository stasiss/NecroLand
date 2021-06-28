using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;
[CreateAssetMenu(fileName = "NewDatabaseUnit", menuName = "Base de données Unite")]
public class UniteBDD : ScriptableObject
{
    private const string DataObjectPath = "/Prefabs/Unite"; // Chemin vers le répertoire d'Assets de Données
    public UniteData[] dataAsArray;
    public UniteData Get(int id)
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
        dataAsArray = new UniteData[paths.Length];
        for (var i = 0; i < paths.Length; ++i)
        {
            var path = paths[i].Replace(Application.dataPath, "Assets");
            Debug.Log("Path to load : " + path);
            var asset = AssetDatabase.LoadAssetAtPath<UniteData>(path);
            dataAsArray[i] = asset;
        }
    }
#endif
}
