using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Connecting : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text message;
    private int display = 0;
    private int display2 = 0;

    public Sprite glitch0;
    public Sprite glitch1;
    public Sprite glitch2;
    public Sprite glitch3;
    public Image image;

    public GameObject failedConnectionMenu;

    private const float TIME = 0.1f;
    private float time;

    // Coroutine
    Coroutine glitchTimer = null;
    Coroutine textTimer = null;

    void OnEnable()
    {
        glitchTimer = StartCoroutine(GlitchCourotine(TIME));
        textTimer = StartCoroutine(TextCourotine(1));

        time = Time.time;
    }

    IEnumerator GlitchCourotine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        display++;
        display = display % 4;
        glitchTimer = StartCoroutine(GlitchCourotine(TIME));
    }
    
    IEnumerator TextCourotine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        display2++;
        display2 = display2 % 4;
        textTimer = StartCoroutine(TextCourotine(1));
    }

    private void OnDisable()
    {
        StopCoroutine(glitchTimer);
        StopCoroutine(textTimer);
    }

    private void Update()
    {
        if (Time.time - time > 15)
        {
            this.gameObject.SetActive(false);
            failedConnectionMenu.SetActive(true);
        }
        switch (display)
        {
            case 0:
                image.sprite = glitch0;
                break;
            case 1:
                image.sprite = glitch1;
                break;
            case 2:
                image.sprite = glitch2;
                break;
            case 3:
                image.sprite = glitch3;
                break;
        }
        switch (display2)
        {
            case 0:
                message.text = "Connecting to Void TV\n ";
                break;
            case 1:
                message.text = "Connecting to Void TV\n.";
                break;
            case 2:
                message.text = "Connecting to Void TV\n..";
                break;
            case 3:
                message.text = "Connecting to Void TV\n...";
                break;
        }
    }
}
