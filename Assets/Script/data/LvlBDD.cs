using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;
[CreateAssetMenu(fileName = "NewDatabaseSkill", menuName = "Base de données Level")]
public class LvlBDD : ScriptableObject
{
    private const string DataObjectPath = "/Prefabs/Level"; // Chemin vers le répertoire d'Assets de Données
    public LvlData[] dataAsArray;
    public LvlData Get(int id)
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
        dataAsArray = new LvlData[paths.Length];
        for (var i = 0; i < paths.Length; ++i)
        {
            var path = paths[i].Replace(Application.dataPath, "Assets");
            Debug.Log("Path to load : " + path);
            var asset = AssetDatabase.LoadAssetAtPath<LvlData>(path);
            dataAsArray[i] = asset;
        }
    }
#endif
}