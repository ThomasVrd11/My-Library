/*
 * ======================================================================================
 *                               IDataPersistence Interface
 * ======================================================================================
 * Interface for data persistence, defining methods for loading and saving game data.
 * Implemented by any class that needs to use or store persistent game data.
 *
 * Key Features:
 * - Defines a method for loading game data into the class.
 * - Defines a method for saving game data from the class.
 * ======================================================================================
 */

public interface IDataPersistence
{
    /* Load the game data into the class */
    void LoadData(GameData data);
    
    /* Save the game data from the class */
    void SaveData(GameData data);
}
