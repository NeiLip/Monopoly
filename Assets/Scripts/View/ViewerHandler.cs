using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewerHandler : MonoBehaviour
{
    public SoundHandler SoundHandler;

    [Header("Base Textures")]
    public Sprite[] DIE_SPRITES;
    public Sprite[] BASE_PLAYERS_TILES_SPRITES;
    public Sprite BASE_NEUTRAL_TILE_SPRITE;
    public GameObject[] PLAYERS_PIECES;
    public GameObject BasePlayerHUD;

    [Header("Gameobjects")]
    public GameObject HUD_Canvas;
    public GameObject DIE_BACKGROUND;
    public GameObject CURRENT_DIE;

    [Header("Tile Map")]
    public GameObject[] TILE_MAP;

    [Header("Windows")]
    public GameObject GAME_LOG_WINDOW;
    public GameObject MAIN_MENU_WINDOW;

    [Header("Texts")]
    public Text MainMenuTitle_Text;
    public Text MainMenuSubTitle_Text;
    public Text LogSum_Text; 
    public Text LogTitle_Text;
    public Text Die_Ready_To_Roll_Text;


    readonly Vector3[] playersOrientationOnTile = {new Vector3(.75f, .55f, 0), new Vector3(-.75f, -.15f, 0)};


    public void OnWakeUp() {
        MainMenuSubTitle_Text.text = "";
        ShowWindow(MAIN_MENU_WINDOW);
        HideWindow(GAME_LOG_WINDOW);
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

            if (i == 0) {//player 1
                MainGameData.PlayersHUD[0].transform.localPosition = new Vector3(-88.5f, 71.8f, 0f);


            }
            else if (i == 1) {//player2
                MainGameData.PlayersHUD[1].transform.localPosition = new Vector3(88.5f, 71.8f, 0f);
                MainGameData.PlayersHUD[1].transform.Find("PlayerName_Text").GetComponent<Text>().text = "PLayer 2";
                MainGameData.PlayersHUD[1].transform.Find("PlayerName_Text").GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                MainGameData.PlayersHUD[1].transform.Find("PlayerMoney_Text").GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                MainGameData.PlayersHUD[1].transform.Find("active_player_indication").gameObject.SetActive(false);
            }
        }

    }


    public void ShowWindow(GameObject window) {
        window.SetActive(true);
    }
    public void HideWindow(GameObject window) {
        window.SetActive(false);
    }

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

    public void UpdateHUD(GameData MainGameData) {
        for (int i = 0; i < MainGameData.NUMBER_OF_PLAYERS; i++) {
            if (i == MainGameData.whosTurnIsIt)
                MainGameData.PlayersHUD[i].transform.Find("active_player_indication").gameObject.SetActive(true);

            else
                MainGameData.PlayersHUD[i].transform.Find("active_player_indication").gameObject.SetActive(false);

           // string withCommas = string.Format("{0:n0}", MainGameData.players[i].GetMoney());
            MainGameData.PlayersHUD[i].transform.Find("PlayerMoney_Text").GetComponent<Text>().text = AddCommasToNumber(MainGameData.players[i].GetMoney()) + "$";
        }

        if (MainGameData.gameType == GameData.GameType.Upgrades) { //If it's a game with upgrades, means that taxes may change
            UpdateTilesTax(MainGameData);
        }
       
    }

    public void UpdateProperty(GameData MainGameData) {
        Tile temp = MainGameData.gameTileMap[MainGameData.players[MainGameData.whosTurnIsIt].GetCurrentPosition()];
        if (temp.GetType() == typeof(Property)) {
            Property currentProperty = (Property)temp;
            if (currentProperty.GetOwnedByPlayerIndex() == MainGameData.whosTurnIsIt) { // Makes sure the player owns the property
                currentProperty.GetTilegameObject().GetComponent<SpriteRenderer>().sprite = BASE_PLAYERS_TILES_SPRITES[MainGameData.whosTurnIsIt];
            }
        }
    }



    public enum LogType {
        Roll,
        BuyProperty,
        PayTax,
        ReceiveBonusMoney,
        NotEnoghtMoney,
        AlreadyBoughtIt,
        Upgrade
    }
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
            case LogType.PayTax:
                LogTitle_Text.text = "You landed on Player " + (((MainGameData.whosTurnIsIt + 1) % MainGameData.NUMBER_OF_PLAYERS) + 1) + "'s property";
                LogSum_Text.text = "Lose " + sum.ToString() + "$";
                break;
            case LogType.ReceiveBonusMoney:
                LogTitle_Text.text = "You landed on a Bonus tile";
                LogSum_Text.text = "Get " + sum.ToString() + "$";
                break;
            case LogType.NotEnoghtMoney:
                LogTitle_Text.text = "It costs " + sum +"$. You can not afford it" ;
                LogSum_Text.text = "Sorry";
                break;
            case LogType.AlreadyBoughtIt:
                LogTitle_Text.text = "This property is already yours!";
                LogSum_Text.text = "";
                break;
            case LogType.Upgrade:
                LogTitle_Text.text = "You can upgrade this property. Tax increased!";
                LogSum_Text.text = "Pay " + sum.ToString() + "$";
                break;
            default:
                break;
        }
    }


    public void InitTilesCosts(GameData MainGameData) {
        for (int i = 0; i < TILE_MAP.Length; i++) {
            if (TILE_MAP[i].transform.Find("TileTexts/Cost_Text") != null) {
                Property temp = (Property)MainGameData.gameTileMap[i];
                TILE_MAP[i].transform.Find("TileTexts/Cost_Text").GetComponent<Text>().text = temp.GetCostPrice() + "$";
            }
        }
        UpdateTilesTax(MainGameData);
    }

    public void UpdateTilesTax(GameData MainGameData) {
        for (int i = 0; i < TILE_MAP.Length; i++) {
            if (TILE_MAP[i].transform.Find("TileTexts/Tax_Text") != null) {
                Property temp = (Property)MainGameData.gameTileMap[i];
                TILE_MAP[i].transform.Find("TileTexts/Tax_Text").GetComponent<Text>().text = temp.GetTaxPrice() + "$";
            }
        }

    }

    /// <summary>
    /// Placing a player piece on a specific tile
    /// </summary>
    /// <param name="player">  Player to place </param>
    /// <param name="newPosition"> The new tile index </param>
    public void PlacePlayerAtPosition(Player player, int newPosition) {
        Vector3 tempNewPosition = TILE_MAP[newPosition].transform.localPosition + playersOrientationOnTile[player.GetPlayerIndex()];
        player.GetPlayerPieceGameObject().transform.localPosition = tempNewPosition;
    }

    /// <summary>
    /// Moving (animating) a player piece to a specific tile
    /// </summary>
    /// <param name="player">  Player to place </param>
    /// <param name="newPosition"> The new tile index </param>
    public void MovePlayerToPosition(GameHandler gameHandler,Player player, int newPosition) {
        Vector3 tempNewPosition = TILE_MAP[newPosition].transform.localPosition + playersOrientationOnTile[player.GetPlayerIndex()];
        StartCoroutine(MovePlayerOverSeconds(gameHandler, player.GetPlayerPieceGameObject(), tempNewPosition));

    }

    public void UpdateCurrentDie(int rolledNumber) {
        SoundHandler.PlayTick();
        CURRENT_DIE.GetComponent<SpriteRenderer>().sprite = DIE_SPRITES[rolledNumber - 1];
    }

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

        gameHandler.PlayTurn();
//        gameData.isBringCardToFrontCoroutineRun = false;
    }

    public void GameOver(GameData MainGameData) {
        int winningPlayer = ((MainGameData.whosTurnIsIt + 1) % MainGameData.NUMBER_OF_PLAYERS) + 1;
        MainMenuTitle_Text.text = "Player " + winningPlayer + " you WIN!";
        MainMenuSubTitle_Text.text =  AddCommasToNumber(MainGameData.players[(MainGameData.whosTurnIsIt + 1) % MainGameData.NUMBER_OF_PLAYERS].GetMoney())
            + "$ left"; //Gets winner's amount of money left and add commas
        ShowWindow(MAIN_MENU_WINDOW);
    }

    string AddCommasToNumber(int number) {
        return string.Format("{0:n0}", number);
    }

    public void RollDieAnimation(GameHandler gameHandler, int finalNummber) {
        StartCoroutine(ChangeDieFace(gameHandler, finalNummber));
    }

    private IEnumerator ChangeDieFace(GameHandler gameHandler, int finalNumber) {
        float finalDuration = Random.Range(0.8f, 1.2f);
        float currentDuration = Random.Range(0.05f, 0.2f);

        int preRolled = finalNumber;
        while (currentDuration < finalDuration) {
            int newRolled = Random.Range(1, 7);
            while (newRolled == preRolled || newRolled == finalNumber) {
                newRolled = Random.Range(1, 7);
            }
            UpdateCurrentDie(newRolled);
            preRolled = newRolled;
            yield return new WaitForSeconds(currentDuration);
            currentDuration *= gameHandler.MainGameData.DIE_ROLL_ANIMATION_SPEED;
        }
        UpdateCurrentDie(finalNumber);
        yield return new WaitForSeconds(0.5f);

        gameHandler.PlayTurn();
    }


}
