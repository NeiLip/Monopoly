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


    public SpecialTile() {
        tileType = TileType.Null;
        baseReward = -1;
    }

    public SpecialTile(TileType _tileType, int _baseReward) {
        tileType = _tileType;
        baseReward = _baseReward;
    }


    public void SetTileType(TileType type) {
        tileType = type;
    }
    public TileType GetTileType() {
        return tileType;
    }

    public int GetReward() {
        if (tileType == TileType.StartingPoint) {// if starting point
            return baseReward;
        }
        else { //if special tile
            int tempRoll = Random.Range(0, 10);


            switch (tempRoll) {
                case int n when (n <= 4):
                    return baseReward;
                case 5:
                    return (int)(baseReward * 1.2);
                case 6:
                    return (int)(baseReward * 1.4);
                case 7:
                    return (int)(baseReward * 0.8);
                case 8:
                    return (int)(baseReward * 0.6);
                case 9:
                    int secTemp = Random.Range(0, 2);
                    if (secTemp == 0) return (int)(baseReward * 1.8);
                    else return (int)(baseReward * 0.2);
                default:
                    return 0;
            }
        }
    }


    

}
