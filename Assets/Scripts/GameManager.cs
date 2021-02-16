using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    const string Gamedata = "GameData.bin";
    [Tooltip("The amount of coins the player has")]
    public int coinCount;
    [Tooltip("The amount of the main collectibles the player has")]
    public int CollectibleCount;
    // Start is called before the first frame update
    void Start()
    {
        //this is how you'd load prev game data 

        //string Filepath = GetCurDir() + "/" + Gamedata;

        //Save s = LoadGameData();
        //coinCount = s.CoinCount;
        //CollectibleCount = s.MainCollectibleCount;
    }

    // Update is called once per frame
    void Update()
    {
        //call this when you want to save the game 
        //SaveGame();
    }
    public void lockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void unlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SaveGame()
    {
        Save s = new Save();
        SaveGameData(s);
    }


    public void SaveGameData(Save save)
    {
        string path = GetCurDir() + "/" + Gamedata;


        Stream outstream = File.Create(path);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(outstream, save);
        outstream.Close();


    }
    public void DeleteGameData()
    {
        string path = GetCurDir() + "/" + Gamedata;


        if (File.Exists(path))
        {
            File.Delete(path);
        }


    }

    public Save LoadGameData()
    {
        string path = GetCurDir() + "/" + Gamedata;
        Save save = new Save();
        if (File.Exists(path))
        {
            Stream inStream = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();
            save = (Save)bf.Deserialize(inStream);
            inStream.Close();

        }
        return save;
    }
    public string GetCurDir()
    {
        return Directory.GetCurrentDirectory();
    }
}
