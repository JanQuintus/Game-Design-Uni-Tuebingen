using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCollectable : AInteractive
{
    private enum FoodType
    {
        DRINK,
        EAT
    }

    [SerializeField] private FoodType foodType;
    [SerializeField] private AudioClip pickUpClip;

    private bool _pickedUp = false;

    public override void Interact(bool isRelease)
    {
        _pickedUp = true;
        SoundController.Instance.PlaySoundAtLocation(pickUpClip, transform.position);
        if (foodType == FoodType.EAT)
            GameManager.Food++;
        else
            GameManager.Drinks++;
        Destroy(GetComponent<Collider>());
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    public override string GetText()
    {
        return "Take " + (foodType == FoodType.DRINK ? "Water" : "Food");
    }

    public object CaptureState()
    {
        return _pickedUp;
    }

    public void RestoreState(object state)
    {
        _pickedUp = (bool)state;
        if (_pickedUp)
        {
            Destroy(GetComponent<Collider>());
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}
