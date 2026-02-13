using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;

public class MapNode
{
    public int row;
    public int column;
    public NodeType type;
    public List<MapNode> nextNodes = new List<MapNode>();
    public bool unlocked;
    public bool completed;

    public GameObject view;
    /// <summary>
    /// Class type map node.
    /// </summary>
    /// <param name="r">row of the node</param>
    /// <param name="c">column of thee node</param>
    /// <param name="type">what type of node it is.e.g. boss, normal, elite or shop.</param>
    public MapNode(int r, int c, NodeType type)
    {
        row = r;
        column = c;
        this.type = type;
        unlocked = false;
        completed = false;
    }

    
}

public enum NodeType
{
    Shop, NormalEnemy, Boss, EliteEnemy
}
