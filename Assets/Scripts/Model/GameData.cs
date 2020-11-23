using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public readonly int NUMBER_OF_PLAYERS = 2;
    public readonly int STARTING_AMOUNT_OF_MONEY = 1500;
    public readonly int PROPERTIES_PRICE_AVERAGE = 300;
    public readonly int BASE_REWARD = 50;
    public readonly int STARTING_POINT_REWARD_VALUE = 200;

    public readonly float PLAYER_MOVEMENT_DURATION = .2f; //Time in seconds to move from tile to tile
    public readonly float DIE_ROLL_ANIMATION_SPEED = 2f; //uses as multiplication factor

    public readonly float MONEY_ANIMATION_SPEED = .3f;//Total time of the animation


    public readonly float TAX_COST_RATIO = .1f;

    public readonly float TAX_AFTER_UPGRADE_RATIO = 1.6f; //The new tax
    public readonly float UPGRADE_COST_RATIO = .06f; //The cost for upgrading a property. 


    public Player[] players;
    
    public int whosTurnIsIt = 0;
    public int nextPlayer {
        get {
            return ((whosTurnIsIt + 1) % NUMBER_OF_PLAYERS);
        }
    }

    public Tile[] gameTileMap;

    public GameObject[] PlayersHUD;

    public State state;
    public GameType gameType = GameType.Upgrades; //Determine the game type (Current options are Classic and Upgrades)

    public Property[] PRE_MADE_PROPERTIES;


    //Default Setter
    public GameData() {
        players = new Player[0];
        gameTileMap = new Tile[0];
        whosTurnIsIt = 0;
        PlayersHUD = new GameObject[0];
        state = State.RollDie;

        InsertPricesToProperties();
    }

    public void IncreaseWhosTurnIsIt() {
        whosTurnIsIt++;

        if (whosTurnIsIt == NUMBER_OF_PLAYERS) whosTurnIsIt = 0; //If last player in array, next player is at 0
    }


    void InsertPricesToProperties() {
        PRE_MADE_PROPERTIES = new Property[20];

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
