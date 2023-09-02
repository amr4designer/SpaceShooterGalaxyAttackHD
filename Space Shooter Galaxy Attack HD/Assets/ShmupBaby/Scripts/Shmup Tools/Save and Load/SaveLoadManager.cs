using UnityEngine;
using UnityEngine.Networking;
using System.IO;

/// <summary>
/// Manages the save and load for any serialized Object using json.
/// </summary>
public static class SaveLoadManager
{
    /// <summary>
    /// The folder name that will contain the save files.
    /// </summary>
    private static string SaveFolderName = "ShmupBabay Save";

    /// <summary>
    /// The full save directory.
    /// </summary>
    private static string SaveFolderPath
    {
        get { return Path.Combine(Application.persistentDataPath, SaveFolderName); }
    }

    /// <summary>
    /// creates the directory if it does not exist.
    /// </summary>
    public static void CheckForDirectory()
    {
        if (!Directory.Exists(SaveFolderPath))
        {
            Directory.CreateDirectory(SaveFolderPath);
        }
    }

    /// <summary>
    /// Changes the name of the folder that contain the saved files,
    /// will also erase any existing data.
    /// </summary>
    /// <param name="name"></param>
    public static void ChangeSaveFolderName(string name)
    {
        if (Directory.Exists(SaveFolderPath))
            Directory.Delete(SaveFolderPath, true);

        SaveFolderName = name;

        CheckForDirectory();
    }

    /// <summary>
    /// Loads data from the save folder.
    /// </summary>
    /// <typeparam name="T">The type of data that's going to be loaded.</typeparam>
    /// <param name="fileName">the name of the file that contains the data.</param>
    /// <returns>the load data.</returns>
    public static T Load<T>( string fileName ) where T : class
    {
        if (fileName == null)
            return null;

        T loadData = null;

        //the full path name.
        string savePath = Path.Combine(SaveFolderPath, fileName + ".json");
        
        if (savePath.Contains("://"))
        {

            UnityWebRequest www = UnityWebRequest.Get(savePath);

            if (www == null)
            {
                return null;
            }

			#if UNITY_2017_2_OR_NEWER

            www.SendWebRequest();

			#else

			www.Send();

			#endif

            loadData = JsonUtility.FromJson<T>(www.downloadHandler.text);

        }
        
        if (File.Exists(savePath))
        {

            string dataAsJson = File.ReadAllText(savePath);

            loadData = JsonUtility.FromJson<T>(dataAsJson);
            
        }

        return loadData;

    }

    /// <summary>
    /// save data in file inside the save folder.
    /// </summary>
    /// <typeparam name="T">the type of the data to be saved.</typeparam>
    /// <param name="fileName">the name of the file that will contain the data.</param>
    /// <param name="data">the data to be saved.</param>
     public static void Save<T>(string fileName , T data ) where T : class
     {

        if (fileName == null)
            return;
        
        string savePath = Path.Combine(SaveFolderPath, fileName + ".json");

        if (!File.Exists(savePath))
        {
            StreamWriter saveFile = File.CreateText(savePath);
            saveFile.Close();
        }

        string savedDataAsJson = JsonUtility.ToJson(data);

        File.WriteAllText(savePath, savedDataAsJson);
        
    }

    /// <summary>
    /// Deletes hte save file inside the save folder.
    /// </summary>
    /// <param name="fileName">the name of the file that will be deleted.</param>
    /// <returns>returns true if the file was found.</returns>
    public static bool DeleteSaveFile(string fileName)
    {
        
        string filePath = Path.Combine(SaveFolderPath, fileName + ".json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }

        return false;
    }
    
}
