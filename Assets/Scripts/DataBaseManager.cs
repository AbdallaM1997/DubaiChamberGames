using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;


public class DataBaseManager : MonoBehaviour
{
    [System.Serializable]
    public class Root
    {
        public string id;
        public string name;
        public string score;
    }

    public class RootList
    {
        public Root[] root;
    }


    [Header("Input Fields Data ")]
    [SerializeField] private TMP_InputField nameInput;
    [Header("Leaderboard Data")]
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private Transform rowsParent;

    GameManager gameManager;
    private RootList myRootList =  new RootList();
    private const string DATABASE_URL = "https://risebydubaichambers.com/service.php?game=1&";


    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }
    public void SendPostRequest()
    {
        StartCoroutine(SendPR());
    }
    [ContextMenu("TestGetLeaderBoard")]
    public void GetPostRequest()
    {
        StartCoroutine(GetRequest());
    }
    IEnumerator SendPR()
    {
        string post_url = DATABASE_URL + "name=" + nameInput.text + "&score=" + gameManager.Score.ToString() + "&setScore=1";
        print(post_url);

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }
        else
        {
            print(hs_post.text);
        }
    }

    IEnumerator GetRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(DATABASE_URL+ "getLeaderboard=1"))
        {
            foreach (Transform item in rowsParent)
            {
                Destroy(item.gameObject);
            }
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(String.Format("Something went wrong: {0}", webRequest.error));
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(webRequest.downloadHandler.text);
                    myRootList = JsonUtility.FromJson<RootList>("{\"root\":" + webRequest.downloadHandler.text + "}");
                    foreach (var item in myRootList.root)
                    {
                        GameObject newGo = Instantiate(rowPrefab, rowsParent);
                        TextMeshProUGUI[] texts = newGo.GetComponentsInChildren<TextMeshProUGUI>();
                        texts[0].text = item.id;
                        texts[1].text = item.name;
                        texts[3].text = item.score;
                    }
                    break;

            }
        }
    }
}