using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonoBehaviour
{
    private int Range = 2;
    private Player player;
    private int FillMana = 20;
    public int fillMana { get { return FillMana; } set { FillMana = value; } }

    public void Start()
    {
        player = GameObject.FindObjectOfType(typeof(Player)) as Player;
    }

    public void Update()
    {
        CountDistance();
    }
    public void CountDistance()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < Range)
        {
            IncreaseMana();
        }
        else
        {
            return;
        }
    }

    public void IncreaseMana()
    {
        player.manaAmount++;
        Debug.Log("You Have" + player.manaAmount + "Mana!");
        gameObject.SetActive(false);
        if (player.manaAmount >= fillMana)
        {
            player.mana = true;
        }
    }
}