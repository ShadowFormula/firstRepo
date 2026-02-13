using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class EnemyLogic : MonoBehaviour
{
    public int Health;
    
    public List<EnemyAttribute> debuffs = new List<EnemyAttribute>() {new NoDebuff()};
    public Text StrHealth;
    private void Start()
    {
        if(DataToStore.Instance.BossNode == true)
        {
            for (int i = 0; i < 3; i++)
            {
                EnemyAttribute newDebuff = RandTrait();
                while (debuffs.Select(p => p.Name).Contains(newDebuff.Name))
                {
                    newDebuff = RandTrait();
                }
                debuffs.Add(newDebuff);
            }
        }
        else if(DataToStore.Instance.EliteNode == true)
        {
            debuffs.Add(RandTrait());
        }
        Health = Mathf.RoundToInt((float)(300m * DataToStore.Instance.enemyHealthMultiplier));
        StrHealth.text = (Health <=0) ? "0" : Health.ToString();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Health = 0;
        }
        StrHealth.text = Health.ToString();
    }

    /// <summary>
    /// Returns a random enemy attribute that acts as a debuff to the score
    /// </summary>
    /// <returns></returns>
    EnemyAttribute RandTrait()
    {
        List<(EnemyAttribute trait, int weight)> pool = new List<(EnemyAttribute trait, int weight)>()
        {
            (new SpadeHeal(), 39),
            (new NighCard(), 20),
            (new GambleMult(), 10),
            (new DisableBuff(), 1)
        };

        int totalWeight = pool.Sum(p => p.weight);
        int roll = Random.Range(0, totalWeight);

        int cummulative = 0;
        foreach((EnemyAttribute trait, int weight) in pool)
        {
            cummulative += weight;
            if(roll < cummulative)
            {
                return trait;
            }
        }
        return new NighCard();

    }
}
