using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property : Tile
{

    int ownedBy;
    int costPrice; //How much the property costs
    int finePrice; //How much a player have to pay if property is held by the other player

    public Property() {
        ownedBy = -1;
        costPrice = -1;
        finePrice = -1;
    }
    public Property(int _ownedBy, int _costPrice, int _finePrice) {
        ownedBy = _ownedBy;
        costPrice = _costPrice;
        finePrice = _finePrice;
    }

    public void SetOwnedByPlayerIndex(int _ownedBy) {
        ownedBy = _ownedBy;
    }
    public int GetOwnedByPlayerIndex() {
        return ownedBy;
    }

    public void SetCostPrice(int _costPrice) {
        costPrice = _costPrice;
    }
    public int GetCostPrice() {
        return costPrice;
    }

    public void SetFinePrice(int _finePrice) {
        finePrice = _finePrice;
    }
    public int GetFinePrice() {
        return finePrice;
    }
}
