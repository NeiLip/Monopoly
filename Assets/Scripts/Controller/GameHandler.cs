using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{

    GameData MainGameData;
    public ViewerHandler ViewerHandler;


     void Awake() {
        MainGameData = new GameData();


        MainGameData.PlayersHUD = new GameObject[MainGameData.NUMBER_OF_PLAYERS];
        for (int i = 0; i < MainGameData.NUMBER_OF_PLAYERS; i++) {
            MainGameData.PlayersHUD[i] = Instantiate(ViewerHandler.BasePlayerHUD);
            MainGameData.PlayersHUD[i].transform.SetParent(ViewerHandler.HUD_Canvas.transform);
            MainGameData.PlayersHUD[i].transform.localScale = new Vector3(1f,1f,1f);
        }

        MainGameData.PlayersHUD[0].transform.localPosition = new Vector3(-88.7f, 71.8f, 0f);


        MainGameData.PlayersHUD[1].transform.Find("PlayerName_Text").GetComponent<Text>().alignment = TextAnchor.MiddleRight;
        MainGameData.PlayersHUD[1].transform.Find("PlayerMoney_Text").GetComponent<Text>().alignment = TextAnchor.MiddleRight;
    }
    // Start is called before the first frame update
    void Start()
    {
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W)) {
            MainGameData.players[0].SetMovesLeft(4);
      
            MainGameData.players[0].SetCurrentPosition((MainGameData.players[0].GetCurrentPosition() + 1) % MainGameData.gameTileMap.Length);


            ViewerHandler.MovePlayerToPosition(MainGameData.players[0], MainGameData.players[0].GetCurrentPosition());
        }
    }







    public void ResetGame() {

        MapHandlingAtStart();

        PlayersHandlingAtStart();


    }

    void MapHandlingAtStart() {
        MainGameData.gameTileMap = new Tile[ViewerHandler.TILE_MAP.Length];

        for (int i = 0; i < MainGameData.gameTileMap.Length; i++) {
            
            MainGameData.gameTileMap[i] = new Tile(ViewerHandler.TILE_MAP[i].gameObject, i);

            //Alway the starting point
            if (i == 0) {
                MainGameData.gameTileMap[i] = new SpecialTile(SpecialTile.TileType.StartingPoint, 200);
            }
            //Bonus points
            else if (i == 6 || i == 12 || i == 18) MainGameData.gameTileMap[i] = new SpecialTile(SpecialTile.TileType.Bonus, 200);
            //Property
            else {
                MainGameData.gameTileMap[i] = new Property(-1, 400, 20);
            }
        }
    }

    void PlayersHandlingAtStart() {
        MainGameData.players = new Player[MainGameData.NUMBER_OF_PLAYERS];

        MainGameData.whosTurnIsIt = 0;

        for (int i = 0; i < MainGameData.players.Length; i++) {
            MainGameData.players[i] = new Player();
            MainGameData.players[i].SetPlayerIndex(i);
            MainGameData.players[i].SetPlayerPieceGameObjectn(ViewerHandler.PLAYERS_PIECES[i]); //setting player's piece
            MainGameData.players[i].SetCurrentPosition(0);
            MainGameData.players[i].SetMoney(MainGameData.STARTING_AMOUNT_OF_MONEY);
            MainGameData.players[i].SetMovesLeft(0);

            if (i == MainGameData.whosTurnIsIt) MainGameData.players[i].SetPlayerState(Player.PlayerState.RollingDice);
            else MainGameData.players[i].SetPlayerState(Player.PlayerState.Waiting);


            ViewerHandler.PlacePlayerAtPosition(MainGameData.players[i], 0);
        }
    }




}
