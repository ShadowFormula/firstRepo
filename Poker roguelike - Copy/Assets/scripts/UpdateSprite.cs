using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;

public class UpdateSprite : MonoBehaviour
{

    public Sprite cardFace;
    public Sprite cardBack;
    public SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private MainCardStuff cardStuff;
    void Start()
    {
        List<string> deck = MainCardStuff.CreateDeck();
        cardStuff = FindFirstObjectByType<MainCardStuff>();

        int index = 0;
        foreach(string cardName in deck)
        {
            if(this.name == cardName)
            {
                cardFace = cardStuff.faces[index];
                break;
            }
            index++;
            
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
    }

    void Update()
    {
        if(selectable.drawn == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
        if(selectable.selected == true)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.grey;
        }
    }
}
