using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inputs : MonoBehaviour
{
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                VisualNodeType view = hit.collider.GetComponent<VisualNodeType>();
                if (view == null)
                {
                    return;
                }
                TryNode(view.node);

            }
        }
    }

    void TryNode(MapNode node)
    {
        if(!node.unlocked || node.completed)
        {
            print("node is locked");
            return;
        }

        MoveToNode(node);
    }

    void MoveToNode(MapNode node)
    {
        node.completed = true;

        foreach(List<MapNode> layer in DataToStore.Instance.mapLayers)
        {
            foreach(MapNode n in layer)
            {
                n.unlocked = false;
            }
        }
        foreach(MapNode n in node.nextNodes)
        {
            n.unlocked = true;

            SpriteRenderer sr = n.view.GetComponent<SpriteRenderer>();
            sr.color = Color.white;
        }

        DataToStore.Instance.currentNode = node;

        switch(node.type)
        {
            case NodeType.NormalEnemy:
                SceneManager.LoadScene("main game");
                break;
            case NodeType.EliteEnemy:
                DataToStore.Instance.EliteNode = true;
                SceneManager.LoadScene("main game");
                break;
            case NodeType.Boss:
                DataToStore.Instance.BossNode = true;
                SceneManager.LoadScene("main game");
                break;
            case NodeType.Shop:
                SceneManager.LoadScene("shop scene");
                break;

        }
    }
}

