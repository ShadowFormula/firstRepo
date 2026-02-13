using System.Collections.Generic;
using UnityEngine;

public class ShopCardScript : MonoBehaviour
{
    public Sprite cardFace;
    public SpriteRenderer spriteRenderer;
    public ShopKeeper shopKeeper;
    public bool Selected;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<string> deck = ShopKeeper.CreateDeck();
        shopKeeper = FindFirstObjectByType<ShopKeeper>();

        int index = 0;
        foreach (string cardName in deck)
        {
            if (this.name == cardName)
            {
                cardFace = shopKeeper.faces[index];
                break;
            }
            index++;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = cardFace;
    }

    // Update is called once per frame
    void Update()
    {
        if(Selected)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.grey;
        }
    }
}
