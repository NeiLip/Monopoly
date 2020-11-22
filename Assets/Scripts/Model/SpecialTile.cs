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

}
