using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A basic tile object
public class Tile
{
    GameObject tileGameObject;
    int tileIndex;

    //Default constructor
    public Tile() {
        tileGameObject = null;
        tileIndex = -1;
    }

    public Tile(GameObject _tileGameObject, int _tileIndex) {
        tileGameObject = _tileGameObject;
        tileIndex = _tileIndex;
    }

    //Setter gameobject
    public void SetTileGameObject(GameObject gameObject) {
        tileGameObject = gameObject;
    }
    //Getter gameobject
    public GameObject GetTilegameObject() {
        return tileGameObject;
    }

    //Setter tile index
    public void SetTileIndex(int _tileIndex) {
        tileIndex = _tileIndex;
    }
    //Getter tile index
    public int GetTileIndex() {
        return tileIndex;
    }
}
