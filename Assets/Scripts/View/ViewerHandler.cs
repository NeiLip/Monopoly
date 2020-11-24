using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Gets data from game handler and shows relevant and up-to-date data
public class ViewerHandler : MonoBehaviour
{
    public SoundHandler SoundHandler;//sound handler reference

    [Header("Base Textures")]
    public Sprite[] DIE_SPRITES;//All faces sprites (6 in our die)
    public Sprite[] BASE_PLAYERS_TILES_SPRITES;//Currently contains red and blue tiles
    public Sprite BASE_NEUTRAL_TILE_SPRITE;//Gray tile
    public GameObject[] PLAYERS_PIECES;
    public GameObject BasePlayerHUD;

    [Header("Gameobjects")]
    public GameObject HUD_Canvas;
    public GameObject DIE_BACKGROUND;//empty die background.
    public GameObject CURRENT_DIE;//Current die face. Will set it to one of DIE_SPRITES[] elements
    public Toggle gameTypeToggle; //The toggle from main menu
    public ParticleSystem ON_WIN_PARTICLE_SYSTEM; //Particle system in main menu. Will be played on win

    [Header("Tile Map")]
    public GameObject[] TILE_MAP;//Our actual tile map.

    [Header("Windows")]
    public GameObject GAME_LOG_WINDOW;
    public GameObject MAIN_MENU_WINDOW;
    public GameObject HOW_TO_PLAY_WINDOW;

    [Header("Texts")]
    public Text MainMenuTitle_Text;
    public Text MainMenuSubTitle_Text;
    public Text LogSum_Text; 
    public Text LogTitle_Text;
    public Text Die_Ready_To_Roll_Text;


    readonly Vector3[] playersPositionWithinATile = {new Vector3(.75f, .55f, 0), new Vector3(-.75f, .2f, 0)};// the players' pieces position within a tile (so they won't cover each other)

    //Called from game handler only once on awake 
    public void OnWakeUp() {
        MainMenuSubTitle_Text.text = "";
        ShowWindow(MAIN_MENU_WINDOW);
        HideWindow(GAME_LOG_WINDOW);
        HideWindow(HOW_TO_PLAY_WINDOW);
        UpdateDieView(false);
    }

    /// <summary>
    /// Initiating players HUDs for two players
    /// </summary>
    public void InitPlayersHUD(GameData MainGameData) {
        for (int i = 0; i < MainGameData.NUMBER_OF_PLAYERS; i++) {
            MainGameData.PlayersHUD[i] = Instantiate(BasePlayerHUD);
            MainGameData.PlayersHUD[i].transform.SetParent(HUD_Canvas.transform);
            MainGameData.PlayersHUD[i].transform.localScale = new Vector3(1f, 1f, 1f);

            //player 1
            if (i == 0) {
                MainGameData.PlayersHUD[i].transform.localPosition = new Vector3(-88.5f, 71.8f, 0f);
            }
            //player2
            else if (i == 1) {//Mainly changes texts alignments
                MainGameData.PlayersHUD[i].transform.localPosition = new Vector3(88.5f, 71.8f, 0f);
                MainGameData.PlayersHUD[i].transform.Find("PlayerName_Text").GetComponent<Text>().text = "PLayer 2";
                MainGameData.PlayersHUD[i].transform.Find("PlayerName_Text").GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                MainGameData.PlayersHUD[i].transform.Find("PlayerMoney_Text").GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                MainGameData.PlayersHUD[i].transform.Find("active_player_indication").gameObject.SetActive(false);
            }
        }

    }

    //Called when a player losses the game
    public void GameOver(GameData MainGameData) {
        int winningPlayer = (MainGameData.nextPlayer) + 1;
        MainMenuTitle_Text.text = "Player " + winningPlayer + " you WIN!";
        MainMenuSubTitle_Text.text = AddCommasToNumber(MainGameData.players[MainGameData.nextPlayer].GetMoney())
            + "$ left"; //Gets winner's amount of money left and add commas
        ShowWindow(MAIN_MENU_WINDOW);
        ON_WIN_PARTICLE_SYSTEM.time = 0;
        ON_WIN_PARTICLE_SYSTEM.Play();
    }

    //Show specific window. Created for convenience
    public void ShowWindow(GameObject window) {
        window.SetActive(true);
    }
    //Hide specific window. Created for convenience
    public void HideWindow(GameObject window) {
        window.SetActive(false);
    }

    //Updated the die to show a number or the text "ready to roll!"
    public void UpdateDieView(bool showNumber) {
        if (showNumber) {
            CURRENT_DIE.SetActive(true);
            Die_Ready_To_Roll_Text.gameObject.SetActive(false);
        }
        else {
            CURRENT_DIE.SetActive(false);
            Die_Ready_To_Roll_Text.gameObject.SetActive(true);
        }
    }

    //Updates players HUD. Updates their amount of money and moves active_player_indication
    public void UpdateHUD(GameData MainGameData) {
        for (int i = 0; i < MainGameData.NUMBER_OF_PLAYERS; i++) {
            if (i == MainGameData.whosTurnIsIt)
                MainGameData.PlayersHUD[i].transform.Find("active_player_indication").gameObject.SetActive(true);
            else
                MainGameData.PlayersHUD[i].transform.Find("active_player_indication").gameObject.SetActive(false);

            MainGameData.PlayersHUD[i].transform.Find("PlayerMoney_Text").GetComponent<Text>().text = AddCommasToNumber(MainGameData.players[i].GetMoney()) + "$";
        }

        if (MainGameData.gameType == GameData.GameType.Upgrades) { //If it's a game with upgrades, means that fines may change
            UpdateTilesFines(MainGameData);
        }
    }

    //Updates property's sprite
    public void UpdatePropertySprite(GameData MainGameData) {
        Tile tempTile = MainGameData.gameTileMap[MainGameData.players[MainGameData.whosTurnIsIt].GetCurrentPosition()];
        if (tempTile.GetType() == typeof(Property)) {
            Property currentProperty = (Property)tempTile;
            if (currentProperty.GetOwnedByPlayerIndex() == MainGameData.whosTurnIsIt) { // Makes sure the player owns the property
                currentProperty.GetTilegameObject().GetComponent<SpriteRenderer>().sprite = BASE_PLAYERS_TILES_SPRITES[MainGameData.whosTurnIsIt];
            }
        }
    }

    //Initializes tiles prices
    public void InitTilesCosts(GameData MainGameData) {
        for (int i = 0; i < TILE_MAP.Length; i++) {
            if (TILE_MAP[i].transform.Find("TileTexts/Cost_Text") != null) {
                Property temp = (Property)MainGameData.gameTileMap[i];
                TILE_MAP[i].transform.Find("TileTexts/Cost_Text").GetComponent<Text>().text = temp.GetCostPrice() + "$";
            }
        }
        UpdateTilesFines(MainGameData);
    }
    //Initializes tiles fines.
    //In classic games- called only once at the beginning
    //Called only during games with upgrades.
    public void UpdateTilesFines(GameData MainGameData) {
        for (int i = 0; i < TILE_MAP.Length; i++) {
            if (TILE_MAP[i].transform.Find("TileTexts/Fine_Text") != null) {
                Property temp = (Property)MainGameData.gameTileMap[i];
                TILE_MAP[i].transform.Find("TileTexts/Fine_Text").GetComponent<Text>().text = temp.GetFinePrice() + "$";
            }
        }

    }

    /// Placing a player piece on a specific tile.
    /// Because during the game we animates the movement, this function is called only once at the beginning of the game.
    /// <param name="player">  Player to place </param>
    /// <param name="newPosition"> The new tile index </param>
    public void PlacePlayerAtPosition(Player player, int newPosition) {
        Vector3 tempNewPosition = TILE_MAP[newPosition].transform.localPosition + playersPositionWithinATile[player.GetPlayerIndex()];
        player.GetPlayerPieceGameObject().transform.localPosition = tempNewPosition;
    }

    /// Moving (animating) a player piece to a specific tile
    /// <param name="player">  Player to place </param>
    /// <param name="newPosition"> The new tile index </param>
    public void MovePlayerToPosition(GameHandler gameHandler,Player player, int newPosition) {
        Vector3 tempNewPosition = TILE_MAP[newPosition].transform.localPosition + playersPositionWithinATile[player.GetPlayerIndex()];
        StartCoroutine(MovePlayerOverSeconds(gameHandler, player.GetPlayerPieceGameObject(), tempNewPosition));

    }

    //Updates the die to show a specific value (in our die it can be 1-6)
    public void UpdateCurrentDie(int rolledNumber) {
        SoundHandler.PlayTick();
        CURRENT_DIE.GetComponent<SpriteRenderer>().sprite = DIE_SPRITES[rolledNumber - 1];
    }

    //Moves player's piece from one position to another over PLAYER_MOVEMENT_DURATION seconds
    private IEnumerator MovePlayerOverSeconds(GameHandler gameHandler, GameObject objectToMove, Vector3 endPosition) {
        float duration = gameHandler.MainGameData.PLAYER_MOVEMENT_DURATION;
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;

        while (elapsedTime < duration) {
            objectToMove.transform.position = Vector3.Lerp(startingPos, endPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = endPosition;

        gameHandler.ContinueTurn();//Finished moving, continue turn
    }

 
    //Add commas to numbers. For example: 2500 return 2,500
    string AddCommasToNumber(int number) {
        return string.Format("{0:n0}", number);
    }

    //Called every time the die is rolled.
    public void RollDieAnimation(GameHandler gameHandler, int finalNummber) {
        StartCoroutine(ChangeDieFace(gameHandler, finalNummber));
    }
    private IEnumerator ChangeDieFace(GameHandler gameHandler, int finalNumber) {
        float finalDuration = Random.Range(0.8f, 1.2f);//Use random so every die roll would feel different
        float currentDuration = Random.Range(0.05f, 0.2f);//Use random so every die roll would feel different

        int preRolled = finalNumber;
        while (currentDuration < finalDuration) {//When current duration goes beyond final duration, stop animation
            int newRolled = Random.Range(1, 7);
            while (newRolled == preRolled || newRolled == finalNumber) { //Just make sure the next random number is not the same as the previous. Otherwise it looks wierd..
                newRolled = Random.Range(1, 7);
            }
            UpdateCurrentDie(newRolled);

            preRolled = newRolled;
            yield return new WaitForSeconds(currentDuration);
            currentDuration *= gameHandler.MainGameData.DIE_ROLL_ANIMATION_SPEED;
        }

        UpdateCurrentDie(finalNumber);
        yield return new WaitForSeconds(0.5f);
        gameHandler.ContinueTurn();//Finished rolling dice, contine turn
    }

    //Called every time any of the players' money is changed
    public void ChangeAmountOfMoneyAnimation(GameHandler gameHandler, int moneyAtBeginOfTurn) {
        HideWindow(GAME_LOG_WINDOW);
        StartCoroutine(ChangeAmountOfMoneyAnimationCoroutine(gameHandler, moneyAtBeginOfTurn));
    }
    private IEnumerator ChangeAmountOfMoneyAnimationCoroutine(GameHandler gameHandler, int moneyAtBeginOfTurn) {
        int finalAmount = gameHandler.MainGameData.players[gameHandler.MainGameData.whosTurnIsIt].GetMoney();//Get player's current amount of money (i.e after the player paid/received money)
        Text currentText = gameHandler.MainGameData.PlayersHUD[gameHandler.MainGameData.whosTurnIsIt].transform.Find("PlayerMoney_Text").GetComponent<Text>(); //Current player's PlayerMoney_Text

        float duration = gameHandler.MainGameData.MONEY_ANIMATION_SPEED;
        float elapsedTime = 0;


        if (!gameHandler.MainGameData.isCurrentPlayerCouldNotBuyProperty) {//If player did not buy anything, skip
            while (elapsedTime < duration) {
                SoundHandler.PlaySnatch();
                currentText.text = AddCommasToNumber((int)Mathf.Lerp(moneyAtBeginOfTurn, finalAmount, (elapsedTime / duration))) + "$";
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        UpdateAfterMoneyAnimation(gameHandler, gameHandler.MainGameData);
    }

    //Updates relevant windows according money changes.
    void UpdateAfterMoneyAnimation(GameHandler gameHandler,GameData MainGameData) {
        gameHandler.CheckIfGameOver();//Every time a turn is finished, we check if the player lost (i.e have 0 or less money)

        UpdatePropertySprite(MainGameData);
        UpdateDieView(false);

        MainGameData.state = GameData.State.RollDie;
        MainGameData.IncreaseWhosTurnIsIt();

        UpdateLogWindow(MainGameData, -1, LogType.Roll); //Updates to "Roll the die" title
        UpdateHUD(MainGameData);
        ShowWindow(GAME_LOG_WINDOW);
    }


    //Different log types
    public enum LogType {
        Roll,
        BuyProperty,
        PayFine,
        ReceiveBonusMoney,
        NotEnoghtMoney,
        AlreadyBoughtIt,
        Upgrade
    }
    //Updates the log window
    public void UpdateLogWindow(GameData MainGameData, int sum, LogType type) {
        switch (type) {
            case LogType.Roll:
                LogTitle_Text.text = "Player " + (MainGameData.whosTurnIsIt + 1) + ", it's your time to roll!";
                LogSum_Text.text = "Roll the die";
                break;
            case LogType.BuyProperty:
                LogTitle_Text.text = "You landed on a free property";
                LogSum_Text.text = "Pay " + sum.ToString() + "$";
                break;
            case LogType.PayFine:
                LogTitle_Text.text = "You landed on Player " + (MainGameData.nextPlayer + 1) + "'s property";
                LogSum_Text.text = "Lose " + sum.ToString() + "$";
                break;
            case LogType.ReceiveBonusMoney:
                LogTitle_Text.text = "You landed on a Bonus tile";
                LogSum_Text.text = "Get " + sum.ToString() + "$";
                break;
            case LogType.NotEnoghtMoney:
                LogTitle_Text.text = "It costs " + sum + "$. You can not afford it";
                LogSum_Text.text = "Sorry";
                break;
            case LogType.AlreadyBoughtIt:
                LogTitle_Text.text = "This property is already yours!";
                LogSum_Text.text = "";
                break;
            case LogType.Upgrade:
                LogTitle_Text.text = "Available for upgrade. Higher fines!";
                LogSum_Text.text = "Pay " + sum.ToString() + "$";
                break;
            default:
                break;
        }
    }
}
