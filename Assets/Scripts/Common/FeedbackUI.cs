using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour
{
    public Transform contentParent;   // El layout donde se agregan filas
    public GameObject feedbackPrefab; // Prefab con color + textos

    void Start()
    {
        LoadFeedback();
    }

    void LoadFeedback()
    {
        foreach (var item in QuizFeedbackData.items)
        {
            GameObject obj = Instantiate(feedbackPrefab, contentParent);

            TMP_Text qText = obj.transform.Find("QuestionText").GetComponent<TMP_Text>();
            TMP_Text resultText = obj.transform.Find("ResultText").GetComponent<TMP_Text>();
            Image bg = obj.GetComponent<Image>();

            qText.text = item.question;

            if (item.isCorrect)
            {
                resultText.text = $"Correcto \nRespuesta: {item.correctAnswer}";
                bg.color = new Color32(150, 255, 110, 255); // verde
            }
            else
            {
                resultText.text = $"Incorrecto \nTu respuesta: {item.selectedAnswer}\nCorrecta: {item.correctAnswer}";
                bg.color = new Color32(255, 100, 100, 255); // rojo
            }
        }
    }
}
