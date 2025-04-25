using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : Singleton<UI_Manager>
{
    public TextMeshProUGUI CurrentBoomCount;
    public TextMeshProUGUI ReloadText;
    public TextMeshProUGUI CurrentBulletCount;
    public UI_BloodScreen BloodScreen;
    public Slider ReloadProgressbar;

    public bool IsReloading = false;
    private float _reloadTime = 2.0f;
    private float _elapsedTime = 0.0f;

    public void BloodFade()
    {
        StartCoroutine(BloodScreen.BloodFade());
    }
    public void AddBoom()
    {
        int count = GetBoomCount();
        count++;
        SetBoomCount(count);
    }
    public void RemoveBoom()
    {
        int count = GetBoomCount();
        count--;
        SetBoomCount(count);
    }
    public int GetBoomCount()
    {
        string[] text = CurrentBoomCount.text.Split('/');
        int count = int.Parse(text[0]);

        return count;
    }
    public void SetBoomCount(int count)
    {
        CurrentBoomCount.text = $"{count.ToString()} / 3";
    }
    public int GetBulletCount()
    {
        string[] text = CurrentBulletCount.text.Split('/');
        int count = int.Parse(text[0]);
        return count;
    }
    public void SetBulletCount(int count)
    {
        CurrentBulletCount.text = $"{count.ToString()} / 50";
    }
    public void ReloadCount()
    {
        CurrentBulletCount.text = $"50 / 50";
    }
    public void Reload()
    {
        IsReloading = true;
    }

    private void Update()
    {
        if (IsReloading == true)
        {
            if (_elapsedTime < _reloadTime)
            {
                ReloadText.gameObject.SetActive(true);
                ReloadProgressbar.gameObject.SetActive(true);
                _elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(_elapsedTime / _reloadTime);
                ReloadProgressbar.value = Mathf.Lerp(0f, 1f, t);
            }
            else
            {
                ReloadText.gameObject.SetActive(false);
                ReloadProgressbar.gameObject.SetActive(false);
                IsReloading = false;
                _elapsedTime = 0.0f;
                ReloadCount();
            }
        }
    }
}
