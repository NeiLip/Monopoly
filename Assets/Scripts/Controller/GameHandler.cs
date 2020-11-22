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


        ViewerHandler.InitPlayersHUD(MainGameData);

    }
    // Start is called before the first frame update
    void Start()
    {
        ResetGame();
    }

  



    public void RollTheDie() {
        ViewerHandler.HideWindow(ViewerHandler.ROLL_THE_DIE_WINDOW);

        int tempRoll = Random.Range(1, 7);
        ViewerHandler.UpdateCurrentDie(tempRoll);

        Debug.Log("Rolled: " + tempRoll);
        MainGameData.players[MainGameData.whosTurnIsIt].SetMovesLeft(tempRoll);

        PlayTurn();

    }


    public void PlayTurn() {
        if(MainGameData.players[MainGameData.whosTurnIsIt].GetMovesLeft() <= 0) {
            ReachedFinalTile();
            return;
        }

        else {
            MainGameData.players[MainGameData.whosTurnIsIt].SetCurrentPosition((MainGameData.players[MainGameData.whosTurnIsIt].GetCurrentPosition() + 1) % MainGameData.gameTileMap.Length);

            
            ViewerHandler.MovePlayerToPosition(this, MainGameData.players[MainGameData.whosTurnIsIt], MainGameData.players[MainGameData.whosTurnIsIt].GetCurrentPosition());
            MainGameData.players[MainGameData.whosTurnIsIt].MoveOneTile();
        }



    }


    void ReachedFinalTile() {

        MainGameData.IncreaseWhosTurnIsIt();

        ViewerHandler.UpdateWhosTurnIsItIndicator(MainGameData);
        ViewerHandler.ShowWindow(ViewerHandler.ROLL_THE_DIE_WINDOW);
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
