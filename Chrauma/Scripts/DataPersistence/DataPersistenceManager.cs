/*
 * ======================================================================================
 *                           DataPersistenceManager Script
 * ======================================================================================
 * This script manages the persistence of game data, including saving and loading game
 * state. It uses a file handler to store data to a file and retrieves data from it.
 * Implements singleton pattern to ensure only one instance is active at any time.
 *
 * Key Features:
 * - Initializes and handles game data storage.
 * - Implements the IDataPersistence interface to save and load data.
 * - Manages the loading of saved data and handles scene transitions.
 * - Checks if saved game data exists.
 * ======================================================================================
 */

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File storage config")]
    [SerializeField] private string fileName;  // * Name of the file where game data is stored
    private GameData gameData;  // * Object to hold the game data
    private List<IDataPersistence> dataPersistencesObjects;  // * List of objects that use the IDataPersistence interface
    private FileDataHandler dataHandler;  // * Handler for file operations
    public static DataPersistenceManager instance { get; private set; }  // * Singleton instance
    public bool isLoading = false;  // * Flag to check if the game has loaded something
    public Vector3 loadedPlayerPos;  // * Position of the player that was saved

    private void Awake()
    {
        // * Singleton pattern to ensure only one instance
        if (instance != null)
        {
            Debug.LogError("More than one Data Persistence Manager in the scene");
            Destroy(gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        // * Initialize the FileDataHandler with the specified file name
        // * Find all objects that implement the IDataPersistence interface
        this.dataHandler = new FileDataHandler(Application.dataPath, fileName);
        this.dataPersistencesObjects = FindAllDataPersistenceObjects();
        Debug.Log(dataPersistencesObjects);
    }

    public void NewGame()
    {
        // * Initialize a new GameData object when starting a new game
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // * Load the game data from the file and update all data persistence objects
        // * Retrieve the saved player position and change scene to the saved scene ID
        this.gameData = dataHandler.Load();
        if (this.gameData == null)
        {
            return;
        }
        isLoading = true;
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        loadedPlayerPos = gameData.playerPos;
        GameManager.instance.SwitchScene(gameData.scene);
    }

    public void SaveGame()
    {
        // * Retrieve the data from all data persistence objects and scene ID
        // * Save the current game data to the file
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }
        gameData.scene = SceneManager.GetActiveScene().buildIndex;
        dataHandler.Save(gameData);
    }

    public bool CheckIfSave()
    {
        // * Check if there is a saved game data file
        // * Return true if save exists, otherwise false
        this.gameData = dataHandler.Load();
        if (this.gameData != null)
        {
            return true;
        }
        else return false;
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // * Find all objects in the scene that implement the IDataPersistence interface
        // * Return list of IDataPersistence objects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    private void OnEnable()
    {
        // * Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // * Unsubscribe to the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // * Called when a new scene is loaded
        // * Refresh the list of data persistence objects
        RetrieveDataPersistencesObjects();
        // Debug.Log(dataPersistencesObjects);
    }

    public void DebugList()
    {
        // * Debugging method to list all data persistence objects in console
        Debug.Log("Debugging");
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            Debug.Log("obj:");
            Debug.Log(dataPersistenceObj.ToString());
        }
    }

    public void RetrieveDataPersistencesObjects()
    {
        // * Refresh the list of data persistence objects
        this.dataPersistencesObjects = FindAllDataPersistenceObjects();
    }
}
