using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FeedbackPanel : MonoBehaviour
{
    public Transform contentParent;   // El contenedor vertical donde se colocan los ítems
    public GameObject feedbackPrefab; // Prefab de cada fila

    public void AddFeedback(int questionNumber, bool correct, string explanation)
    {
        // Crear item visual
        GameObject item = Instantiate(feedbackPrefab, contentParent);

        // Obtener componentes
        TMP_Text numberText = item.transform.Find("NumberText").GetComponent<TMP_Text>();
        TMP_Text messageText = item.transform.Find("FeedbackText").GetComponent<TMP_Text>();
        Image bg = item.transform.Find("Background").GetComponent<Image>();

        // Llenar info
        numberText.text = questionNumber + ".";
        messageText.text = explanation;

        // Color
        bg.color = correct ? new Color32(150, 255, 110, 255) : new Color32(255, 80, 80, 255);
    }

    public void ClearFeedback()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
    }
}
