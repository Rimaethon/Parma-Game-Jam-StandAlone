using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private bool Mana = false;
    [SerializeField] private int ManaAmount = 0;
    public int manaAmount { get { return ManaAmount; } set { ManaAmount = value; } }
    public bool mana { get { return Mana; } set { Mana = value; } }
   
    void Update()
    {
        
    }
}
