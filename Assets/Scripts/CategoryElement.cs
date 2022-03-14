using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class CategoryElement : MonoBehaviour
{
    public GameObject storyParent;
   public TextMeshProUGUI hindiLabel;
    public TextMeshProUGUI englishLabel;
    public string Id = "";
    public  void init(Title title, string id)
    {
        englishLabel.text = "" + title.en ;
        hindiLabel.text = ""  + title.hi;
        Id = id;
    }


}
