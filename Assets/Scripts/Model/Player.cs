using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject playerPieceGameObject;//piece gameobject
    int playerIndex;
    int currentPosition;//tile index
    int money;

    Property[] propertiesBought; //can be use in the future for expansions, we don't really need it so I'm not using it.

    int movesLeft;//How many moves left for this turn. Will be 0 at the end of each turn, and will receive the die number at the begin of each turn

    //Default constructor
    public Player() {
        playerIndex = -1;
        playerPieceGameObject = null;
        currentPosition = -1;
        money = -1;
        movesLeft = 0;
    }


    //Sets player index
    public void SetPlayerIndex(int newIndex) {
        playerIndex = newIndex;
    }
    //Gets player index
    public int GetPlayerIndex() {
        return playerIndex;
    }

    //Sets piece game object
    public void SetPlayerPieceGameObject(GameObject gameObject) {
        playerPieceGameObject = Instantiate(gameObject); //gameObject;
    }
    //Gets piece game object
    public GameObject GetPlayerPieceGameObject() {
        return playerPieceGameObject;
    }

    //Sets current position
    public void SetCurrentPosition(int newPosition) {
        currentPosition = newPosition;
    }
    //Gets current position
    public int GetCurrentPosition() {
        return currentPosition;
    }

    //Sets number of moves left
    public void SetMovesLeft(int _movesLeft) {
        movesLeft = _movesLeft;
    }
    //Gets number of moves left
    public int GetMovesLeft() {
        return movesLeft;
    }
    //Gets number of moves left
    public void MoveOneTile() {
        movesLeft--;
        if (movesLeft < 0) movesLeft = 0;
    }

    //Sets amount money
    public void SetMoney(int _money) {
        money = _money;
    }
    //Gets amount of money
    public int GetMoney() {
        return money;
    }
    //Adds money to player
    public void IncreaseMoney(int sum) {
        money += sum;
    }
    //Takes money from player
    public void DecreaseMoney(int sum) {
        money -= sum;
    }


    /// Check and return if the player ran out of money.
    /// <returns> TRUE if the player lost his game </returns>
    public bool CheckIfLostGame() {
        return (money <= 0); 
    }
}
