using UnityEngine;

public class Main
{
    [RuntimeInitializeOnLoadMethod]
    public static void Bootstrap()
    {
        var gameManagerPrefab = Resources.Load("[GAME_MANAGER]");
        var gameManager = Object.Instantiate(gameManagerPrefab);
        gameManager.name = gameManagerPrefab.name;
    }
}
