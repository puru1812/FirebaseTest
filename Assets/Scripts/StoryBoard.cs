using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using ICSharpCode.SharpZipLib.Zip;

public class StoryBoard : MonoBehaviour
{
    public TextMeshProUGUI nameLabel;
    public TextMeshProUGUI bytesLabel;
    public Slider progressBar;
    bool paused = true;
    float lastProgress = 0.0f;
    public static void UnZip(string filePath, byte[] data)
    {
        using (ZipInputStream s = new ZipInputStream(new MemoryStream(data)))
        {
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
#if UNITY_EDITOR
                Debug.LogFormat("Entry Name: {0}", theEntry.Name);
#endif

                string directoryName = Path.GetDirectoryName(theEntry.Name);
                string fileName = Path.GetFileName(theEntry.Name);

                // create directory
                if (directoryName.Length > 0)
                {
                    var dirPath = Path.Combine(filePath, directoryName);

#if UNITY_EDITOR
                    Debug.LogFormat("CreateDirectory: {0}", dirPath);
#endif

                    Directory.CreateDirectory(dirPath);
                }

                if (fileName != string.Empty)
                {
                    // retrieve directory name only from persistence data path.
                    var entryFilePath = Path.Combine(filePath, theEntry.Name);
                    using (FileStream streamWriter = File.Create(entryFilePath))
                    {
                        int size = 2048;
                        byte[] fdata = new byte[size];
                        while (true)
                        {
                            size = s.Read(fdata, 0, fdata.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(fdata, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            } //end of while
        } //end of using
    }
    public void togglePause()
    {
        paused = !paused;
    }
    public void init(string url,string filename)
    {
        Debug.Log("init"  + url );

        //  progressBar.value = 0;
        //  paused = false;

        // A correct website page.
        paused = false;
        StartCoroutine(GetRequest(url ,filename));
    }
    //Method for 9download progress
    public bool updateRequest(UnityWebRequestAsyncOperation operation, UnityWebRequest webRequest)
    {
        if (paused == true)
            return false;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        Debug.Log("got"+webRequest.GetResponseHeader("Content-Type"));
        if (webRequest.GetResponseHeader("Content-Length") != null)
        {
            bytesLabel.text = "Downloaded " + (webRequest.downloadedBytes / (1024 * 1024)) + " Kb out of " + (Int32.Parse(webRequest.GetResponseHeader("Content-Length")) / (1024 * 1024)) + " Kb";
        }

        float downloadDataProgress = operation.progress * 100;
        progressBar.value = downloadDataProgress / 100;

        float deltaProgress = operation.progress - lastProgress;
        float progressPerSec = deltaProgress / Time.deltaTime;
        float remaingTime = (1 - operation.progress) / progressPerSec;
        lastProgress = operation.progress;


        if (webRequest.GetResponseHeader("Content-Length") != null)
        {
            bytesLabel.text = "Downloaded " + (webRequest.downloadedBytes / (1024 * 1024)) + " Kb out of " + (Int32.Parse(webRequest.GetResponseHeader("Content-Length")) / (1024 * 1024)) + " Kb";
            bytesLabel.text = bytesLabel.text + " Remaining: " + Math.Floor(remaingTime) + " sec";
        }
        return operation.isDone;
   
    }
     IEnumerator GetRequest(string uri,string filename)
    {
        Debug.LogError("start");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
            yield return new WaitUntil(()=>updateRequest(operation,webRequest)==true);

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);

                StartCoroutine(GetRequest(uri, filename));
            }
            else
            {

                Debug.Log(webRequest.downloadHandler.text.Contains(filename));
                byte[] results = webRequest.downloadHandler.data;
                Debug.Log(results.Length);


                File.WriteAllBytes(Application.persistentDataPath + filename, results);
                bytesLabel.text = bytesLabel.text+ " Saved " + filename + " at " + Application.persistentDataPath;

                // Debug.Log(webRequest.downloadHandler.text + ":\nReceived: "+ webRequest.responseCode);
                //  UnZip(path, webRequest.downloadHandler.data);
            }
        }
    }

  
    private static StoryBoard instance;
    public static StoryBoard storyBoardInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StoryBoard>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(StoryBoard).Name;
                    instance = obj.AddComponent<StoryBoard>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as StoryBoard;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
