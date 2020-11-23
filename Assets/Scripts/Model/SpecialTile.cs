using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : Tile
{
    public enum TileType {
        Null,
        StartingPoint,
        Bonus
    }

    TileType tileType;
    int baseReward;

    //Default constructor
    public SpecialTile() {
        tileType = TileType.Null;
        baseReward = -1;
    }

    public SpecialTile(TileType _tileType, int _baseReward) {
        tileType = _tileType;
        baseReward = _baseReward;
    }

    //Setter tile type
    public void SetTileType(TileType type) {
        tileType = type;
    }
    //Getter tile type
    public TileType GetTileType() {
        return tileType;
    }

    //Calculate current reward and returns it
    public int GetReward() {
        if (tileType == TileType.StartingPoint) {// if starting point, return its base reward as is
            return baseReward;
        }
        else { //if special tile
            int tempRoll = Random.Range(0, 10); //Get a random number which represents probabilites

            switch (tempRoll) {
                case int n when (n <= 4): 
                    return baseReward;//50% chance to enter
                case 5: 
                    return (int)(baseReward * 1.2);//10% chance to enter
                case 6:
                    return (int)(baseReward * 1.4);//10% chance to enter
                case 7:
                    return (int)(baseReward * 0.8);//10% chance to enter
                case 8:
                    return (int)(baseReward * 0.6);//10% chance to enter
                case 9:
                    int secTemp = Random.Range(0, 2);
                    if (secTemp == 0) return (int)(baseReward * 1.8);//10% * 50% = 5% chance to enter
                    else return (int)(baseReward * 0.2);//10% * 50% = 5% chance to enter
                default:
                    return 0;
            }
        }
    }
}
