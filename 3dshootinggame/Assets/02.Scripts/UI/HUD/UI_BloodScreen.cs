using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_BloodScreen : MonoBehaviour
{
    private Image _bloodScreen;
    public float FadeDuration = 1f;

    private void Awake()
    {
        _bloodScreen = GetComponent<Image>();
    }
    public IEnumerator BloodFade()
    {
        float t = 0f;
        while(t < FadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / FadeDuration);
            _bloodScreen.color = new Color(_bloodScreen.color.r, _bloodScreen.color.g, _bloodScreen.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(FadeDuration);

        t = 0f;
        while(t < FadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / FadeDuration);
            _bloodScreen.color = new Color(_bloodScreen.color.r, _bloodScreen.color.g, _bloodScreen.color.b, alpha);
            yield return null;
        }
        
    }

}
