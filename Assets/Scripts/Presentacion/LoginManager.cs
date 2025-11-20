using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public IEnumerator Login(string user, string pass)
    {
        string url = "http://11.11.25.64:3000//login";

        LoginData data = new LoginData();
        data.username = user;
        data.password = pass;

        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("Login: " + request.downloadHandler.text);
        else
            Debug.Log(" ERROR: " + request.error);
    }

    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
    }
}
