using UnityEngine;
using TMPro;
using System.Collections;

public class MessagePanel : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text messageText;

    public void ShowMessage(string msg, float duration = 2f)
    {
        messageText.text = msg;
        panel.SetActive(true);
        StartCoroutine(HideAfter(duration));
    }

    IEnumerator HideAfter(float t)
    {
        yield return new WaitForSeconds(t);
        panel.SetActive(false);
    }
}
