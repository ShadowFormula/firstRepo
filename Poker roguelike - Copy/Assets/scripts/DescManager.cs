using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class DescManager : MonoBehaviour
{
    public static DescManager Instance;

    public TextMeshProUGUI TextComponent;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAndShowDesc(string desc)
    {
        gameObject.SetActive(true);
        TextComponent.text = desc;
    }

    public void HideDesc()
    {
        gameObject.SetActive(false);
        TextComponent.text = "";
    }
}
