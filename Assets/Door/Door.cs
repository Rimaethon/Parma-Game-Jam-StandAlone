using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Mana mana;
    Player player;
    float wait = 1f;
    int Range = 20;
    [SerializeField] private int DoorHeal = 20;

    public void Start()
    {
        player = FindObjectOfType<Player>();
        mana = FindObjectOfType<Mana>();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(BuildDoor());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            DoorHeal -= 1;
            if (DoorHeal == 0)
            {
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
    public bool CountDistance()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < Range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator BuildDoor()
    {
        if (DoorHeal == 0)
        {
            if (CountDistance() && player.mana == true)
            {
                PartedDoor.instance.Rebuild();
                player.manaAmount -= mana.fillMana;
                yield return new WaitForSeconds(wait);
                this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                DoorHeal = 20;
            }
            else
            {
                Debug.Log("You Dont Have Enough Mana or get closer to Door!");
            }
        }
    }

}