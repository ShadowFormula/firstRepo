using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainCardStuff : MonoBehaviour
{

    public Sprite[] faces;
    public Transform deckPosition;
    //public DataToStore data;

    //these two arrays generate strings foreach card name
    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] ranks = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K" };
    
    public GameObject cardPrefab;
    public PlayerScript Player;
    public EnemyLogic Enemy;

    //these three arrays work together to keep track of what cards are in which slot
    public GameObject[] slots;
    public string[] placeHolders;
    public GameObject[] drawnCards;

    


    //
    public List<string> deck = new List<string>();
    public List<string> constDeck = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"DataToStore.Instance.deck is null? {DataToStore.Instance.deck == null}");
        Debug.Log($"DataToStore.Instance.deck.Count = {(DataToStore.Instance.deck != null ? DataToStore.Instance.deck.Count.ToString() : "null")}");

        if (DataToStore.Instance.deck == null || DataToStore.Instance.deck.Count < 8)
        {
            DrawCards();
        }
        else
        {

            constDeck = new List<string>(DataToStore.Instance.deck);
            Shuffle(constDeck);
            GrabCards();
        }
    }

    // Update is called once per frame
    void Update()
    { 

        UpdateDrawnCards();
        if(Enemy.Health <= 0)
        {

            SaveStats();
            DataToStore.Instance.PrepNxtGame();
        }
    }

    void SaveStats()
    {
        DataToStore.Instance.deck = constDeck;
        Player.SaveData();
    }

    /// <summary>
    /// Creates the deck. Shuffles the deck. Fills in card slots.
    /// </summary>
    public void DrawCards()
    {
        constDeck = CreateDeck();

        Shuffle(constDeck);
        GrabCards();
    }

    /// <summary>
    /// Outputs every successfully created card. 
    /// </summary>
    public void TestDeckGeneration()
    {
        foreach(string s in deck)
        {
            print(s);
        }
    }

    /// <summary>
    /// Creates a list of size 52 of card names in the format {card rank}{card suit}.
    /// </summary>
    /// <returns>String list of card names.</returns>
    public static List<string> CreateDeck()
    {
        List<string> cards = new List<string>();
        foreach (string s in suits)
        {
            foreach (string r in ranks)
            {
                cards.Add(r + s);
            }
        }
        return cards;
    }

    

    /// <summary>
    /// Selects random stringg from a list and add them to the main "deck" list while removing the string from the original list.
    /// </summary>
    /// <param name="TempDeck">List of string names of cards.</param>
    void Shuffle(List<string> TempDeck)
    {
        System.Random rand = new System.Random();
        while(TempDeck.Count > 0)
        {
            int index = rand.Next(0, TempDeck.Count);
            string card = TempDeck[index];
            deck.Add(card);
            TempDeck.RemoveAt(index);
        }
        constDeck = new List<string>(deck);  
        print(constDeck.Count);
    }
    /// <summary>
    /// Fills empty card slots with the first 8 cards of the deck.
    /// </summary>
    void GrabCards()
    {
        for(int i =0; i < 8; i++)
        {
            string card = deck[i];
            GameObject newCard = Instantiate(cardPrefab, new Vector3(slots[i].transform.position.x, slots[i].transform.position.y, slots[i].transform.position.z), Quaternion.identity);
            newCard.name = card;
            newCard.GetComponent<Selectable>().drawn = true;

            placeHolders[i] = newCard.name;
            drawnCards[i] = newCard;
        }
        foreach(string s in placeHolders)
        {
            deck.Remove(s);
        }
    }
    /// <summary>
    /// Refills empty card slots with new cards from the deck.
    /// </summary>
    void UpdateDrawnCards()
    {

        for (int i = 0; i < 8; i++)
        {
            if(drawnCards[i] == null)
            {
                placeHolders[i] = "X";
            }
        }
        for(int i = 0;i < 8; i++)
        {
            if(placeHolders[i] == "X" && deck.Count >0)
            {
                string card = deck[0];
                GameObject newCard = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity);
                newCard.name = card;
                newCard.GetComponent<Selectable>().drawn = true;

                placeHolders[i] = newCard.name;
                drawnCards[i] = newCard;
                deck.Remove(card);

                StartCoroutine(MoveCardToSlot(newCard, slots[i].transform.position));
            }
        }
    }

    IEnumerator MoveCardToSlot(GameObject card, Vector3 target)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        Vector3 start = card.transform.position;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            card.transform.position = Vector3.Lerp(start, target, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        card.transform.position = target;
    }
    void OnDestroy()
    {
        StopAllCoroutines();
    }

}
