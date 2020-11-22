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

        ViewerHandler.ShowWindow(ViewerHandler.MAIN_MENU_WINDOW);
        ViewerHandler.HideWindow(ViewerHandler.GAME_LOG_WINDOW);

   

    }
    // Start is called before the first frame update
    void Start()
    {
     //   ResetGame();
    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
           
            SpecialTile temp = (SpecialTile)MainGameData.gameTileMap[6];

            Debug.Log("Reward: " + temp.GetReward());
           
        }
    }


    public void UpdateLogWindow() {
        if (MainGameData.state == GameData.State.RollDie) {
            RollTheDie();
            MainGameData.state = GameData.State.Moving;
        }

        else { //Playing
            
            MainGameData.state = GameData.State.RollDie;
            ViewerHandler.UpdateLogWindow(MainGameData, -1);
        }
    }


    public void RollTheDie() {
        ViewerHandler.HideWindow(ViewerHandler.GAME_LOG_WINDOW);

        int tempRoll = Random.Range(1, 7);
        ViewerHandler.UpdateCurrentDie(tempRoll);

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
        Tile tempTile = MainGameData.gameTileMap[MainGameData.players[MainGameData.whosTurnIsIt].GetCurrentPosition()];

        Debug.Log("Tile name" + tempTile.GetTileIndex());

        if (tempTile.GetType() == typeof(Property)) {
            Property tempProperty = (Property)tempTile;

            if (tempProperty.GetOwnedByPlayerIndex() == MainGameData.whosTurnIsIt) { // This property is already yours

            }
            else if (tempProperty.GetOwnedByPlayerIndex() == -1) { //This property is waiting for purchase
                MainGameData.players[MainGameData.whosTurnIsIt].DecreaseMoney(tempProperty.GetCostPrice());
                tempProperty.GetTilegameObject().GetComponent<SpriteRenderer>().sprite = ViewerHandler.BASE_PLAYERS_TILES_SPRITES[MainGameData.whosTurnIsIt];
                tempProperty.SetOwnedByPlayerIndex(MainGameData.whosTurnIsIt);

                ViewerHandler.UpdateLogWindow(MainGameData, tempProperty.GetCostPrice());
            }
            else { //Other player already purchased this property
                MainGameData.players[(MainGameData.whosTurnIsIt)].DecreaseMoney(tempProperty.GetFinePrice());//Takes money from the player and gives it to the player owning this property
                MainGameData.players[(MainGameData.whosTurnIsIt + 1) % MainGameData.NUMBER_OF_PLAYERS].IncreaseMoney(tempProperty.GetFinePrice());//Takes money from the player and gives it to the player owning this property

                ViewerHandler.UpdateLogWindow(MainGameData, tempProperty.GetFinePrice());
            }

           
            CheckIfGameOver();
        }

        else { //Special tile

            SpecialTile specialTile = (SpecialTile)tempTile;
            int tempReward = specialTile.GetReward();
            MainGameData.players[(MainGameData.whosTurnIsIt)].IncreaseMoney(tempReward);

            ViewerHandler.UpdateLogWindow(MainGameData, tempReward);
        }


        MainGameData.IncreaseWhosTurnIsIt();

        ViewerHandler.UpdateHUD(MainGameData);
        ViewerHandler.ShowWindow(ViewerHandler.GAME_LOG_WINDOW);
    }


    void CheckIfGameOver() {
        if (MainGameData.players[MainGameData.whosTurnIsIt].CheckIfLostGame()) {// enters when player looses the game
            // Go Back to main menu and change text to player won

            ViewerHandler.GameOver(MainGameData);
        }

    }


    public void ResetGame() {
        
        ResetGameData();
        MainGameData.PlayersHUD = new GameObject[MainGameData.NUMBER_OF_PLAYERS];
        ViewerHandler.InitPlayersHUD(MainGameData);


        ViewerHandler.HideWindow(ViewerHandler.MAIN_MENU_WINDOW);

        MapHandlingAtStart();
        PlayersHandlingAtStart();

        ViewerHandler.UpdateHUD(MainGameData);

        ViewerHandler.ShowWindow(ViewerHandler.GAME_LOG_WINDOW);
    }

    void ResetGameData() {
        foreach (Player player in MainGameData.players) {
            Destroy(player.GetPlayerPieceGameObject());
        }

        foreach (Tile tile in MainGameData.gameTileMap) {
            tile.GetTilegameObject().GetComponent<SpriteRenderer>().sprite = ViewerHandler.BASE_NEUTRAL_TILE_SPRITE;
        }

        foreach (GameObject gameObject in MainGameData.PlayersHUD) {
            Destroy(gameObject);
        }

    }

    void MapHandlingAtStart() {
        MainGameData.gameTileMap = new Tile[ViewerHandler.TILE_MAP.Length];

        for (int i = 0; i < MainGameData.gameTileMap.Length; i++) {     
            //Always the starting point
            if (i == 0) {
                MainGameData.gameTileMap[i] = new SpecialTile(SpecialTile.TileType.StartingPoint, 200);
            }
            //Bonus tiles
            else if (i == 6 || i == 12 || i == 18) MainGameData.gameTileMap[i] = new SpecialTile(SpecialTile.TileType.Bonus, 50);
            //Property
            else {
                MainGameData.gameTileMap[i] = new Property(-1, 200, 20);
            }

            MainGameData.gameTileMap[i].SetTileGameObject(ViewerHandler.TILE_MAP[i]); //Setting the tile gameobject
            MainGameData.gameTileMap[i].SetTileIndex(i);
        }
    }

    void PlayersHandlingAtStart() {
        MainGameData.players = new Player[MainGameData.NUMBER_OF_PLAYERS];

        MainGameData.whosTurnIsIt = 0;

        for (int i = 0; i < MainGameData.players.Length; i++) {
            MainGameData.players[i] = new Player();
            MainGameData.players[i].SetPlayerIndex(i);
            MainGameData.players[i].SetPlayerPieceGameObject(ViewerHandler.PLAYERS_PIECES[i]); //setting player's piece
            MainGameData.players[i].SetCurrentPosition(0);
            MainGameData.players[i].SetMoney(MainGameData.STARTING_AMOUNT_OF_MONEY);
            MainGameData.players[i].SetMovesLeft(0);

            if (i == MainGameData.whosTurnIsIt) MainGameData.players[i].SetPlayerState(Player.PlayerState.RollingDice);
            else MainGameData.players[i].SetPlayerState(Player.PlayerState.Waiting);


            ViewerHandler.PlacePlayerAtPosition(MainGameData.players[i], 0);
        }

      
    }




}
