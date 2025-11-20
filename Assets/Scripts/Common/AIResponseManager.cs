using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class AIResponseManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI respuestaText; // Dónde se muestra la retroalimentación IA

    [Header("Configuración Hugging Face")]
    [Tooltip("Tu token de Hugging Face (gratis desde huggingface.co/settings/tokens)")]
    public string huggingFaceToken = "hf_tu_token_aqui";

    //  Nuevo endpoint obligatorio (actualizado 2025)
    private string apiURL = "https://router.huggingface.co/hf-inference/models/<modelo>";


    public IEnumerator ObtenerRetroalimentacion(float tiempo, float velocidad)
    {
        respuestaText.text = " Analizando resultados...";

        string prompt = $"El estudiante realizó un experimento de movimiento rectilíneo uniforme (MRU). " +
                        $"El tiempo medido fue {tiempo:F2} segundos y la velocidad fue {velocidad:F2} m/s. " +
                        $"Genera una retroalimentación corta, amable y educativa sobre el resultado.";

        using (UnityWebRequest request = new UnityWebRequest(apiURL, "POST"))
        {
            string jsonBody = "{\"inputs\": \"" + prompt + "\"}";
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + huggingFaceToken);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                string respuesta = ExtraerTexto(json);
                respuestaText.text = respuesta;
            }
            else
            {
                respuestaText.text = " Error al conectar con la IA: " + request.responseCode;
                Debug.LogError($"[AI] Error HTTP {request.responseCode}: {request.downloadHandler.text}");
            }
        }
    }

    private string ExtraerTexto(string json)
    {
        int idx = json.IndexOf("\"generated_text\":");
        if (idx == -1) return "No se recibió respuesta válida de la IA.";
        idx += "\"generated_text\":".Length;
        int end = json.IndexOf("\"", idx + 2);
        if (end == -1) end = json.Length - 1;
        string texto = json.Substring(idx, end - idx).Trim(':', '\"', ' ', '}', ']');
        return texto;
    }
}
