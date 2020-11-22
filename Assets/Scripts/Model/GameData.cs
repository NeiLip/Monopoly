using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public enum State {
        RollDie,
        Moving
    }


    public readonly int NUMBER_OF_PLAYERS = 2;
    public readonly int STARTING_AMOUNT_OF_MONEY = 1500;
    public readonly int PROPERTIES_PRICE_AVERAGE = 250;
    public readonly float DIE_ROLL_ANIMATION_SPEED = 1.3f; //uses as multiplication factor



    public Player[] players;
    
    public int whosTurnIsIt;

    public Tile[] gameTileMap;

    public GameObject[] PlayersHUD;

    public State state;

    public Property[] PRE_MADE_PROPERTIES;


    //Default Setter
    public GameData() {
        players = new Player[0];
        gameTileMap = new Tile[0];
        whosTurnIsIt = 0;
        PlayersHUD = new GameObject[0];
        state = State.RollDie;

        CreatePreMadeProperties();
    }

    public void IncreaseWhosTurnIsIt() {
        whosTurnIsIt++;

        if (whosTurnIsIt == NUMBER_OF_PLAYERS) whosTurnIsIt = 0; //If we are last player in array, next player is at 0
    }


    void CreatePreMadeProperties() {
        PRE_MADE_PROPERTIES = new Property[20];

        int minPrice = (int)(PROPERTIES_PRICE_AVERAGE * 0.5);
        int maxPrice = (int)(PROPERTIES_PRICE_AVERAGE * 1.5);
        int sub = maxPrice - minPrice;

        int increment = (int)(sub / PRE_MADE_PROPERTIES.Length);

        int currentPrice = minPrice;
        
        for (int i = 0; i < PRE_MADE_PROPERTIES.Length; i++) {
            PRE_MADE_PROPERTIES[i] = new Property(-1, currentPrice, (int)(currentPrice * 0.1));
            currentPrice += increment;
        }
    }


}
