using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum EPopupType
{
    UI_OptionPopup,
    UI_CreditPopup,
}
public class PopupManager : Singleton<PopupManager>
{
    public EPopupType PopupType = EPopupType.UI_OptionPopup;
    [Header("팝업 UI 참조")]
    public List<UI_Popup> Popups; // 모든 팝업을 관리하는데
    
    private List<UI_Popup> _openedPopups = new List<UI_Popup>(); // null은 아니지만 비어있는 리스트
    public void Open(EPopupType popupType, Action closeCallback = null)
    {
        PopupType = popupType;
        Open(popupType.ToString(), closeCallback);
    }
    private void Open(string popupName, Action closeCallback)
    {
        foreach(UI_Popup popup in Popups)
        {
            if (popup.gameObject.name == popupName)
            {
                popup.Open(closeCallback);
                _openedPopups.Add(popup);
                break;
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_openedPopups.Count > 0)
            {
                while(true)
                {
                    bool opened = _openedPopups[_openedPopups.Count - 1].isActiveAndEnabled;
                    _openedPopups[_openedPopups.Count - 1].Close();
                    _openedPopups.RemoveAt(_openedPopups.Count - 1);

                    // 열려있는 팝업을 닫았거나 || 더이상 닫을 팝업이 없다면 탈출
                    if (opened || _openedPopups.Count == 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                GameManager.Instance.Pause();
            }
        }
    }

}
