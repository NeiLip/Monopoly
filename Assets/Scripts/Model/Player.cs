using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState {
        Null,
        RollingDice,
        Playing,
        Waiting
    }

    GameObject playerPieceGameObject;
    int playerIndex;
    int currentPosition;
    int money;

    Property[] propertiesBought; //can be use in the future for expansions, we don't really need it so I won't use it

    int movesLeft;
    PlayerState playerState;


    public Player() {
        playerIndex = -1;
        playerPieceGameObject = null;
        currentPosition = -1;
        money = -1;
        movesLeft = 0;
        playerState = PlayerState.Null;
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
    public void SetPlayerPieceGameObjectn(GameObject gameObject) {
        playerPieceGameObject = Instantiate(gameObject); //gameObject;
    }
    //Gets piece game object
    public GameObject GetPlayerPieceGameObjectn() {
        return playerPieceGameObject;
    }

    //Sets current position
    public void SetCurrentPosition(int newPosition) {
        currentPosition = newPosition;
    }
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

    public void IncreaseMoney(int sum) {
        money += sum;
    }
    public void DecreaseMoney(int sum) {
        money -= sum;
    }


    /// Check and return if the player lost the game
    /// <returns> TRUE if the player lost his game</returns>
    public bool CheckIfLostGame() {
        return (money <= 0);
    }

    //Sets player's game state
    public void SetPlayerState(PlayerState state) {
        playerState = state;
    }
    //Gets player's game state
    public PlayerState GetPlayerState() {
        return playerState;
    }
}
