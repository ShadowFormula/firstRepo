using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

[System.Serializable]
public abstract class Attribute
{
    public string Name { get; protected set; }
    public bool IsActive { get; protected set; }

    
}

[System.Serializable]
public abstract class playerAttribute : Attribute
{
    public bool Enabled { get;  set; }

    public abstract void ModifyScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player);
    protected int Sum(List<Tuple<int, char, string>> cards)
    {
        if (cards.Count > 1)
        {
            return cards[0].Item1 + Sum(cards.GetRange(1, cards.Count - 1));
        }
        return cards[0].Item1;
    }
}

public class NormalScore : playerAttribute
{
    public NormalScore()
    {
        Name = "NormalScore";
        IsActive = false;
        Enabled = true;
    }
    public override void ModifyScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult,  int handType, ref PlayerScript player)
    {
        ChipMult[0] = Sum(cards);
    }

    
}




/// <summary>
/// Adds 2 to the multiplier for every heart card scored.
/// </summary>
public class HeartBonus : playerAttribute
{
    public HeartBonus()
    {
        Name = "HeartBonus";
        IsActive=false;
        Enabled=true;
    }
    public override void ModifyScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
        int numHearts = cards.Select(x => x.Item2).Count(s => s =='H');
        if(numHearts > 0 && Enabled)
        {
            IsActive = true;
            int newMult = numHearts * 2;
            ChipMult[1] += newMult;
        }
        else
        {
            IsActive= false;
        }
        
        
    }
}
[System.Serializable]
/// <summary>
/// Player buff that rewards the player with an additional 5 chips everytime a straight is scored. The amount of chips rewarded increases everything this buff is triggerred.
/// </summary>
public class StraightChips : playerAttribute
{
    [SerializeField] private int chipMultiplyer = 1;
    public StraightChips()
    {
        Name = "StraightChips";
        IsActive = false;
        Enabled = true;
    }

    public override void ModifyScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
        if((handType == 5 || handType == 9 || handType == 10) && Enabled) // if hand is a straight
        {
            IsActive = true;
            ChipMult[0] += 5 * chipMultiplyer;
            chipMultiplyer++;
        }
        else
        {
            IsActive = false;
        }
    }
}

public class Fibonacci : playerAttribute
{
    public Fibonacci()
    {
        Name = "Fibonacci";
        IsActive = false;
        Enabled= true;
    }

    public override void ModifyScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
        if(cards.Select(x => x.Item1).Count(p => p == 14 || p == 2 || p == 3 || p == 5 || p == 8) > 0 && Enabled)
        {
            IsActive=true;
            foreach(var card in cards)
            {
                if(card.Item1 == 14 || card.Item1 == 2 || card.Item1 == 3 || card.Item1 == 5 || card.Item2 == 8)
                {
                    ChipMult[1] += 8;
                }
            }
        }
        else
        {
            IsActive = false;
        }
    }
}

public class OneBuffArmy : playerAttribute
{
    public OneBuffArmy()
    {
        Name = "OneBuffArmy";
        IsActive = false;
        Enabled = true;
    }

    public override void ModifyScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
        if(player.plyrAttributes.Count > 1)
        {
            IsActive = true;
            int mult = player.plyrAttributes.Count - 1;
            ChipMult[1] += 3 * mult;
        }
        else
        {
            IsActive = false;
        }
    }
}

public class TwoFace : playerAttribute
{
    public TwoFace()
    {
        Name = "TwoFace";
        IsActive = false;
        Enabled= true;
    }

    public override void ModifyScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
        if(cards.Select(c => c.Item1).Count(x => x == 13 || x == 12 || x == 11) > 0)
        {
            IsActive = true;
            foreach(var card in cards)
            {
                if(card.Item1 == 13 || card.Item1 == 12 || card.Item1 == 11)
                {
                    ChipMult[0] += card.Item1;
                }
            }
        }
        else
        {
            IsActive = false;
        }
    }
}


public abstract class EnemyAttribute : Attribute
{
    public abstract void DebuffScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player);
}

public class SpadeHeal : EnemyAttribute
{
    public SpadeHeal()
    {
        Name = "SpadeHeal";
        IsActive=false;
    }
    public override void DebuffScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
        int numSpades = cards.Select(x => x.Item2).Count(s => s == 'S');
        if( numSpades > 0)
        {
            IsActive = true;
            foreach(Tuple<int, char, string> card in cards)
            {
                if(card.Item2 == 'S')
                {
                    ChipMult[0] -= card.Item1;
                }
            }
        }
        else
        {
            IsActive = false;
        }
    }
}

public class NighCard : EnemyAttribute
{
    public NighCard()
    {
        Name = "NighCard";
        IsActive=false;
    }

    public override void DebuffScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
        if(handType == 1) //if hand is a high card
        {
            IsActive = true;
            ChipMult[0] = 0;
        }
        else
        {
            IsActive = false;
        }
    }
}

public class GambleMult : EnemyAttribute
{
    public GambleMult()
    {
        Name = "GambleMult";
        IsActive = true; 
    }

    public override void DebuffScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
        int temp = ChipMult[1] - handType;
        int randMult = UnityEngine.Random.Range(1, 11);
        ChipMult[1] = temp + randMult;
    }
}

public class DisableBuff : EnemyAttribute
{
    public DisableBuff()
    {
        Name = "DisableBuff";
        IsActive = true;
    }

    public override void DebuffScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
        int roll = UnityEngine.Random.Range(0, 100);
        if (roll < 10)
        {
            int index = UnityEngine.Random.Range(0, player.plyrAttributes.Count);
            while(player.plyrAttributes[index].Name == "NormalScore")
            {
                index = UnityEngine.Random.Range(0, player.plyrAttributes.Count);
            }
            player.plyrAttributes[index].Enabled = false;
        }
    }
}

public class NoDebuff : EnemyAttribute
{
    public NoDebuff()
    {
        Name = "";
        IsActive = false;
    }

    public override void DebuffScore(List<Tuple<int, char, string>> cards, ref int[] ChipMult, int handType, ref PlayerScript player)
    {
    }
}

