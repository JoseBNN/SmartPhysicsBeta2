using UnityEngine;
using UnityEngine.UI;

public class ResultsManager : MonoBehaviour
{
    [Header("UI")]
    public Image[] stars; // Ahora son Images para cambiar color
    public string levelName = "Level1"; // Cambiar según el nivel que quieras mostrar

    [Header("Colores de estrellas")]
    public Color starRed = Color.red;
    public Color starYellow = Color.yellow;
    public Color starGreen = Color.green;

    void Start()
    {
        string levelKey = "QuizStars_" + levelName;
        int earnedStars = PlayerPrefs.GetInt(levelKey, 0);
        ShowStars(earnedStars);
    }

    void ShowStars(int count)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < count)
            {
                stars[i].gameObject.SetActive(true);

                // Color según cantidad de estrellas obtenidas
                if (count == 3)
                    stars[i].color = starGreen;
                else if (count == 2)
                    stars[i].color = starYellow;
                else if (count == 1)
                    stars[i].color = starRed;
            }
            else
            {
                stars[i].gameObject.SetActive(false);
            }
        }
    }
}
