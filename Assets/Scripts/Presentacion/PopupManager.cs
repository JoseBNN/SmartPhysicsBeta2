using UnityEngine;
using TMPro;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text popupText;

    public void ShowPopup(string message)
    {
        popupText.text = message;
        panel.SetActive(true);
        StartCoroutine(HidePopup());
    }

    IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
    }
}
