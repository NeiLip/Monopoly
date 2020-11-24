using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Links between the data from GameData to the viewer handler. Most calculations are made within this script
public class GameHandler : MonoBehaviour
{
    public ViewerHandler ViewerHandler; //Viewer Reference
    [HideInInspector]
    public GameData MainGameData;// Game data reference

    
    //Called only once at awake
    void Awake() {
        MainGameData = new GameData();
        ViewerHandler.OnWakeUp();
    }

    //Called when log menu button is clicked 
    public void OnLogWindowButtonClicked() {
        if (MainGameData.state == GameData.State.RollDie) {// if current game state is RollDie, it means we need to roll the die
            ViewerHandler.UpdateDieView(true);
            RollTheDie();
            MainGameData.state = GameData.State.Moving;
        }
        else { //Means we finished moving
            ViewerHandler.ChangeAmountOfMoneyAnimation(this, MainGameData.MoneyAtStartOfTurn);
        }
    }

    //Roll a random number from 1 to 6 (inclusive), call RollDieAnimation to animate it, and setting it as player's moves left
    public void RollTheDie() {
        ViewerHandler.HideWindow(ViewerHandler.GAME_LOG_WINDOW);
        int tempRoll = Random.Range(1, 7);
      
        ViewerHandler.RollDieAnimation(this, tempRoll);
        MainGameData.players[MainGameData.whosTurnIsIt].SetMovesLeft(tempRoll);
    }


    //Checks if the current player have moves left or he reached destination
    public void ContinueTurn() {
        if(MainGameData.players[MainGameData.whosTurnIsIt].GetMovesLeft() <= 0) {//reached destination
            ReachedFinalTile();
            return;
        }
        else {//still have moves left
            MainGameData.players[MainGameData.whosTurnIsIt].SetCurrentPosition((MainGameData.players[MainGameData.whosTurnIsIt].GetCurrentPosition() + 1) % MainGameData.gameTileMap.Length);

            ViewerHandler.MovePlayerToPosition(this, MainGameData.players[MainGameData.whosTurnIsIt], MainGameData.players[MainGameData.whosTurnIsIt].GetCurrentPosition());
            MainGameData.players[MainGameData.whosTurnIsIt].MoveOneTile();
        }
    }

    /// IMPORTANT FUNCTION
    /// Called when the player finished moving.
    /// We check what is the type of the destination tile and act accordingly.
    /// In addition, if we reached a property tile, we also need to check who owns it and act accordingly
    void ReachedFinalTile() {
        MainGameData.isCurrentPlayerCouldNotBuyProperty = false;//resets the variable

        Tile tempTile = MainGameData.gameTileMap[MainGameData.players[MainGameData.whosTurnIsIt].GetCurrentPosition()];
        MainGameData.MoneyAtStartOfTurn = MainGameData.players[MainGameData.whosTurnIsIt].GetMoney();
        if (tempTile.GetType() == typeof(Property)) {
            Property currentProperty = (Property)tempTile;
            if (currentProperty.GetOwnedByPlayerIndex() == MainGameData.whosTurnIsIt) { // This property is already yours
                //Enters only in games with upgrades
                if (MainGameData.gameType == GameData.GameType.Upgrades) {
                    int upgradeCost = (int)(currentProperty.GetCostPrice() * MainGameData.UPGRADE_COST_RATIO);
                    if (MainGameData.players[MainGameData.whosTurnIsIt].GetMoney() - upgradeCost > 0) {//Player has enough money to upgrade the property
                        MainGameData.players[MainGameData.whosTurnIsIt].DecreaseMoney(upgradeCost);
                        UpgradeProperty(currentProperty);
                        ViewerHandler.UpdateLogWindow(MainGameData, upgradeCost, ViewerHandler.LogType.Upgrade);
                    }
                    else {//Player did not have enough money to buy the property
                        MainGameData.isCurrentPlayerCouldNotBuyProperty = true;
                        ViewerHandler.UpdateLogWindow(MainGameData, -1, ViewerHandler.LogType.AlreadyBoughtIt);
                    }
                }
                //Enters only in classic games
                else {
                    MainGameData.isCurrentPlayerCouldNotBuyProperty = true;
                    ViewerHandler.UpdateLogWindow(MainGameData, -1, ViewerHandler.LogType.AlreadyBoughtIt);
                }
            }
            else if (currentProperty.GetOwnedByPlayerIndex() == -1) { //This property is free
                //make sure the player has enough money to buy the property
                if (MainGameData.players[MainGameData.whosTurnIsIt].GetMoney() - currentProperty.GetCostPrice() > 0) { //That means the player can afford buying the property
                    MainGameData.players[MainGameData.whosTurnIsIt].DecreaseMoney(currentProperty.GetCostPrice());//Takes money from the player
                    currentProperty.SetOwnedByPlayerIndex(MainGameData.whosTurnIsIt);//Make the current property be owned by current player
                    ViewerHandler.UpdateLogWindow(MainGameData, currentProperty.GetCostPrice(), ViewerHandler.LogType.BuyProperty);
                }
                else {//Means the player don't have enought money to buy the property
                    MainGameData.isCurrentPlayerCouldNotBuyProperty = true;
                    ViewerHandler.UpdateLogWindow(MainGameData, currentProperty.GetCostPrice(), ViewerHandler.LogType.NotEnoghtMoney);
                }
            }
            else { //Other player already purchased this property
                MainGameData.players[(MainGameData.whosTurnIsIt)].DecreaseMoney(currentProperty.GetFinePrice());//Takes money from the player and gives it to the player owning this property
                MainGameData.players[currentProperty.GetOwnedByPlayerIndex()].IncreaseMoney(currentProperty.GetFinePrice());//Takes money from the player and gives it to the player owning this property

                ViewerHandler.UpdateLogWindow(MainGameData, currentProperty.GetFinePrice(), ViewerHandler.LogType.PayFine);
            }
        }

        else { //Special tile - Can only earn money
            SpecialTile specialTile = (SpecialTile)tempTile;

            int tempReward = specialTile.GetReward();//Every time we call GetReward() we receive differnet amount, so we keep it for this turn

            MainGameData.players[(MainGameData.whosTurnIsIt)].IncreaseMoney(tempReward);
            ViewerHandler.UpdateLogWindow(MainGameData, tempReward, ViewerHandler.LogType.ReceiveBonusMoney);
        }
       // ViewerHandler.UpdateHUD(MainGameData);
        ViewerHandler.ShowWindow(ViewerHandler.GAME_LOG_WINDOW);
    }

    public void CheckIfGameOver() {
        if (MainGameData.players[MainGameData.whosTurnIsIt].CheckIfLostGame()) {// enters when player looses the game
            ViewerHandler.GameOver(MainGameData);
        }

    }

    //Called from main menu when starting a new game
    //Initialize all relevant data and destroys all current gameobjects
    public void ResetGame() {
        ResetGameData();
        //Check if classic game or game with upgrades. If enabled than it is a game with upgrades
        if (ViewerHandler.gameTypeToggle.isOn) MainGameData.gameType = GameData.GameType.Upgrades;
        else MainGameData.gameType = GameData.GameType.Classic;

        MainGameData.PlayersHUD = new GameObject[MainGameData.NUMBER_OF_PLAYERS];
        
        MapBuildingAtStart();
        PlayersHandlingAtStart();
        ViewerHandler.ResetGameViewer(MainGameData);
    }

    //Destroys previous game pieces and resets tiles 
    void ResetGameData() {
        foreach (Player player in MainGameData.players) { //Destroys players pieces gameobjects
            Destroy(player.GetPlayerPieceGameObject());
        }
        foreach (Tile tile in MainGameData.gameTileMap) { //Resets all game tiles to be the neutral gray tile
            tile.GetTilegameObject().GetComponent<SpriteRenderer>().sprite = ViewerHandler.BASE_NEUTRAL_TILE_SPRITE;
        }
        foreach (GameObject gameObject in MainGameData.PlayersHUD) { //Destroys players HUDs gameobjects
            Destroy(gameObject);
        }

        MainGameData.state = GameData.State.RollDie;
    }

    //Called evry time a new game begins. Initializes game data tile map. At the end, Updates game view
    void MapBuildingAtStart() {
        MainGameData.gameTileMap = new Tile[ViewerHandler.TILE_MAP.Length];
        InsertPricesToPremadeProperties();

        int preMadePropertiesIndex = 0;
        for (int i = 0; i < MainGameData.gameTileMap.Length; i++) {
            //Always the starting point
            if (i == 0) {
                MainGameData.gameTileMap[i] = new SpecialTile(SpecialTile.TileType.StartingPoint, MainGameData.STARTING_POINT_REWARD_VALUE);
            }
            //Bonus tiles
            else if (MainGameData.SPECIAL_TILES_INDEXES.Contains(i)) MainGameData.gameTileMap[i] = new SpecialTile(SpecialTile.TileType.Bonus, MainGameData.BASE_REWARD);
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


    //Insert property price and fine price for eact property
     void InsertPricesToPremadeProperties() {
        MainGameData.PRE_MADE_PROPERTIES = new Property[20];//we know the actual map and each game the map stays the same. So I allowed myself use an actual number

        int minPrice = (int)(MainGameData.PROPERTIES_PRICE_AVERAGE * 0.5);
        int maxPrice = (int)(MainGameData.PROPERTIES_PRICE_AVERAGE * 1.5);
        int sub = maxPrice - minPrice;

        int increment = (int)(sub / MainGameData.PRE_MADE_PROPERTIES.Length);

        int currentPrice = minPrice;

        for (int i = 0; i < MainGameData.PRE_MADE_PROPERTIES.Length; i++) {
            MainGameData.PRE_MADE_PROPERTIES[i] = new Property(-1, currentPrice, (int)(currentPrice * MainGameData.FINE_COST_RATIO));
            currentPrice += increment;
        }
    }

    //Called evry time a new game begins. Initializes players details
    void PlayersHandlingAtStart() {
        MainGameData.players = new Player[MainGameData.NUMBER_OF_PLAYERS];
        MainGameData.whosTurnIsIt = 0;
        for (int i = 0; i < MainGameData.NUMBER_OF_PLAYERS; i++) {
            MainGameData.players[i] = new Player();
            MainGameData.players[i].SetPlayerIndex(i); 
            MainGameData.players[i].SetPlayerPieceGameObject(Instantiate(ViewerHandler.PLAYERS_PIECES[i])); //setting player's piece
            MainGameData.players[i].SetCurrentPosition(0);
            MainGameData.players[i].SetMoney(MainGameData.STARTING_AMOUNT_OF_MONEY);
            MainGameData.players[i].SetMovesLeft(0);

            ViewerHandler.PlacePlayerAtPosition(MainGameData.players[i], 0);
        }
    }

    //Called when a player is upgrading his property
    void UpgradeProperty(Property property) {
        property.SetFinePrice((int)(property.GetFinePrice() * MainGameData.FINE_AFTER_UPGRADE_RATIO));
    }

    //Toggles menu-window/how-to-play-window accordingly
    public void ToggleMenuAndHowToPlayWindows(bool openHowToPlay) {
        if (openHowToPlay) {
            ViewerHandler.HideWindow(ViewerHandler.MAIN_MENU_WINDOW);
            ViewerHandler.ShowWindow(ViewerHandler.HOW_TO_PLAY_WINDOW);
        }
        else {
            ViewerHandler.HideWindow(ViewerHandler.HOW_TO_PLAY_WINDOW);
            ViewerHandler.ShowWindow(ViewerHandler.MAIN_MENU_WINDOW);
        }
    }
}
