using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEditor.SearchService;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
public class ScoreScript : MonoBehaviour
{
    public int Mult;
    public int Chips;
    public Text finalScore;
    public Text ActualScore;
    public Text Multiplier;
    public Text Debuff;
    public EnemyLogic Enemy;
    public PlayerScript Player;
    public bool DoDamage = false;



    public void Product(int score, int Mul, Attribute attribute, List<Tuple<int, char, string>> cards, int handType)
    {
        Chips = score;
        Mult = Mul;
        
        finalScore.text = Chips.ToString() + "x" + Mult.ToString();
        ActualScore.text = (Chips * Mult).ToString();
        Multiplier.text = (attribute.IsActive)? attribute.Name : null;
        if(DoDamage)
        {
            StartCoroutine(DeBuffScore(cards, handType));
            
            DoDamage = false;
        }
    }

    IEnumerator DeBuffScore(List<Tuple<int, char, string>> cards, int handType)
    {
        int[] ChipMult = new int[2] {Chips, Mult};
        foreach(EnemyAttribute debuff in Enemy.debuffs)
        {
            debuff.DebuffScore(cards, ref ChipMult, handType, ref Player);
            Debuff.text = (debuff.IsActive) ? debuff.Name : null;
            finalScore.text = ChipMult[0].ToString() + "x" + ChipMult[1].ToString();
            ActualScore.text = (ChipMult[0] * ChipMult[1]).ToString();
            yield return new WaitForSeconds(0.5f);
        }

        Chips = ChipMult[0];
        Mult = ChipMult[1];
        int damage = Chips * Mult;
        Enemy.Health -= damage;
    }
}
