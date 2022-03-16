using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Library : MonoBehaviour
{
    public BookShelf bookShelf;
    public LoaderTime loadTime;
    public StoryElement storyElement;
    public CategoryElement categoryElement;
    public GameObject content;
    // Start is called before the first frame update
    public void showStory()
    {
        SceneManager.LoadScene("StoryBoard");
       
    }
    void OnEnable()
    {
      ///  Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
       
        string url = loadTime.RootUri+"packs/";
        Package pack = loadTime.Packages[UnityEngine.Random.Range(0, loadTime.Packages.Length)];
        string filename =pack.Id+".zip";
        //  Debug.Log("init1" + url + filename);
        StoryBoard.storyBoardInstance.nameLabel.text=""+ pack.Title;
        StoryBoard.storyBoardInstance.init(url+ filename, pack.Id);
    }
   
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
