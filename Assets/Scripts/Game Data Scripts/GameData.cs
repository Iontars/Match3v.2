using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;


[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;

}
/// <summary>
/// Хранение игровых данных
/// </summary>
public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;

    private void Awake()
    {
        if (gameData == null)
        {           
            DontDestroyOnLoad(gameObject);
            gameData = GetComponent<GameData>(); // same as keyword "this"
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Load();
    }

    public void Save()
    {
        // создать бинарный формат для чтения бинарных файлов
        BinaryFormatter formatter = new();
        // создать маршрут от программы к файлу
        //File.Create(Application.persistentDataPath + PlayerPrefsStorage.pathToSaveFile);
        FileStream file = File.Open(Application.persistentDataPath + PlayerPrefsStorage.pathToSaveFile, FileMode.Open);
        // создать новый объект SaveData, значит перезапистаь данные
        SaveData data = new();
        data = saveData;
        // сохранить данные в файл
        formatter.Serialize(file, data);
        // закрыть стрим к файлу
        file.Close();
        Debug.Log("Data saved");
    }

    public void Load()
    {
        // Проверить существует ли файл загрузки
        if (File.Exists(Application.persistentDataPath + PlayerPrefsStorage.pathToSaveFile))
        {
            // создать бинарный формат для чтения бинарных файлов
            BinaryFormatter formatter = new();
            FileStream file = File.Open(Application.persistentDataPath + PlayerPrefsStorage.pathToSaveFile, FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            Debug.Log("Data loaded");
        }
    }

    private void OnDisable()
    {
        Save();
    }
}
