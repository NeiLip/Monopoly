using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property : Tile
{
    int ownedBy;//player index who ows this property
    int costPrice; //How much does this property costs
    int finePrice; //How much a player have to pay if the property is held by other player. i.e fine rate

    //Default constructor
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

    //Setter of owned player index
    public void SetOwnedByPlayerIndex(int _ownedBy) {
        ownedBy = _ownedBy;
    }
    //Getter of owned player index
    public int GetOwnedByPlayerIndex() {
        return ownedBy;
    }
    //Setter property price
    public void SetCostPrice(int _costPrice) {
        costPrice = _costPrice;
    }
    //Getter property price
    public int GetCostPrice() {
        return costPrice;
    }
    //Setter property fine
    public void SetFinePrice(int _finePrice) {
        finePrice = _finePrice;
    }
    //Getter property fine
    public int GetFinePrice() {
        return finePrice;
    }
}
