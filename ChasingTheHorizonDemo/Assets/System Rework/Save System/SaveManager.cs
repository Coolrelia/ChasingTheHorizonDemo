using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static string directory = "/SaveData";
    public static string fileName = "saveData";

    private readonly string encryptionCodeWord = "beyblade";

    public void Save(SaveObject saveObject)
    {
        // create the directory the file will be writen to if it doesn't already exist
        string fullPath = Path.Combine(directory, fileName);
        if(!Directory.Exists(fullPath))
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

        // serialize and encrypt c# game data into Json
        string json = JsonUtility.ToJson(saveObject, true);
        string dataToStore = EncryptToDecrypt(json);
        
        // write serialized data to the file
        using(FileStream stream = new FileStream(fullPath, FileMode.Create))
        {
            using(StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(dataToStore);
                print("data saved");
            }
        }
    }

    public SaveObject Load()
    {
        string fullPath = Path.Combine(directory, fileName);
        SaveObject saveObject = new SaveObject();

        if(File.Exists(fullPath))
        {
            string dataToLoad = "";
            using(FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using(StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            // decrypt the data
            dataToLoad = EncryptToDecrypt(dataToLoad);

            // deserialize the data from Json back to C# object
            saveObject = JsonUtility.FromJson<SaveObject>(dataToLoad);
        }

        print("Data Loaded");
        return saveObject;
    }

    private string EncryptToDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}
