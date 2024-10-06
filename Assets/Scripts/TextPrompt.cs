using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextPrompt : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public float displayTime = 3f;
    public float fadeTime = 1f;

    private void Start()
    {
        SetTextAlpha(0);
        // promptText.text = "这段文字应该在测试的时候被替换掉. 现在显示是应该的, 因为Alpha是1";
    }

    public void ShowText(string message)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayText(message));
    }

    private IEnumerator DisplayText(string message)
    {
        promptText.text = message;
        float fadeSpeed = 1 / fadeTime;
        float displayTimer = 0f;
        while (displayTimer < fadeTime)
        {
            displayTimer += Time.deltaTime;
            SetTextAlpha(displayTimer * fadeSpeed);
            yield return null;
        }

        SetTextAlpha(1);
        yield return new WaitForSeconds(displayTime);
        displayTimer = 0f;
        while (displayTimer < fadeTime)
        {
            displayTimer += Time.deltaTime;
            SetTextAlpha(1 - displayTimer * fadeSpeed);
            yield return null;
        }

        SetTextAlpha(0);
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = promptText.color;
        color.a = alpha;
        promptText.color = color;
    }
}
