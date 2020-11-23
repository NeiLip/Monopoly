using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Contains all relevant data
public class GameData
{
    public enum State {
        RollDie,
        Moving
    }
    public enum GameType {
        Classic,
        Upgrades
    }

    //Map game info
    public readonly int NUMBER_OF_PLAYERS = 2;

    //Money management
    public readonly int STARTING_AMOUNT_OF_MONEY = 1500;
    public readonly int PROPERTIES_PRICE_AVERAGE = 300; //Property price average. At range of PROPERTIES_PRICE_AVERAGE*0.5 to PROPERTIES_PRICE_AVERAGE
    public readonly int BASE_REWARD = 50;
    public readonly int STARTING_POINT_REWARD_VALUE = 200;//How much get from landing on starting point

    public readonly float TAX_COST_RATIO = .1f; //Base tax = TAX_COST_RATIO * property_price
    public readonly float TAX_AFTER_UPGRADE_RATIO = 1.6f; //The new tax = TAX_AFTER_UPGRADE_RATIO * current_tax
    public readonly float UPGRADE_COST_RATIO = .06f; //The cost for upgrading a property = UPGRADE_COST_RATIO * property_price

    //Animations info
    public readonly float PLAYER_MOVEMENT_DURATION = .3f; //Time in seconds to move from tile to tile
    public readonly float DIE_ROLL_ANIMATION_SPEED = 1.3f; //uses as multiplication factor
    public readonly float MONEY_ANIMATION_SPEED = .3f;//Total time of the animation


    //Players
    public Player[] players;    
    public int whosTurnIsIt = 0;
    public int nextPlayer {//Gets next player
        get {
            return ((whosTurnIsIt + 1) % NUMBER_OF_PLAYERS);
        }
    }
    public GameObject[] PlayersHUD;//Players hud's showing whos playing and how much money each player has
    public int MoneyAtStartOfTurn = 0; //current player's money amount at begin turn. Uses for animating money changes


    //Map and Tiles
    public Tile[] gameTileMap;//All current game tiles
    public Property[] PRE_MADE_PROPERTIES; //Being constructed at begin. We are  not changing it.

    //Game
    public State state;//Current game state
    public GameType gameType = GameType.Upgrades; //Determine the game type (Current options are Classic and Upgrades)


    //Default constructor
    public GameData() {
        players = new Player[0];
        gameTileMap = new Tile[0];
        whosTurnIsIt = 0;
        PlayersHUD = new GameObject[0];
        state = State.RollDie;

        InsertPricesToProperties();
    }

    //Change current player. If we are last player, next playr is at 0
    public void IncreaseWhosTurnIsIt() {
        whosTurnIsIt++;
        if (whosTurnIsIt == NUMBER_OF_PLAYERS) whosTurnIsIt = 0; //If last player in array, next player is at 0
    }


    //Insert property price and tax price for eact property
    void InsertPricesToProperties() {
        PRE_MADE_PROPERTIES = new Property[20];//we know the actual map and each game the map stays the same. So I allowed myself use an actual number

        int minPrice = (int)(PROPERTIES_PRICE_AVERAGE * 0.5);
        int maxPrice = (int)(PROPERTIES_PRICE_AVERAGE * 1.5);
        int sub = maxPrice - minPrice;

        int increment = (int)(sub / PRE_MADE_PROPERTIES.Length);

        int currentPrice = minPrice;
        
        for (int i = 0; i < PRE_MADE_PROPERTIES.Length; i++) {
            PRE_MADE_PROPERTIES[i] = new Property(-1, currentPrice, (int)(currentPrice * TAX_COST_RATIO));
            currentPrice += increment;
        }
    }
}
