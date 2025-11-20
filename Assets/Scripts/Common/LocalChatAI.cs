using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text;

public class LocalChatAI : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputPregunta;
    public TextMeshProUGUI respuestaText;

    [Header("Ollama Local")]
    public string modelo = "llama3";

    public void EnviarPregunta()
    {
        string pregunta = inputPregunta.text.Trim();

        if (string.IsNullOrEmpty(pregunta))
        {
            respuestaText.text = "Por favor escribe una pregunta.";
            return;
        }

        respuestaText.text = "Pensando...";
        StartCoroutine(ConsultarIA(pregunta));
    }

    // --- Método para MRUExperiment ---
    public IEnumerator ObtenerRetroalimentacion(float tiempo, float velocidad)
    {
        string prompt =
            $"El estudiante realizó un experimento de MRU. Tiempo medido: {tiempo:F2} s, velocidad: {velocidad:F2} m/s. " +
            $"Genera una retroalimentación amable y educativa.";

        respuestaText.text = "Generando retroalimentación...";
        yield return ConsultarIA(prompt);
    }

    // --- CONSULTA CON STREAM ---
    public IEnumerator ConsultarIA(string prompt)
    {
        string escaped = prompt.Replace("\"", "\\\"");
        string json = "{\"model\": \"" + modelo + "\", \"prompt\": \"" + escaped + "\"}";

        using (UnityWebRequest req = new UnityWebRequest("http://172.20.10.2:11434/api/generate", "POST"))

        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            respuestaText.text = "";

            // Enviar petición
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                respuestaText.text = $"Error: {req.error}";
                yield break;
            }

            // Ollama retorna múltiples JSON -> debemos separarlos
            string raw = req.downloadHandler.text;
            string[] partes = raw.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            StringBuilder resultadoFinal = new StringBuilder();

            foreach (string fragmento in partes)
            {
                // Cada fragmento es un JSON individual
                int idx = fragmento.IndexOf("\"response\":");
                if (idx != -1)
                {
                    idx += "\"response\":".Length;
                    int end = fragmento.IndexOf("\"", idx + 2);
                    if (end == -1) continue;

                    string texto = fragmento.Substring(idx, end - idx)
                                    .Trim(':', '\"', ' ', '}');

                    resultadoFinal.Append(texto + " ");
                }
            }

            respuestaText.text = resultadoFinal.ToString().Trim();
        }
    }
}
