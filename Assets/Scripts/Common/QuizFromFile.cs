using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class QuizFromFile : MonoBehaviour
{
    [Header("Nivel")]
    public string levelName = "Level1"; // Cambiar según el nivel

    [Header("UI")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public GameObject[] starIcons;
    public GameObject PanelResultados;

    [Header("Colores")]
    public Color colorCorrecto = Color.green;
    public Color colorIncorrecto = Color.red;
    public Color colorNormal = Color.white;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;
    public AudioClip sonidoPopStar;

    [Header("Vibración (solo Android)")]
    public bool vibrarEnIncorrecto = true;
    public bool vibrarEnCorrecto = false;

    [Header("Retroalimentación")]
    public Transform feedbackContent;
    public GameObject feedbackPrefab;

    [Header("Retroalimentación Panel")]
    public GameObject feedbackPanel;
    public Transform feedbackPanelContent;

    [Header("UI Extra")]
    public Button retryButton;

    [Header("Config")]
    public string fileName = "banco_preguntas_cinematica.txt";
    public int totalQuestionsToPlay = 3;

    [Header("Botón Continuar")]
    public Button continueButton;
    public string nextSceneName = "NombreDeTuEscena";

    private List<QuestionData> allQuestions = new List<QuestionData>();
    private List<QuestionData> selectedQuestions = new List<QuestionData>();
    private int currentQuestionIndex = 0;
    private int score = 0;

    void Start()
    {
        QuizFeedbackData.items.Clear();
        ResetStars();
        StartCoroutine(InitQuiz());
    }

    IEnumerator InitQuiz()
    {
        LoadQuestions();
        yield return new WaitUntil(() => allQuestions.Count > 0);
        PickRandomQuestions();
        ShowQuestion();
    }

    void ResetStars()
    {
        foreach (var star in starIcons)
        {
            if (star == null) continue;
            star.SetActive(true);
            Image img = star.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
            star.transform.localScale = Vector3.one * 0.01f;
        }
    }

    void LoadQuestions()
    {
        allQuestions.Clear();
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(LoadFromStreamingAssets(path));
#else
        if (!File.Exists(path))
        {
            Debug.LogError("❌ Archivo no encontrado: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);
        ParseLines(lines);
#endif
    }

    IEnumerator LoadFromStreamingAssets(string path)
    {
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Error al leer en Android: " + www.error);
                yield break;
            }
            string[] lines = www.downloadHandler.text.Split('\n');
            ParseLines(lines);
        }
    }

    void ParseLines(string[] lines)
    {
        foreach (string rawLine in lines)
        {
            if (string.IsNullOrWhiteSpace(rawLine)) continue;
            if (!rawLine.Contains("|") || rawLine.Split('|').Length < 6) continue;

            string[] data = rawLine.Split('|');
            int.TryParse(data[5].Trim(), out int idx);

            allQuestions.Add(new QuestionData
            {
                question = data[0].Trim(),
                options = new string[] { data[1].Trim(), data[2].Trim(), data[3].Trim(), data[4].Trim() },
                correctIndex = Mathf.Clamp(idx, 0, 3)
            });
        }
    }

    void PickRandomQuestions()
    {
        int take = Mathf.Min(totalQuestionsToPlay, allQuestions.Count);
        selectedQuestions = allQuestions.OrderBy(x => Random.value).Take(take).ToList();
        currentQuestionIndex = 0;
        score = 0;
    }

    public void OnAnswerSelected(int index)
    {
        foreach (var btn in answerButtons)
            btn.interactable = false;

        var q = selectedQuestions[currentQuestionIndex];
        bool correct = (index == q.correctIndex);

        if (correct)
        {
            if (sonidoCorrecto != null) audioSource.PlayOneShot(sonidoCorrecto);
            if (vibrarEnCorrecto) Vibrar(50);
        }
        else
        {
            if (sonidoIncorrecto != null) audioSource.PlayOneShot(sonidoIncorrecto);
            if (vibrarEnIncorrecto) Vibrar(120);
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Image img = answerButtons[i].GetComponent<Image>();
            if (i == q.correctIndex)
                img.color = colorCorrecto;
            else if (i == index)
                img.color = colorIncorrecto;
            else
                img.color = colorNormal;
        }

        // ================================
        // ✔ RETROALIMENTACIÓN AVANZADA
        // ================================

        var dic = FeedbackData.Get();

        string explanation = "";
        if (dic.ContainsKey(q.question))
        {
            explanation = correct ? dic[q.question].correct : dic[q.question].incorrect;
        }
        else
        {
            explanation = correct ? "✔ Muy bien." : "✘ Incorrecto.";
        }

        QuizFeedbackData.items.Add(new FeedbackItem
        {
            question = q.question,
            selectedAnswer = q.options[index],
            correctAnswer = q.options[q.correctIndex],
            isCorrect = correct,
            explanation = explanation
        });

        if (correct) score++;

        StartCoroutine(NextQuestion());
    }

    IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(0.4f);
        currentQuestionIndex++;
        if (currentQuestionIndex < selectedQuestions.Count)
            ShowQuestion();
        else
            StartCoroutine(ShowResultsAnimated());
    }

    void ShowQuestion()
    {
        var q = selectedQuestions[currentQuestionIndex];
        questionText.text = q.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Image img = answerButtons[i].GetComponent<Image>();
            img.color = colorNormal;

            var label = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            answerButtons[i].interactable = true;

            if (i < q.options.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                label.text = q.options[i];
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    IEnumerator ShowResultsAnimated()
    {
        PanelResultados.SetActive(true);

        foreach (var btn in answerButtons)
        {
            btn.interactable = false;
            btn.gameObject.SetActive(false);
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.05f);

        switch (score)
        {
            case 3:
                questionText.text = "¡Excelente trabajo! Has dominado esta práctica de Cinemática. Sigue así, tu comprensión es sobresaliente.";
                break;
            case 2:
                questionText.text = "Estás cerca de dominar la cinemática. ¿Te animas a intentarlo de nuevo y ganar la tercera estrella?";
                break;
            case 1:
                questionText.text = "¡Una estrella hoy, pero muchas más mañana! Lee la retroalimentación y ajusta los pasos de la práctica.";
                break;
            default:
                questionText.text = "¡No te desanimes! Esta es una gran oportunidad para mejorar. Revisa tus pasos y vuelve a intentarlo.";
                break;
        }

        PlayerPrefs.SetInt("QuizStars_" + levelName, score);
        PlayerPrefs.Save();

        for (int i = 0; i < starIcons.Length; i++)
        {
            var star = starIcons[i];

            if (star == null) continue;

            // Mostrar solo las estrellas ganadas
            bool earned = i < score;
            star.SetActive(earned);

            if (!earned) continue; // No animar las estrellas no ganadas

            // Color UNIFICADO SEGÚN SCORE
            Color colorEstrella = Color.white;

            if (score == 3) colorEstrella = Color.green;
            else if (score == 2) colorEstrella = Color.yellow;
            else if (score == 1) colorEstrella = Color.red;

            // Animación
            StarAnimator anim = star.GetComponent<StarAnimator>();
            if (anim != null)
                yield return StartCoroutine(anim.PlayStar(colorEstrella));

            if (sonidoPopStar != null)
                audioSource.PlayOneShot(sonidoPopStar);

            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1.5f);

        feedbackPanel.SetActive(true);

        foreach (Transform child in feedbackPanelContent)
            Destroy(child.gameObject);

        var sortedItems = QuizFeedbackData.items.OrderBy(x => x.isCorrect).ToList();

        foreach (var item in sortedItems)
        {
            GameObject obj = Instantiate(feedbackPrefab, feedbackPanelContent);

            Transform panelTransform = obj.transform.Find("Panel");
            if (panelTransform != null)
            {
                Image panelImage = panelTransform.GetComponent<Image>();
                if (panelImage != null)
                    panelImage.color = item.isCorrect ? Color.green : Color.red;
            }

            TextMeshProUGUI txt = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = $"<b>Pregunta:</b> {item.question}\n<b>Tu respuesta:</b> {item.selectedAnswer}";
                if (!item.isCorrect)
                    txt.text += $"\n<b>Respuesta correcta:</b> {item.correctAnswer}";
                txt.text += $"\n<i>{item.explanation}</i>";
            }
        }

        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(true);
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(() =>
            {
                retryButton.gameObject.SetActive(false);
                feedbackPanel.SetActive(false);
                ResetQuiz();
            });
        }

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(nextSceneName);
            });
        }
    }

    void ResetQuiz()
    {
        foreach (var btn in answerButtons)
        {
            var img = btn.GetComponent<Image>();
            if (img != null) img.color = Color.white;
            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.color = Color.black;
            btn.interactable = true;
        }

        PanelResultados.SetActive(false);
        QuizFeedbackData.items.Clear();
        score = 0;
        currentQuestionIndex = 0;
        ResetStars();
        StartCoroutine(InitQuiz());
    }

    void Vibrar(long ms)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            if (vibrator != null) vibrator.Call("vibrate", ms);
        }
        catch { Debug.LogWarning("No se pudo activar la vibración."); }
#else
        Debug.Log("Vibración simulada: " + ms + "ms");
#endif
    }
}

[System.Serializable]
public class QuestionData
{
    public string question;
    public string[] options;
    public int correctIndex;
}

public static class QuizFeedbackData
{
    public static List<FeedbackItem> items = new List<FeedbackItem>();
}

public class FeedbackItem
{
    public string question;
    public string selectedAnswer;
    public string correctAnswer;
    public bool isCorrect;
    public string explanation;
}

// ===========================
// 🔥 DICCIONARIO DE FEEDBACK
// ===========================

public static class FeedbackData
{
    public static Dictionary<string, (string correct, string incorrect)> Get()
    {
        return new Dictionary<string, (string correct, string incorrect)>
        {
            {
                "¿Qué magnitud permanece constante en el MRU?",
                (
                    "Correcto. En el MRU la velocidad permanece constante porque no existe aceleración que modifique su movimiento.",
                    "Incorrecto. La magnitud que no cambia en el MRU es la velocidad, ya que la aceleración es cero."
                )
            },
            {
                "En el MRU, ¿qué relación cumple la distancia con el tiempo?",
                (
                    "Muy bien. La distancia aumenta al mismo ritmo que el tiempo porque la velocidad no cambia.",
                    "Incorrecto. En el MRU la distancia crece proporcionalmente al tiempo debido a la velocidad constante."
                )
            },
            {
                "Si un cuerpo recorre 20 m en 5 s, su velocidad es:",
                (
                    "Correcto. Velocidad = distancia / tiempo = 20 ÷ 5 = 4 m/s.",
                    "Incorrecto. La velocidad se calcula dividiendo distancia entre tiempo: 20 ÷ 5 = 4 m/s."
                )
            },
            {
                "¿Qué magnitud cambia cuando hay aceleración?",
                (
                    "Exacto. La aceleración mide cómo cambia la velocidad en el tiempo.",
                    "Incorrecto. La aceleración implica cambio en la velocidad, no en la masa o el tiempo."
                )
            },
            {
                "¿Cuál es la unidad de velocidad en el SI?",
                (
                    "Muy bien. La velocidad en el SI se mide en metros por segundo (m/s).",
                    "Incorrecto. La unidad correcta es metros por segundo (m/s)."
                )
            },
            {
                "Si lanzas un objeto hacia arriba, ¿qué ocurre con su velocidad en el punto más alto?",
                (
                    "Correcto. En el punto más alto la velocidad es cero porque el objeto cambia de dirección.",
                    "Incorrecto. En la parte más alta, la velocidad se detiene por un instante antes de caer."
                )
            },
            {
                "¿Qué tipo de movimiento ocurre cuando la aceleración es cero?",
                (
                    "Muy bien. Sin aceleración no hay cambio de velocidad, por lo que es un MRU.",
                    "Incorrecto. Cuando la aceleración es cero, el movimiento es rectilíneo uniforme."
                )
            },
            {
                "La aceleración de la gravedad cerca de la Tierra es…",
                (
                    "Correcto. La gravedad terrestre tiene un valor aproximado de 9.8 m/s².",
                    "Incorrecto. El valor estándar es 9.8 m/s²."
                )
            },
            {
                "El área bajo la curva velocidad-tiempo indica…",
                (
                    "Correcto. El área bajo la gráfica v–t representa el desplazamiento recorrido.",
                    "Incorrecto. Esta área corresponde al desplazamiento, no al tiempo o la energía."
                )
            },
            {
                "En un lanzamiento horizontal, la velocidad vertical inicial es…",
                (
                    "Muy bien. La velocidad vertical inicial es 0 m/s porque el movimiento comienza solo con velocidad horizontal.",
                    "Incorrecto. En un lanzamiento horizontal no hay velocidad inicial hacia arriba o abajo: es 0 m/s."
                )
            }
        };
    }
}
