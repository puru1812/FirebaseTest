using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class StoryElement : MonoBehaviour
{
    public TextMeshProUGUI nameLabel;
    public Image icon;

    public void init(Title name, string id)
    {
        nameLabel.text = "" + name.en;
   
        StartCoroutine(initializeImage("https://storage.googleapis.com/kbp/content/posters/"+id+ "/bookshelf.png"));
    }
    IEnumerator initializeImage(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        icon.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }
    

}
