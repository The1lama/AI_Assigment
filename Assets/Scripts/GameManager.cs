using System;
using System.Collections.Generic;
using Common.Interfaces;
using Factory;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

public class GameManager : Singleton<GameManager>
{
    public List<GameObject> allEntities =  new();
    private InputAction hurtAction;

    private void Awake()
    {
        if (Instance != this)
            Destroy(this);
    }

    private void OnEnable()
    {
        hurtAction = new InputAction(
                    name: "DamageAction",
                    type: InputActionType.Button,
                    binding: "<Keyboard>/p"
                    );
        hurtAction.performed += HoldActionOnperformed;
        hurtAction.Enable();

    }

    private void HoldActionOnperformed(InputAction.CallbackContext obj)
    {
        foreach (var entity in allEntities)
        {
            if(entity.GetComponent<CharacterFactory>() == null) continue;
            
            entity.GetComponent<CharacterFactory>().TakeDamage(10f);
        }
    }
}