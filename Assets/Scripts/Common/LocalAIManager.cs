using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text;

public class LocalAIManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputPregunta;
    public TextMeshProUGUI respuestaText;

    [Header("Modelo local (Ollama)")]
    public string modelo = "llama3";
    public int timeout = 120;        // segundos
    public int maxTokens = 512;

    public void EnviarPregunta()
    {
        string pregunta = inputPregunta.text.Trim();

        if (pregunta.Length < 2)
        {
            respuestaText.text = "Escribe una pregunta primero.";
            return;
        }

        respuestaText.text = "Pensando... ";

        StartCoroutine(PedirRespuesta(pregunta));
    }

    IEnumerator PedirRespuesta(string pregunta)
    {
        string json = $"{{\"model\":\"{modelo}\",\"prompt\":\"{pregunta.Replace("\"", "\\\"")}\",\"stream\":false,\"max_tokens\":{maxTokens}}}";

        UnityWebRequest req = new UnityWebRequest("http://172.20.10.2:11434/api/generate", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.timeout = timeout;

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            respuestaText.text = "Error: " + req.error;
            yield break;
        }

        string data = req.downloadHandler.text;
        Debug.Log("[IA COMPLETA] " + data);

        // Extraer texto del campo "response"
        string respuesta = ExtraerRespuesta(data);

        if (respuesta == null)
            respuestaText.text = "No pude leer la respuesta.";
        else
            respuestaText.text = respuesta;
    }

    private string ExtraerRespuesta(string json)
    {
        string key = "\"response\":";
        int idx = json.IndexOf(key);
        if (idx == -1) return null;

        idx += key.Length;

        // Encuentra comilla inicial
        int start = json.IndexOf("\"", idx);
        if (start == -1) return null;

        // Extraer hasta comilla final manejando escapes
        StringBuilder sb = new StringBuilder();
        bool escaping = false;

        for (int i = start + 1; i < json.Length; i++)
        {
            char c = json[i];

            if (escaping)
            {
                if (c == 'n') sb.Append('\n');
                else sb.Append(c);
                escaping = false;
            }
            else
            {
                if (c == '\\') { escaping = true; continue; }
                if (c == '"') break;
                sb.Append(c);
            }
        }

        return sb.ToString().Trim();
    }
}
