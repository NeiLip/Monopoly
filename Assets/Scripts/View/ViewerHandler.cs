using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewerHandler : MonoBehaviour
{

    [Header("Base Textures")]
    public Sprite[] DIE_SPRITES;
    public Sprite[] BASE_PLAYERS_TILES_SPRITES;
    public GameObject[] PLAYERS_PIECES;
    public GameObject BasePlayerHUD;

    [Header("Gameobjects")]
    public GameObject HUD_Canvas;
    public GameObject DIE_BACKGROUND;
    public GameObject CURRENT_DIE;

    [Header("Tile Map")]
    public GameObject[] TILE_MAP;

    [Header("Windows")]
    public GameObject ROLL_THE_DIE_WINDOW;

    private readonly Vector3[] playersOrientationOnTile = {new Vector3(.75f, .55f, 0), new Vector3(-.75f, -.15f, 0)};


    /// <summary>
    /// Initiating players HUDs for two players
    /// </summary>
    public void InitPlayersHUD(GameData MainGameData) {

        MainGameData.PlayersHUD = new GameObject[MainGameData.NUMBER_OF_PLAYERS];

        for (int i = 0; i < MainGameData.NUMBER_OF_PLAYERS; i++) {
            MainGameData.PlayersHUD[i] = Instantiate(BasePlayerHUD);
            MainGameData.PlayersHUD[i].transform.SetParent(HUD_Canvas.transform);
            MainGameData.PlayersHUD[i].transform.localScale = new Vector3(1f, 1f, 1f);

            if (i == 0) {//player 1
                MainGameData.PlayersHUD[0].transform.localPosition = new Vector3(-88.5f, 71.8f, 0f);


            }
            else if (i == 1) {//player2
                MainGameData.PlayersHUD[1].transform.localPosition = new Vector3(88.5f, 71.8f, 0f);
                MainGameData.PlayersHUD[1].transform.Find("PlayerName_Text").GetComponent<Text>().text = "PLayer 2";
                MainGameData.PlayersHUD[1].transform.Find("PlayerName_Text").GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                MainGameData.PlayersHUD[1].transform.Find("PlayerMoney_Text").GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                MainGameData.PlayersHUD[1].transform.Find("active_player_indication").gameObject.SetActive(false);
            }
        }

    }


    public void ShowWindow(GameObject window) {
        window.SetActive(true);
    }
    public void HideWindow(GameObject window) {
        window.SetActive(false);
    }

    public void UpdateHUD(GameData MainGameData) {
        for (int i = 0; i < MainGameData.NUMBER_OF_PLAYERS; i++) {
            if (i == MainGameData.whosTurnIsIt)
                MainGameData.PlayersHUD[i].transform.Find("active_player_indication").gameObject.SetActive(true);

            else
                MainGameData.PlayersHUD[i].transform.Find("active_player_indication").gameObject.SetActive(false);

            MainGameData.PlayersHUD[i].transform.Find("PlayerMoney_Text").GetComponent<Text>().text = MainGameData.players[i].GetMoney().ToString();
        }
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
    public void MovePlayerToPosition(GameHandler gameHandler,Player player, int newPosition) {
        Vector3 tempNewPosition = TILE_MAP[newPosition].transform.localPosition + playersOrientationOnTile[player.GetPlayerIndex()];
        StartCoroutine(MoveplayerOverSeconds(gameHandler, player.GetPlayerPieceGameObjectn(), tempNewPosition));

    }

    public void UpdateCurrentDie(int rolledNumber) {
        CURRENT_DIE.GetComponent<SpriteRenderer>().sprite = DIE_SPRITES[rolledNumber - 1];
    }

    private IEnumerator MoveplayerOverSeconds(GameHandler gameHandler, GameObject objectToMove, Vector3 endPosition) {
        float time = .5f;
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;

        while (elapsedTime < time) {
            objectToMove.transform.position = Vector3.Lerp(startingPos, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = endPosition;

        gameHandler.PlayTurn();
//        gameData.isBringCardToFrontCoroutineRun = false;
    }
}
