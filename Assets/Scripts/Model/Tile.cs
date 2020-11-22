using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    GameObject tileGameObject;
    int tileIndex;


    public Tile() {
        tileGameObject = null;
        tileIndex = -1;
    }

    public Tile(GameObject _tileGameObject, int _tileIndex) {
        tileGameObject = _tileGameObject;
        tileIndex = _tileIndex;
    }

    public void SetTileGameObject(GameObject gameObject) {
        tileGameObject = gameObject;
    }
    public GameObject GetTilegameObject() {
        return tileGameObject;
    }
 

    public void SetTileIndex(int _tileIndex) {
        tileIndex = _tileIndex;
    }
    public int GetTileIndex() {
        return tileIndex;
    }
}
