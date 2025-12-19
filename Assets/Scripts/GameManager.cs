using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class GameManager : Singleton<GameManager>
{
    
    
    public List<GameObject> allEntities =  new();


    private void Awake()
    {
        if (Instance != this)
            Destroy(this);
    }
}