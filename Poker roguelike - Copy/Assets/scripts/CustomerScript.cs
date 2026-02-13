using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class CustomerScript : MonoBehaviour
{
    public List<GameObject> SelectedItems = new List<GameObject>();
    private int selectLimit;
    void Start()
    {
        selectLimit = 2 + (DataToStore.Instance.ElitesDefeated / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                if(hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                }
                else if(hit.collider.CompareTag("Attribute"))
                {
                    Buff(hit.collider.gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (GameObject item in SelectedItems)
            {
                if(item.tag == "Attribute")
                {
                    DataToStore.Instance.plyrAttributes.Add(item.GetComponent<VisualAttribScript>().attribute);
                }
                else if(item.tag == "Card")
                {
                    DataToStore.Instance.deck.Add(item.name);
                }
            }
            DataToStore.Instance.PrepNxtGame();
        }
    }

    void Buff(GameObject buff)
    {
        if (buff.GetComponent<VisualAttribScript>().Selected == true)
        {
            SelectedItems.Remove(buff);
            buff.GetComponent<VisualAttribScript>().Selected = false;
        }
        else
        {
            SelectedItems.Add(buff);
            buff.GetComponent<VisualAttribScript>().Selected = true;
        }
        if (SelectedItems.Count > selectLimit)
        {
            GameObject item = SelectedItems[0];
            if (item.CompareTag("Card"))
            {
                item.GetComponent<ShopCardScript>().Selected = false;
            }
            else if (item.CompareTag("Attribute"))
            {
                item.GetComponent<VisualAttribScript>().Selected = false;
            }
            SelectedItems.RemoveAt(0);
        }
    }

    void Card(GameObject card)
    {
        if (card.GetComponent<ShopCardScript>().Selected == true)
        {
            SelectedItems.Remove(card);
            card.GetComponent<ShopCardScript>().Selected = false;
        }
        else
        {
            SelectedItems.Add(card);
            card.GetComponent<ShopCardScript>().Selected = true;
        }
        if(SelectedItems.Count > selectLimit)
        {
            GameObject item = SelectedItems[0];
            if(item.CompareTag("Card"))
            {
                item.GetComponent<ShopCardScript>().Selected = false;
            }
            else if(item.CompareTag("Attribute"))
            {
                item.GetComponent<VisualAttribScript>().Selected = false;
            }
            SelectedItems.RemoveAt(0);
        }
    }
}
