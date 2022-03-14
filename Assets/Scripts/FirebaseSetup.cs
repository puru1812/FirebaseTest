using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Extensions;
using Firebase;
public class FirebaseSetup : MonoBehaviour
{
    protected bool isFirebaseInitialized = false;
    public Library library;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    // Use this for initialization
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
               InitFirebase();
            }
            else
            {
                Debug.Log("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    void InitFirebase()
    {
        System.Collections.Generic.Dictionary<string, object> defaults =
          new System.Collections.Generic.Dictionary<string, object>();
        
        defaults.Add("BookShelf", "{\"Categories\":[{\"Id\":\"d4974e57 - 4bb9 - 859a - 2140 - 6ad0a304e5e9\",\"Title\":{\"en\":\"Casual\",\"hi\":\"डरावनी\"}}],\"Stories\":[{\"Id\":\"6kT2ZffecC29yQkdGc2N\",\"Title\":{\"en\":\"Jsut Dost\",\"hi\":\"घोस्ट दोस्त\"},\"Categories\":[\"87cab99c - eb2a - 40bf - 9572 - 6e50e7e3d572\"]}]}");
        defaults.Add("LoadTime", "{\"RootUri\":\"https://storage.googleapis.com/kbp/\",\"Packages\":[{\"Id\":\"bSwarFS8Eet7HfeSAfm2\",\"Title\":\"Default\"}]}");

        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
          .ContinueWithOnMainThread(task => {
            Debug.Log("RemoteConfig configured and ready!");
              isFirebaseInitialized = true;
              FetchDataAsync();
          });
    }
   
    public void FetchFireBase()
    {
        FetchDataAsync();
    }

    public void ShowData()
    {
        Debug.Log("Current Data is:");
        Debug.Log("BookShelf: " +
                 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                 .GetValue("BookShelf").StringValue);
     
        Debug.Log("Loadtime: " +
         Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
         .GetValue("LoadTime").StringValue);
    }

    public void DisplayAllKeys()
    {
        Debug.Log("All Keys Are:");
        System.Collections.Generic.IEnumerable<string> keys =
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Keys;
        foreach (string key in keys)
        {
            Debug.Log("    " + key);
        }
 
    }
    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        System.Threading.Tasks.Task fetchTask =
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    public void InitializeLibrary()
    {
        library.CreateLibrary(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("BookShelf").StringValue, Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("LoadTime").StringValue);
 
    }

    void FetchComplete(Task fetchTask)
    {
        if(fetchTask.IsCanceled) {
            Debug.Log("Fetch canceled.");
        } else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
            InitializeLibrary();
            ShowData();
        }

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                .ContinueWithOnMainThread(task => {
                    Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                                   info.FetchTime));
                });

                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
    }
}