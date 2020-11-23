using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{

    public ViewerHandler ViewerHandler;
    
    [HideInInspector]
    public GameData MainGameData;

    private int MoneyAtStartOfTurn = 0; //always be 2 even if we have more than 2 players beacuse it is used only to transfer money from one player to other

    void Awake() {
        MainGameData = new GameData();

        ViewerHandler.OnWakeUp();

    }


 
    public void OnLogWindowButtonClicked() {
        if (MainGameData.state == GameData.State.RollDie) {
            ViewerHandler.UpdateDieView(true);
            RollTheDie();
            MainGameData.state = GameData.State.Moving;
        }

        else { //Playing
            ViewerHandler.ChangeAmountOfMoneyAnimation(this, MoneyAtStartOfTurn);
        }
    }


    public void RollTheDie() {
        ViewerHandler.HideWindow(ViewerHandler.GAME_LOG_WINDOW);

        int tempRoll = Random.Range(1, 7);
        //ViewerHandler.UpdateCurrentDie(tempRoll);

        ViewerHandler.RollDieAnimation(this, tempRoll);

        MainGameData.players[MainGameData.whosTurnIsIt].SetMovesLeft(tempRoll);

       // PlayTurn();

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
        MoneyAtStartOfTurn = MainGameData.players[MainGameData.whosTurnIsIt].GetMoney();

        if (tempTile.GetType() == typeof(Property)) {
            Property currentProperty = (Property)tempTile;

            if (currentProperty.GetOwnedByPlayerIndex() == MainGameData.whosTurnIsIt) { // This property is already yours

                if (MainGameData.gameType == GameData.GameType.Upgrades) {//if Game is with upgrades
                    int upgradeCost = (int)(currentProperty.GetCostPrice() * MainGameData.UPGRADE_COST_RATIO);
                    if (MainGameData.players[MainGameData.whosTurnIsIt].GetMoney() - upgradeCost > 0) {
                        MainGameData.players[MainGameData.whosTurnIsIt].DecreaseMoney(upgradeCost);
                        UpgradeProperty(currentProperty);
                        ViewerHandler.UpdateLogWindow(MainGameData, upgradeCost, ViewerHandler.LogType.Upgrade);
                    }
                    else {
                         ViewerHandler.UpdateLogWindow(MainGameData, -1, ViewerHandler.LogType.AlreadyBoughtIt);
                    }
                }

                else {//classic game
                    ViewerHandler.UpdateLogWindow(MainGameData, -1, ViewerHandler.LogType.AlreadyBoughtIt);
                }

              
               
            }
            else if (currentProperty.GetOwnedByPlayerIndex() == -1) { //This property is free
                //make sure the player has enough money to buy the property
                if (MainGameData.players[MainGameData.whosTurnIsIt].GetMoney() - currentProperty.GetCostPrice() > 0) { //that means the player can afford buying the property
                   // ViewerHandler.ChangeAmountOfMoneyAnimation(this, MainGameData.players[MainGameData.whosTurnIsIt].GetMoney() - currentProperty.GetCostPrice());
                    MainGameData.players[MainGameData.whosTurnIsIt].DecreaseMoney(currentProperty.GetCostPrice());
                  //  currentProperty.GetTilegameObject().GetComponent<SpriteRenderer>().sprite = ViewerHandler.BASE_PLAYERS_TILES_SPRITES[MainGameData.whosTurnIsIt];
                    currentProperty.SetOwnedByPlayerIndex(MainGameData.whosTurnIsIt);

                    ViewerHandler.UpdateLogWindow(MainGameData, currentProperty.GetCostPrice(), ViewerHandler.LogType.BuyProperty);
                }
                else {
                    ViewerHandler.UpdateLogWindow(MainGameData, currentProperty.GetCostPrice(), ViewerHandler.LogType.NotEnoghtMoney);
                }

          
            }
            else { //Other player already purchased this property
                MainGameData.players[(MainGameData.whosTurnIsIt)].DecreaseMoney(currentProperty.GetTaxPrice());//Takes money from the player and gives it to the player owning this property
                MainGameData.players[MainGameData.nextPlayer].IncreaseMoney(currentProperty.GetTaxPrice());//Takes money from the player and gives it to the player owning this property

                ViewerHandler.UpdateLogWindow(MainGameData, currentProperty.GetTaxPrice(), ViewerHandler.LogType.PayTax);
            }

            CheckIfGameOver();
        }

        else { //Special tile

            SpecialTile specialTile = (SpecialTile)tempTile;
            int tempReward = specialTile.GetReward();
            MainGameData.players[(MainGameData.whosTurnIsIt)].IncreaseMoney(tempReward);

            ViewerHandler.UpdateLogWindow(MainGameData, tempReward, ViewerHandler.LogType.ReceiveBonusMoney);
        }

       // ViewerHandler.UpdateHUD(MainGameData);
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

        //Check if classic game or game with upgrades. If enabled than it is a game with upgrades
        if (ViewerHandler.gameTypeToggle.isOn) MainGameData.gameType = GameData.GameType.Upgrades;
        else MainGameData.gameType = GameData.GameType.Classic;

        MainGameData.PlayersHUD = new GameObject[MainGameData.NUMBER_OF_PLAYERS];
        ViewerHandler.InitPlayersHUD(MainGameData);


        ViewerHandler.HideWindow(ViewerHandler.MAIN_MENU_WINDOW);

        MapBuildingAtStart();
        PlayersHandlingAtStart();

        ViewerHandler.UpdateHUD(MainGameData);
        ViewerHandler.UpdateLogWindow(MainGameData, -1, ViewerHandler.LogType.Roll);

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
        MainGameData.state = GameData.State.RollDie;

    }

    void MapBuildingAtStart() {
        MainGameData.gameTileMap = new Tile[ViewerHandler.TILE_MAP.Length];

        int preMadePropertiesIndex = 0;
        for (int i = 0; i < MainGameData.gameTileMap.Length; i++) {
            //Always the starting point
            if (i == 0) {
                MainGameData.gameTileMap[i] = new SpecialTile(SpecialTile.TileType.StartingPoint, MainGameData.STARTING_POINT_REWARD_VALUE);
            }
            //Bonus tiles
            else if (i == 6 || i == 12 || i == 18) MainGameData.gameTileMap[i] = new SpecialTile(SpecialTile.TileType.Bonus, MainGameData.BASE_REWARD);
            //Property
            else {
                //MainGameData.gameTileMap[i] = new Property();
                MainGameData.gameTileMap[i] = MainGameData.PRE_MADE_PROPERTIES[preMadePropertiesIndex];
                preMadePropertiesIndex++;
            }

            MainGameData.gameTileMap[i].SetTileGameObject(ViewerHandler.TILE_MAP[i]); //Setting the tile gameobject
            MainGameData.gameTileMap[i].SetTileIndex(i);
        }

        ViewerHandler.InitTilesCosts(MainGameData);
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

    void UpgradeProperty(Property property) {
        property.SetTaxPrice((int)(property.GetTaxPrice() * MainGameData.TAX_AFTER_UPGRADE_RATIO));
    }


}
