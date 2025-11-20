using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarController : MonoBehaviour
{
    public GameObject[] starIcons; // GameObjects con Image + StarAnimator
    public Color colorActive = Color.yellow;

    public void ActivateStars(int score)
    {
        for (int i = 0; i < starIcons.Length; i++)
        {
            if (starIcons[i] == null) continue;

            // Activar GameObject
            starIcons[i].SetActive(true);

            // Determinar color individual
            Color color = (i < score) ? Color.green : Color.red;

            // Activar animación
            StarAnimator anim = starIcons[i].GetComponent<StarAnimator>();
            if (anim != null)
            {
                StartCoroutine(anim.PlayStar(color));
            }
            else
            {
                // Por si no tiene StarAnimator
                Image img = starIcons[i].GetComponent<Image>();
                if (img != null)
                {
                    img.enabled = true;
                    img.color = color;
                }
            }
        }
    }
}