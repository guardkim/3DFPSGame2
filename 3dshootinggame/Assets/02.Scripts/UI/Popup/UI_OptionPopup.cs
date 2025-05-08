using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UI_OptionPopup : UI_Popup
{

    
    public void OnClickCreditButton()
    {
        PopupManager.Instance.Open(EPopupType.UI_CreditPopup);
    }
    
    public void OnClickContinueButton()
    {
        Debug.Log("Continue");
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
