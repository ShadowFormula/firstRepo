using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    public GameObject LinePrefab;

    public GameObject[] layer1;
    public GameObject[] layer2;
    public GameObject[] layer3;
    public GameObject[] layer4;
    public GameObject[] layer5;
    public GameObject[] layer6;
    public GameObject[] layer7;
    public GameObject[] layer8;
    public GameObject[] layer9;
    public GameObject[] layer10;

    public List<GameObject[]> listOfLayers = new List<GameObject[]>();

    public GameObject NormalEnemy;
    public GameObject EliteEnemy;
    public GameObject Shop;
    public GameObject Boss;


    public List<List<MapNode>> mapLayers= new List<List<MapNode>>();
    


    public int layers = 9;
    public int minNodes = 3; //minimum nodes per layer
    public int maxNodes = 5; //maximum nodes per layer

    void Start()
    {
        CreateLayerList();

        if (DataToStore.Instance.mapLayers == null)
        {
            MakeNewMap();
            SaveMap();
        }
        else
        {
            mapLayers = DataToStore.Instance.mapLayers;
            if(!mapLayers.Any(layer => layer.Any(node => node.unlocked)))
            {
                foreach(MapNode node in mapLayers[0])
                {
                    node.unlocked = true;
                }
            }
        }
        if (mapLayers != null)
        {
            foreach (var layer in mapLayers)
            {
                foreach (var node in layer)
                {
                    if (node.view != null)
                    {
                        Destroy(node.view);
                        node.view = null;
                    }
                }
            }
        }
        OutputMap();
    }

    void MakeNewMap()
    {

        CreateMap();
        ConnectNodes();
        CheckDeadEnds();
        AddBoss();

        foreach(MapNode node in mapLayers[0])
        {
            node.unlocked = true;
        }
    }

    void SaveMap()
    {
        DataToStore.Instance.mapLayers = mapLayers;
    }

    void CreateMap()
    {
        for(int l = 0; l < layers; l++)
        {
            int nodeCount = Random.Range(minNodes, maxNodes+1);
            List<int> indeces = nodesOnLayer(nodeCount, maxNodes);
            List<MapNode> layer = new List<MapNode>();
            for (int c = 0; c < nodeCount; c++)
            {
                if (l == layers - 1)
                {
                    layer.Add(new MapNode(l, indeces[c], NodeType.Shop));
                }
                else
                {
                    layer.Add(new MapNode(l, indeces[c], RandType()));
                }
            }
            mapLayers.Add(layer);
        }
        
    }

    NodeType RandType()
    {
        List<(NodeType type, int wieght)> pool = new List<(NodeType type, int wieght)>()
        {
            (NodeType.NormalEnemy, 50),
            (NodeType.EliteEnemy, 35),
            (NodeType.Shop, 15)
        };

        int totatWeight = pool.Sum(p => p.wieght);
        int roll = Random.Range(0, totatWeight);

        int cumulative = 0;

        foreach ((NodeType type, int wieght) in pool)
        {
            cumulative += wieght;
            if (roll < cumulative)
            {
                return type;
            }
        }


        throw new System.Exception("did not work");
    }

    List<int> nodesOnLayer(int nodeCount, int totalSlots)
    {
        List<int> nodes = new List<int>();
        for(int i = 0; i <totalSlots; i++)
        {
            nodes.Add(i);
        }

        for(int i = 0; i < nodeCount; i++)
        {
            int j = Random.Range(i, nodes.Count);
            (nodes[i], nodes[j]) = (nodes[j], nodes[i]);
        }
        return nodes.Take(nodeCount).ToList();
    }

    void ConnectNodes()
    {
        for(int l = 0; l < mapLayers.Count -1; l++)
        {
            List<MapNode> currentLayer = mapLayers[l];
            List<MapNode> nextLayer = mapLayers[l + 1];

            foreach(MapNode node in currentLayer)
            {
                int connections = Random.Range(1, 3);

                for(int i = 0; i < connections; i++)
                {
                    int targetIndex = Mathf.Clamp(node.column + Random.Range(-1, 2), 0, 6);

                    MapNode TargetNode = nextLayer.OrderBy(n => Mathf.Abs(n.column - targetIndex)).First();

                    if(!node.nextNodes.Contains(TargetNode))
                    {
                        node.nextNodes.Add(TargetNode);
                    }
                }
            }
        }
    }

    void CreateLayerList()
    {
        listOfLayers.Add(layer1);
        listOfLayers.Add(layer2);
        listOfLayers.Add(layer3);
        listOfLayers.Add(layer4);
        listOfLayers.Add(layer5);
        listOfLayers.Add(layer6);
        listOfLayers.Add(layer7);
        listOfLayers.Add(layer8);
        listOfLayers.Add(layer9);
        listOfLayers.Add(layer10);
    }

    void CheckDeadEnds()
    {
        for(int l = 1;l < mapLayers.Count;l++)
        {
            foreach(MapNode node in mapLayers[l])
            {
                bool hasParent = mapLayers[l - 1].Any(n => n.nextNodes.Contains(node));
                if(!hasParent)
                {
                    MapNode parent = mapLayers[l - 1][Random.Range(0, mapLayers[l-1].Count)];
                    parent.nextNodes.Add(node);
                }
            }
        }
    }
    void AddBoss()
    {
        MapNode Boss = new MapNode(layers, 3, NodeType.Boss);
        foreach(MapNode node in mapLayers[layers -1])
        {
            node.nextNodes.Add(Boss);
        }
        mapLayers.Add(new List<MapNode>() { Boss });
    }

    void SetColourOfNode(MapNode node)
    {
        SpriteRenderer sr = node.view.GetComponent<SpriteRenderer>();

        if (!node.unlocked)
        {
            sr.color = Color.gray;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    void OutputMap()
    {
        for(int i =0; i < mapLayers.Count;i++)
        {

            if(i>= listOfLayers.Count)
            {
                Debug.Log($"Missing grid layer at layer {i}");
                continue;
            }
            GameObject[] GridLayer = listOfLayers[i];
            List<MapNode> currentLayer = mapLayers[i];
            foreach(MapNode node in currentLayer)
            {
                GameObject Node = new GameObject();
                if (node.type == NodeType.NormalEnemy)
                {
                    Node = Instantiate(NormalEnemy, GridLayer[node.column].transform.position, Quaternion.identity);
                }
                else if (node.type == NodeType.EliteEnemy)
                {
                    Node = Instantiate(EliteEnemy, GridLayer[node.column].transform.position, Quaternion.identity);
                }
                else if (node.type == NodeType.Shop)
                {
                    Node = Instantiate(Shop, GridLayer[node.column].transform.position, Quaternion.identity);
                }
                else if (node.type == NodeType.Boss)
                {
                    Node = Instantiate(Boss, GridLayer[0].transform.position, Quaternion.identity);
                }
                node.view = Node;
                SetColourOfNode(node);
                VisualNodeType viewComp = Node.AddComponent<VisualNodeType>();
                viewComp.node = node;
            }
        }
        DrawConnections();
    }

    void DrawConnections()
    {
        foreach(List<MapNode> layer in mapLayers)
        {
            foreach(MapNode node in layer)
            {
                if(node.view == null)
                {
                    continue;
                }

                foreach(MapNode nextNode in node.nextNodes)
                {
                    if(nextNode.view == null)
                    {
                        continue;
                    }

                    Vector3 start = node.view.transform.position;
                    Vector3 end = nextNode.view.transform.position;

                    start.z += 0.1f;
                    end.z += 0.1f;

                    GameObject lineObj = Instantiate(LinePrefab);
                    LineRenderer lr = lineObj.GetComponent<LineRenderer>();

                    lr.SetPosition(0, start);
                    lr.SetPosition(1, end);

                    if(node.completed)
                    {
                        lr.startColor = Color.green;
                    }
                    if(nextNode.unlocked)
                    {
                        lr.endColor = Color.yellow;
                    }
                    else
                    {
                        lr.startColor = Color.gray;
                    }
                }
            }
        }
    }

}
