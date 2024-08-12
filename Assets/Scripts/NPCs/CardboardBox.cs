using Interfaces;
using UnityEngine;

public class CardboardBox : MonoBehaviour, IDamagable
{
    public SpriteRenderer spriteR;
    public Sprite[] states;
    public uint current_state;


    [EasyButtons.Button]
    private void Reset()
    {
        current_state = 0;
        spriteR.sprite = states[current_state];
    }


    [EasyButtons.Button]
    public void NextState()
    {
        current_state = (uint)Mathf.Clamp(current_state + 1, 0, states.Length);
        if (current_state == states.Length)
        {
            OnFolded();
        }
        else
        {
            spriteR.sprite = states[current_state];
        }
    }

    private void OnFolded()
    {
        Destroy(gameObject);
    }

    public void Damage(float damage)
    {
        Debug.Log("TEST");
        NextState();
    }
}