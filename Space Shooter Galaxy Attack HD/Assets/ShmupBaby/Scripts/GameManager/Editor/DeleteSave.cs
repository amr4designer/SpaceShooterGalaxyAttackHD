using UnityEngine;
using UnityEditor;
using ShmupBaby;

public class DeleteSave : ScriptableObject
{
    [MenuItem("Edit/Shmup Baby Delete Save Data")]
    static void Delete()
    {
        SaveLoadManager.DeleteSaveFile("Save Data");
		SaveLoadManager.DeleteSaveFile("Game Settings");
    }
}