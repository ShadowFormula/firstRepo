using UnityEngine;

public class Description : MonoBehaviour
{
    public string description;

    private void OnMouseEnter()
    {
        DescManager.Instance.SetAndShowDesc(description);
    }

    private void OnMouseExit()
    {
        DescManager.Instance.HideDesc();
    }
}
