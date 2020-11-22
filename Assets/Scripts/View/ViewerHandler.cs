using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerHandler : MonoBehaviour
{

    [Header("Base Textures")]
    public Sprite[] BASE_PLAYERS_TILES_SPRITES;
    public GameObject[] PLAYERS_PIECES;

    public GameObject BasePlayerHUD;
    public GameObject HUD_Canvas;

    [Header("Tile Map")]
    public GameObject[] TILE_MAP;


    private readonly Vector3[] playersOrientationOnTile = {new Vector3(.75f, .55f, 0), new Vector3(-.75f, -.15f, 0)};


    /// <summary>
    /// Initiating players HUDs for two players
    /// </summary>
    public void InitPlayersHUD() {

    }


    /// <summary>
    /// Placing a player piece on a specific tile
    /// </summary>
    /// <param name="player">  Player to place </param>
    /// <param name="newPosition"> The new tile index </param>
    public void PlacePlayerAtPosition(Player player, int newPosition) {
        Vector3 tempNewPosition = TILE_MAP[newPosition].transform.localPosition + playersOrientationOnTile[player.GetPlayerIndex()];
        player.GetPlayerPieceGameObjectn().transform.localPosition = tempNewPosition;
    }

    /// <summary>
    /// Moving (animating) a player piece to a specific tile
    /// </summary>
    /// <param name="player">  Player to place </param>
    /// <param name="newPosition"> The new tile index </param>
    public void MovePlayerToPosition(Player player, int newPosition) {
        Vector3 tempNewPosition = TILE_MAP[newPosition].transform.localPosition + playersOrientationOnTile[player.GetPlayerIndex()];
        StartCoroutine(MoveplayerOverSeconds(player.GetPlayerPieceGameObjectn(), tempNewPosition));

    }

    private IEnumerator MoveplayerOverSeconds(GameObject objectToMove, Vector3 endPosition) {
        float time = .5f;
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;

        while (elapsedTime < time) {
            objectToMove.transform.position = Vector3.Lerp(startingPos, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = endPosition;

//        gameData.isBringCardToFrontCoroutineRun = false;
    }
}
