using UnityEngine;

public class UI_OptionPopup : MonoBehaviour
{
    public UI_CreditPopup CreditPopup;

    public void OnClickCreditButton()
    {
        CreditPopup.Open();
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnClickContinueButton()
    {
        GameManager.Instance.Continue();
        gameObject.SetActive(false);
    }
    public void OnClickRetryButton()
    {
        // 게임 재시작
        gameObject.SetActive(false);
        GameManager.Instance.RestartGame();
    }
    public void OnClickQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 게임 종료
        Application.Quit();
#endif
    }
}
