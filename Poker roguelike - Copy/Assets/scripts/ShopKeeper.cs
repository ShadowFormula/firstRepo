using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject cardPrefab;
    public GameObject buffPrefab;
    public GameObject[] slots;
    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] ranks = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K" };
    private List<string> cards = new List<string>();

    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpCardShop();
        SetUpBuffShop();
    }

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

    void SetUpCardShop()
    {
        Shuffle(CreateDeck());

        for (int i = 0; i < 6; i++)
        {
            string card = cards[0];
            GameObject newCard = Instantiate(cardPrefab, this.transform.position, Quaternion.identity);
            newCard.name = card;
            StartCoroutine(MoveCardToSlot(newCard, slots[i].transform.position));
            cards.Remove(card);
        }
    }

    void SetUpBuffShop()
    {
        for(int i = 6; i < slots.Length; i++)
        {
            GameObject newBuff = Instantiate(buffPrefab, this.transform.position, Quaternion.identity);
            BuffType buff = RandBuff();
            
            newBuff.GetComponent<VisualAttribScript>().SetAttribute(CreateBuff(buff), buff);
            StartCoroutine(MoveCardToSlot(newBuff, slots[i].transform.position));
        }
    }

    public static BuffType RandBuff()
    {
        List<(BuffType buff, int weight)> pool = new List<(BuffType buff, int weight)>()
        {
            (BuffType.HeartBonus, 20),
            (BuffType.StraightChips, 20),
            (BuffType.Fibonacci,  20),
            (BuffType.OneBuffArmy, 20),
            (BuffType.TwoFace, 20)
        };
        int totalWeight = pool.Sum(p => p.weight);
        int roll = Random.Range(0, totalWeight);

        int cummulative = 0;
        foreach ((BuffType buff, int weight) in pool)
        {
            cummulative += weight;
            if (roll < cummulative)
            {
                return buff;
            }
        }
        throw new System.Exception("rand buff didn't work");
    }

    void Shuffle(List<string> TempDeck)
    {
        System.Random rand = new System.Random();
        while (TempDeck.Count > 0)
        {
            int index = rand.Next(0, TempDeck.Count);
            string card = TempDeck[index];
            cards.Add(card);
            TempDeck.RemoveAt(index);
        }
    }

    IEnumerator MoveCardToSlot(GameObject card, Vector3 target)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        Vector3 start = card.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            card.transform.position = Vector3.Lerp(start, target, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        card.transform.position = target;
    }

    playerAttribute CreateBuff(BuffType type)
    {
        switch(type)
        {
            case BuffType.HeartBonus:
                return new HeartBonus();
                break;
            case BuffType.Fibonacci: 
                return new Fibonacci();
                break;
            case BuffType.OneBuffArmy:
                return new OneBuffArmy();
                break;
            case BuffType.StraightChips:
                return new StraightChips();
                break;
            case BuffType.TwoFace:
                return new TwoFace();
                break;
        }
        throw new System.Exception("kys");
    }
}

public enum BuffType
{
    HeartBonus,
    StraightChips,
    Fibonacci,
    OneBuffArmy,
    TwoFace
}


