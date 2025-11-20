using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;

public class ChatGPTClient : MonoBehaviour
{
    [Header("Configuración")]
    public string openAI_API_Key = "TU_API_KEY_AQUI";
    public string model = "gpt-4o-mini"; // o "gpt-4o", según tus créditos

    [Header("UI Opcional")]
    public TMP_InputField userInput;
    public TextMeshProUGUI chatOutput;

    public void EnviarPregunta()
    {
        string prompt = userInput != null ? userInput.text : "Explícame qué es MRU";
        StartCoroutine(EnviarMensaje(prompt));
    }

    IEnumerator EnviarMensaje(string prompt)
    {
        string endpoint = "https://api.openai.com/v1/chat/completions";

        // Construimos el JSON manualmente, bien formado
        string json = "{\"model\":\"" + model + "\",\"messages\":["
            + "{\"role\":\"system\",\"content\":\"Eres un asistente de laboratorio virtual que explica resultados de experimentos de física de forma breve y amigable.\"},"
            + "{\"role\":\"user\",\"content\":\"" + prompt.Replace("\"", "\\\"") + "\"}"
            + "],\"max_tokens\":150}";

        var request = new UnityWebRequest(endpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + openAI_API_Key);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error + "\n" + request.downloadHandler.text);
            if (chatOutput) chatOutput.text = "Error: " + request.error + "\n" + request.downloadHandler.text;
        }
        else
        {
            string result = request.downloadHandler.text;
            string answer = ExtraerTexto(result);
            if (chatOutput) chatOutput.text = answer;
            Debug.Log("ChatGPT: " + answer);
        }
    }


    string ExtraerTexto(string json)
    {
        // Muy simple parser sin JSON library extra
        int index = json.IndexOf("\"content\":");
        if (index == -1) return "(sin respuesta)";
        int start = json.IndexOf("\"", index + 10) + 1;
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start).Replace("\\n", "\n");
    }
}
