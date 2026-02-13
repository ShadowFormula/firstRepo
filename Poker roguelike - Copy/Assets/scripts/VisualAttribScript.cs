using UnityEngine;

public class VisualAttribScript : MonoBehaviour
{

    public playerAttribute attribute;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public Description description;
    public bool Selected = false;

    private void Start()
    {
    }

    public void SetAttribute(playerAttribute attr, BuffType buff)
    {
        attribute = attr;

        switch (buff)
        {
            case BuffType.HeartBonus:
                spriteRenderer.sprite = sprites[2];
                description.description =
                    "Adds 2 to the multiplier for every heart card scored.";
                break;

            case BuffType.StraightChips:
                spriteRenderer.sprite = sprites[1];
                description.description =
                    "Gain +5 chips when scoring a straight. This bonus increases each time.";
                break;

            case BuffType.Fibonacci:
                spriteRenderer.sprite = sprites[3];
                description.description =
                    "Adds 8 to the mult for every Ace, 2, 3, 5 or 8 scored.";
                break;

            case BuffType.OneBuffArmy:
                spriteRenderer.sprite = sprites[0];
                description.description =
                    "Add 3 to the mult for every attribute the player has.";
                break;

            case BuffType.TwoFace:
                spriteRenderer.sprite = sprites[4];
                description.description =
                    "Every contributing face card has their chips doubled.";
                break;
        }
        spriteRenderer.color = Color.grey;
    }

    private void Update()
    {
        if(Selected)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.grey;
        }
    }
}
