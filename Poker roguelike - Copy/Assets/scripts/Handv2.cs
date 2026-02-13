using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{




    private static List<(char rank, int value)> hierachy = new List<(char rank, int value)>
    {
        ('A', 14), ('K', 13), ('Q', 12), ('J', 11), ('T', 10), ('9', 9), ('8', 8), ('7', 7),
            ('6', 6), ('5', 5), ('4', 4), ('3', 3), ('2', 2)
    };
    private Dictionary<int, char> IntChar;
    public int handWorth = 0;
    private List<int> CardRanks = new List<int>();
    private List<char> CardSuits = new List<char>();
    //private string[] hand;
    private Dictionary<char, int> rankValues = hierachy.ToDictionary(x => x.rank, x => x.value);
    private List<Tuple<int, char, string>> Cards = new List<Tuple<int, char, string>>();
    public List<Tuple<int, char, string>> ValidCardsToScore = new List<Tuple<int, char, string>>();

    //public int totalChips;



    public void CreateHand(string playedHand)
    {
        ResetHand();

        string[] hand = playedHand.Split(' ');
        CreateReverseDict();

        //extract rank of each card 
        foreach (string card in hand)
        {
            //print(rankValues[card[0]]);
            CardRanks.Add(rankValues[card[0]]);
            CardSuits.Add(card[1]);
        }



        SortRankings();
        //Debug.Log($"New hand: {string.Join(", ", hand)}");
        //Debug.Log($"Ranks: {string.Join(", ", CardRanks)}");
        //Debug.Log($"Suits: {string.Join(", ", CardSuits)}");
    }

    private void ResetHand()
    {
        CardRanks.Clear();
        CardSuits.Clear();
        handWorth = 0;
        Cards.Clear();
        ValidCardsToScore.Clear();
        Cards.Clear();
    }



    void CreateReverseDict()
    {
        IntChar = new Dictionary<int, char>();
        foreach ((char c, int i) in rankValues)
        {
            IntChar[i] = c;
        }
    }



    /// <summary>
    /// sorts the card ranks via the dictionary "rankValues" so checking for straights is easier
    /// </summary>
    void SortRankings()
    {
        List<Tuple<int, char>> temp = CardRanks.Select((rank, i) => Tuple.Create(rank, CardSuits[i])).ToList();
        temp.Sort((a, b) => b.Item1.CompareTo(a.Item1));
        CardRanks = temp.Select(x => x.Item1).ToList();
        CardSuits = temp.Select(x => x.Item2).ToList();
        for (int i = 0; i < CardRanks.Count; i++)
        {
            string cardName = IntChar[CardRanks[i]].ToString() + CardSuits[i];
            Cards.Add(Tuple.Create(CardRanks[i], CardSuits[i], cardName));
        }
    }

    void giveWorth()
    {
        ValidCardsToScore.Clear();
        handWorth = 0;
        foreach (int num in CardRanks)
        {
            handWorth += num;
        }
        foreach (Tuple<int, char, string> card in Cards)
        {
            ValidCardsToScore.Add(card);
        }
    }




    public int getHandWorth()
    {
        return handWorth;
    }
    /// <summary>
    /// finds the number duplicate ranks in the hand
    /// </summary>
    /// <param name="n">the rank of a card</param>
    /// <returns>the number of cards with the rank "n" in the hand</returns>
    public int calcAmounOfRank(int n)
    {
        int count = CardRanks.Count(p => p == n);

        return count;
    }

    

    int ScoreValidChips(List<int> ranks)
    {
        int score = 0;
        foreach (int rank in ranks)
        {
            if (calcAmounOfRank(rank) == 4)
            {
                score += rank * 4;
                AppendValidCards(rank);
            }
            else if (calcAmounOfRank(rank) == 3)
            {
                score += rank * 3;
                AppendValidCards(rank);
            }
            else if (calcAmounOfRank(rank) == 2)
            {
                score += rank * 2;
                AppendValidCards(rank);
            }
        }
        return score;
    }
    void AppendValidCards(int rank)
    {
        foreach (Tuple<int, char, string> card in Cards)
        {
            if (card.Item1 == rank)
            {
                ValidCardsToScore.Add(card);
            }
        }
    }


    /// <summary>
    /// Checks if played hand contains exactly two cards with the same rank.
    /// </summary>
    /// <returns>True if hand contains exactly two cards with the same rank. False if otherwise</returns>
    public bool isPair()
    {
        List<int> uniqueRank = new List<int>();
        int numberOfDouble = 0;
        foreach (int rank in CardRanks)
        {
            if (uniqueRank.Contains(rank))
            {
                continue;
            }

            if (calcAmounOfRank(rank) == 2)
            {
                numberOfDouble++;
            }
            uniqueRank.Add(rank);
        }
        if (numberOfDouble == 1)
        {
            handWorth = ScoreValidChips(uniqueRank);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Checks if the current played hand contains two pairs.
    /// </summary>
    /// <returns>true if hand contains two pairs. False if otherwise.</returns>
    public bool is2Pair()
    {
        List<int> uniqueRank = new List<int>();
        int numberOfDouble = 0;
        foreach (int rank in CardRanks)
        {
            if (uniqueRank.Contains(rank))
            {
                continue;
            }

            numberOfDouble += (calcAmounOfRank(rank) == 2) ? 1 : 0;
            uniqueRank.Add(rank);
        }
        if (numberOfDouble == 2)
        {
            handWorth = ScoreValidChips(uniqueRank);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Checks if the current played hand is a three of a kind
    /// </summary>
    /// <returns>true if hand is a three of a kind. False if otherwise.</returns>
    public bool is3OfKind()
    {
        List<int> uniqueRank = new List<int>();
        int numberOfTriple = 0;
        foreach (int rank in CardRanks)
        {
            if (uniqueRank.Contains(rank))
            {
                continue;
            }
            numberOfTriple += (calcAmounOfRank(rank) == 3) ? 1 : 0;
            uniqueRank.Add(rank);
        }
        if (numberOfTriple == 1)
        {
            handWorth = ScoreValidChips(uniqueRank);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the current played hand is a four of a kind
    /// </summary>
    /// <returns>true if hand is four of a kind. False if otherwise.</returns>
    public bool is4OfKind()
    {
        List<int> uniqueRank = new List<int>();
        int numberOfQuad = 0;
        foreach (int rank in CardRanks)
        {

            if (uniqueRank.Contains(rank))
            {
                continue;
            }

            numberOfQuad += (calcAmounOfRank(rank) == 4) ? 1 : 0;
            uniqueRank.Add(rank);
        }

        if (numberOfQuad == 1)
        {
            handWorth = ScoreValidChips(uniqueRank);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Checks if the current played hand is a full house.
    /// </summary>
    /// <returns>true if hand is a full house. False if otherwise.</returns>
    public bool isFullHouse()
    {
        List<int> uniqueRank = new List<int>();
        int numberOfDouble = 0;
        int numberOfTriple = 0;
        foreach (int rank in CardRanks)
        {

            if (uniqueRank.Contains(rank))
            {
                continue;
            }

            numberOfDouble += (calcAmounOfRank(rank) == 2) ? 1 : 0;
            numberOfTriple += (calcAmounOfRank(rank) == 3) ? 1 : 0;
            uniqueRank.Add(rank);
        }
        if (numberOfDouble == 1 && numberOfTriple == 1)
        {
            handWorth = ScoreValidChips(uniqueRank);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the current played hand is a straight flush.
    /// </summary>
    /// <returns>true if hand is a straight and a flush. False if otherwise.</returns>
    public bool isStraightFlush()
    {

        if (isFlush() && isStraight())
        {
            giveWorth();
            handWorth *= 9;
            return true;
        }
        else
        {
            handWorth = 0;
            ValidCardsToScore.Clear();
            return false;
        }
    }
    /// <summary>
    /// Checks if the current played hand is a straight
    /// </summary>
    /// <returns>true if hand is a straight. False if otherwise.</returns>
    public bool isStraight()
    {
        if (Cards.Count != 5)
        {
            return false;
        }
        if (CardRanks.SequenceEqual(new int[] { 14, 5, 4, 3, 2 }))
        {
            giveWorth();
            return true;
        }
        for (int i = 0; i < CardRanks.Count - 1; i++)
        {
            if (CardRanks[i] - CardRanks[i + 1] != 1)
            {
                handWorth = 0;
                return false;
            }
        }
        giveWorth();
        return true;
    }
    /// <summary>
    /// Check if hand is a royal flush.
    /// </summary>
    /// <returns>True if hand is a royal flush. False if otherwise.</returns>
    public bool isRoyalFlush()
    {

        bool flush = isFlush();
        if (flush && CardRanks.SequenceEqual(new int[] { 14, 13, 12, 11, 10 }))
        {
            handWorth = 0;
            giveWorth();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if every card is from the same suit.
    /// </summary>
    /// <returns>True if all cards is from the same suit. False if otherwise.</returns>
    public bool isFlush()
    {
        if (Cards.Count != 5)
        {
            return false;
        }
        HashSet<char> uniqueSuit = new HashSet<char>();

        foreach (char s in CardSuits)
        {
            uniqueSuit.Add(s);
        }
        if (uniqueSuit.Count == 1)
        {
            giveWorth();
            return true;
        }
        return false;
    }


    public bool isHighCard()
    {
        string cardName = IntChar[CardRanks[0]].ToString() + CardSuits[0];
        ValidCardsToScore.Add(Tuple.Create(CardRanks[0], CardSuits[0], cardName));
        handWorth += CardRanks[0];
        return true;
    }
}
