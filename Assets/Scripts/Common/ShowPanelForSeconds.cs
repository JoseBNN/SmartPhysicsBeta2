using UnityEngine;
using System.Collections;

public class ShowPanelForSeconds : MonoBehaviour
{
    public GameObject panel;
    public float duration = 2f; // tiempo en segundos

    public void ShowPanel()
    {
        StartCoroutine(ShowAndHide());
    }

    IEnumerator ShowAndHide()
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(duration);
        panel.SetActive(false);
    }
}
