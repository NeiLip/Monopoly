﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{

    public readonly int NUMBER_OF_PLAYERS = 2;
    public readonly int STARTING_AMOUNT_OF_MONEY = 200;

   // [HideInInspector]
    public Player[] players;
    //[HideInInspector]
    public int whosTurnIsIt;

    public Tile[] gameTileMap;

    public GameObject[] PlayersHUD;

    public GameData() {

    }

    public void IncreaseWhosTurnIsIt() {
        whosTurnIsIt++;

        if (whosTurnIsIt == NUMBER_OF_PLAYERS) whosTurnIsIt = 0; //If we are last player in array, next player is at 0
    }

}
