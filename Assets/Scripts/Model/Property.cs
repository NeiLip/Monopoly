using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property : Tile
{
    int ownedBy;//player index who ows this property
    int costPrice; //How much does this property costs
    int taxPrice; //How much a player have to pay if the property is held by other player

    //Default constructor
    public Property() {
        ownedBy = -1;
        costPrice = -1;
        taxPrice = -1;
    }
    public Property(int _ownedBy, int _costPrice, int _finePrice) {
        ownedBy = _ownedBy;
        costPrice = _costPrice;
        taxPrice = _finePrice;
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
    //Setter property tax
    public void SetTaxPrice(int _taxPrice) {
        taxPrice = _taxPrice;
    }
    //Getter property tax
    public int GetTaxPrice() {
        return taxPrice;
    }
}
