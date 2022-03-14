using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Library : MonoBehaviour
{
    public BookShelf bookShelf;
    public LoaderTime loadTime;
    public StoryElement storyElement;
    public CategoryElement categoryElement;
    public GameObject content;
    // Start is called before the first frame update
    public void CreateLibrary(string bookShelfData, string loadTimeData)
    {
        bookShelf= JsonUtility.FromJson<BookShelf>(bookShelfData);
        loadTime = JsonUtility.FromJson<LoaderTime>(loadTimeData);
        List<CategoryElement> Allcategories = new List<CategoryElement>();
       
        foreach (Category category in bookShelf.Categories)
        {
            CategoryElement element = Instantiate(categoryElement, content.transform);
            element.init(category.Title, category.Id);
            Allcategories.Add(element);
        }

        string link = ""+ loadTime.RootUri;
        foreach (Story story in bookShelf.Stories)
        {       
            foreach (CategoryElement category in Allcategories)
            {
                if (Array.Exists(story.Categories, element => element == category.Id))
                {
                    StoryElement element = Instantiate(storyElement, category.storyParent.transform);
                    element.init(story.Title,  story.Id);
                }
            }
        }


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
