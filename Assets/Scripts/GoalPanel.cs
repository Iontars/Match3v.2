using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    public Image thisImage;
    public Sprite thisSprite;
    public Text thisText;
    public string thisString;

    void Setup()
    {
        thisImage.sprite = thisSprite;
        thisText.text = thisString;
    }
   
    void Start()
    {
        Setup();
    }

    
    void Update()
    {
        
    }
}
