using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginHandler : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public PopupManager popupManager;

    [Header("API URL (ejemplo: http://TU_IP:3000/login)")]
    public string API = "http://172.20.10.2:3000/login";

    [Header("Escena a cargar si el login es exitoso")]
    public string nextScene = "MenuPrincipal";

    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool success;
        public string message;
    }

    public void OnLoginButton()
    {
        StartCoroutine(LoginRequest());
    }

    IEnumerator LoginRequest()
    {
        LoginData data = new LoginData
        {
            username = usernameInput.text,
            password = passwordInput.text
        };

        string json = JsonUtility.ToJson(data);
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(API, "POST");
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            popupManager.ShowPopup("❌ Error de conexión");
            yield break;
        }

        LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

        popupManager.ShowPopup(response.message);

        // 🚀 SI EL LOGIN ES EXITOSO → CAMBIAR ESCENA
        if (response.success)
        {
            yield return new WaitForSeconds(1f); // Pequeña pausa para que se vea el mensaje
            SceneManager.LoadScene(nextScene);
        }
    }
}
