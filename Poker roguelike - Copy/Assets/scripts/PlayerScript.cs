using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEditor.SearchService;
using System;
using System.Runtime.CompilerServices;

public class PlayerScript : MonoBehaviour
{
    public PlayerScript player;

    Dictionary<int, string> HandTypes = new Dictionary<int, string>()
    {{1, "HighCard"}, {2, "Pair"}, {3, "TwoPair"}, {4, "3OfAKind"}, {5, "Straight"}, {6, "Flush" }, {7, "FullHouse" }, {8, "4OfAKind"}, {9, "StraightFlush"},
        {10, "RoyalFlush" } };
    [SerializeReference] public List<playerAttribute> plyrAttributes = new List<playerAttribute>();
    public int plyrHealth;
    public ScoreScript product;
    public int RemainingDiscards;
    public int RemainingHands;
    public TextMeshProUGUI HandType;
    public TextMeshProUGUI HandsAndDiscards;

    private List<string> namesOfSelectedCards = new List<string>();
    public List<GameObject> currentlySelectedCards = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = this;
        if(DataToStore.Instance.plyrAttributes.Count == 0 || DataToStore.Instance.RemainingHands == 0)
        {
            plyrAttributes.Add(new NormalScore());
            plyrHealth = 2;
            RemainingDiscards = 5;
            RemainingHands = 6;
        }
        else
        {
            plyrAttributes = new List<playerAttribute>(DataToStore.Instance.plyrAttributes);
            plyrHealth = DataToStore.Instance.plyrHealth;
            RemainingHands = DataToStore.Instance.RemainingHands;
            RemainingDiscards = DataToStore.Instance.RemainingDiscards;
        }
    }

    void Update()
    {
        GetMouseClick();
        UpdateText();
    }

    public void SaveData()
    {
        DataToStore.Instance.plyrAttributes = plyrAttributes;
        DataToStore.Instance.RemainingDiscards = RemainingDiscards;
        DataToStore.Instance.RemainingHands = RemainingHands;
        DataToStore.Instance.plyrHealth = plyrHealth;
    }

    void UpdateText()
    {
        HandsAndDiscards.text = "Hands: " + RemainingHands + Environment.NewLine + "Discards: " + RemainingDiscards; 
    }


    public void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                if (hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Play Button") && RemainingHands >0)
                {
                    PlayButton();
                    RemainingHands--;
                }
                else if (hit.collider.CompareTag("Discard Button") && RemainingDiscards >0)
                {
                    DiscardButton();
                    RemainingDiscards--;
                }
            }
        }
    }

    void Card(GameObject SelectedCard)
    {
        Selectable cardProperties = SelectedCard.GetComponent<Selectable>();
        if (cardProperties.selected == true)
        {
            //if card is selected
            //deselect card
            currentlySelectedCards.Remove(SelectedCard);
            cardProperties.selected = false;
        }
        else
        {
            //if card not selected
            //select card
            currentlySelectedCards.Add(SelectedCard);
            cardProperties.selected = true;
        }
        //the amount of cards the player can play is a maximum of 5. This if statement makes sure that only a maximum of 5 cards can be selected
        if (currentlySelectedCards.Count == 6)
        {
            currentlySelectedCards[0].GetComponent<Selectable>().selected = false;
            currentlySelectedCards.RemoveAt(0);
        }
    }
    void PlayButton()
    {
        foreach (GameObject Card in currentlySelectedCards)
        {
            namesOfSelectedCards.Add(Card.name);
        }
        string hand = string.Join(" ", namesOfSelectedCards.ToArray());
        namesOfSelectedCards.Clear();
        Hand hand1 = GameObject.FindGameObjectWithTag("Deck").GetComponent<Hand>();
        
        hand1.CreateHand(hand);
        int mult = eval(hand1);
        StartCoroutine(GetDamage(mult, hand1));
        foreach(GameObject Card in currentlySelectedCards)
        {
            Destroy(Card);
        }
        currentlySelectedCards.Clear();
    }
    void DiscardButton()
    {
        foreach (GameObject Card in currentlySelectedCards)
        {
            Destroy(Card);
        }
        currentlySelectedCards.Clear();
    }
    public static int eval(Hand hand)
    {
        hand.handWorth = 0;
        if (hand.isRoyalFlush())
        {
            return 10;
        }
        else if (hand.isStraightFlush())
        {
            return 9;
        }
        else if (hand.is4OfKind())
        {
            return 8;
        }
        else if (hand.isFullHouse())
        {
            return 7;
        }
        else if (hand.isFlush())
        {
            return 6;
        }
        else if (hand.isStraight())
        {
            return 5;
        }
        else if (hand.is3OfKind())
        {
            return 4;
        }
        else if (hand.is2Pair())
        {
            return 3;
        }
        else if (hand.isPair())
        {

            return 2;
        }
        else if (hand.isHighCard())
        {
            return 1;
        }
        return 0;
    }


    IEnumerator GetDamage(int mult, Hand hand)
    {
        int[] ChipMult = new int[2] {0, mult};
        List<Tuple<int, char, string>> cards = hand.ValidCardsToScore;
        
        foreach(playerAttribute attribute in plyrAttributes)
        {
            if(plyrAttributes.IndexOf(attribute) == plyrAttributes.Count-1)
            {
                product.DoDamage = true;
            }
            attribute.ModifyScore(cards, ref ChipMult, eval(hand), ref player);
            product.Product(ChipMult[0], ChipMult[1], attribute, cards, eval(hand));
            yield return new WaitForSeconds(0.5f);
        }
        HandType.text = HandTypes[eval(hand)];
    }

}


