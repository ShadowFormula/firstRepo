using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;

public class UpdateSprite : MonoBehaviour
{

    public Sprite cardFace;
    public Sprite cardBack;
    public SpriteRenderer spriteRenderer;
    private Selectable selected;
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
        selected = GetComponent<Selectable>();
    }

    void Update()
    {
        if(selected.drawn == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
        if(selected.selected == true)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.grey;
        }
    }
}
