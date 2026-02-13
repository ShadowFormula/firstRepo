using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DataToStore : MonoBehaviour
{
    public static DataToStore Instance;

    //combat data
    [SerializeField] public decimal enemyHealthMultiplier = 1;
    [SerializeField] public bool EliteNode = false;
    [SerializeField] public bool BossNode = false;
    public int ElitesDefeated = 0;

    //map data
    public List<List<MapNode>> mapLayers;
    public MapNode currentNode;

    //player data
    [SerializeReference]
    public List<playerAttribute> plyrAttributes;
    public List<string> deck;
    public int RemainingDiscards;
    public int RemainingHands;
    public int plyrHealth;



    void Awake()
    {
        plyrAttributes.Add(new NormalScore());
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        if(deck == null || deck.Count == 0)
        {
            deck = new List<string>(MainCardStuff.CreateDeck());
        }
    }



    public void PrepNxtGame()
    {
        enemyHealthMultiplier += 1.6m;
        RemainingDiscards++;
        if(BossNode)
        {
            BossNode = false;
        }
        if(EliteNode)
        {
            EliteNode = false;
            ElitesDefeated++;
        }
        SceneManager.LoadScene("menu scene");
    }

}
