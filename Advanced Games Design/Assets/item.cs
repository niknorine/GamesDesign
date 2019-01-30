using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class item : MonoBehaviour {
    [System.Serializable]
    public class Items{
        public int ID;
        public string Name;
        public string Desc;
        public GameObject Asset;
        public Sprite ui_Image;

        
    }

    public Items Item;
    public bool thisIsAnItem;

    void Start()
    {

    }

    void Update()
    {
        if (!thisIsAnItem)
        {
            Image iconImage = GetComponentInChildren<Image>();
            iconImage.sprite = Item.ui_Image;
        }



    }
}
