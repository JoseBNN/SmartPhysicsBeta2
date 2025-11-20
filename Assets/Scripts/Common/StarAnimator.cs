using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarAnimator : MonoBehaviour
{
    public float popScale = 1.4f;     
    public float animDuration = 0.25f;
    public AudioClip popSound;
    public AudioSource audioSource;    // Asignar desde inspector

    private Vector3 originalScale;
    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        originalScale = transform.localScale;

        if (img != null)
            img.enabled = false; // Inicialmente invisible

        transform.localScale = originalScale * 0.01f; // escala mínima

        // Desactivar cualquier Animator que pueda interferir
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;
    }

    public IEnumerator PlayStar(Color color)
    {
        if (img == null) yield break;

        img.enabled = true;
        img.color = color;

        // Empezar desde escala muy pequeña
        transform.localScale = originalScale * 0.01f;

        float t = 0f;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0.01f, popScale, t / animDuration);
            transform.localScale = originalScale * s;
            yield return null;
        }

        t = 0f;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(popScale, 1f, t / animDuration);
            transform.localScale = originalScale * s;
            yield return null;
        }

        if (audioSource != null && popSound != null)
            audioSource.PlayOneShot(popSound);
    }
}