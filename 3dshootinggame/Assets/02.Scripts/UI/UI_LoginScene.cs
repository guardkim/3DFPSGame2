using System;
using TMPro;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class UI_InputFields
{
    public TextMeshProUGUI ResultText; // 결과 텍스트
    public TMP_InputField IDInputField;
    public TMP_InputField PasswordInputField;
    public TMP_InputField PasswordConfirmInputField;
    public Button ConfirmButton;
}
public class UI_LoginScene : MonoBehaviour
{
    [Header("패널")]
    public GameObject LoginPanel;
    public GameObject RegisterPanel;

    [Header("로그인")]   public UI_InputFields LoginInputFields;
    [Header("회원가입")] public UI_InputFields RegisterInputFields;

    private const string PREFIX = "ID_";
    private const string SALT = "10043420"; // 해시 암호화 시 사용되는 salt 값


    private void Start()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
        LoginCheck();
    }

    public void OnClickRegisterButton()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }
    public void OnClickGoToLoginButton()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }
    public void OnClickRegisterConfirmButton()
    {
        //if(PasswordInputField.text(PasswordConfirmInputField.text))
        {

        }
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }
    // 회원가입
    public void Register()
    {
        // 1. 아이디 입력을 확인한다.
        string id = RegisterInputFields.IDInputField.text;
        if(string.IsNullOrEmpty(id))
        {
            RegisterInputFields.ResultText.text = "아이디를 입력해주세요.";
            return;
        }
        // 2. 비밀번호 입력을 확인한다.
        string password = RegisterInputFields.PasswordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            RegisterInputFields.ResultText.text = "비밀번호를 입력해주세요.";
            return;
        }
        // 3. 2차 비밀번호 입력을 확인하고, 1차 비밀번호 입력과 같은지 확인한다.

        string passwordConfirm = RegisterInputFields.PasswordConfirmInputField.text;
        if (string.IsNullOrEmpty(passwordConfirm))
        {
            RegisterInputFields.ResultText.text = "비밀번호 확인을 입력해주세요.";
            return;
        }

        if (password != passwordConfirm)
        {
            RegisterInputFields.ResultText.text = "비밀번호가 일치하지 않습니다.";
            return;
        }

        // 4. PlayerPrefs를 이용해서 아이디와 비밀번호를 저장한다.
        PlayerPrefs.SetString( PREFIX + id, Encryption(password + SALT));

        RegisterInputFields.ResultText.text = "회원가입이 완료되었습니다.";


        // 5. 로그인 창으로 돌아간다. (이 때 아이디는 자동 입력 되어있다.)'
        LoginInputFields.IDInputField.text = id;
        OnClickGoToLoginButton();
    }

    public string Encryption(string text)
    {
        // 해시 암호화 알고리즘 인스턴스를 생성한다.
        SHA256 sha256 = SHA256.Create();

        // 운영체제 혹은 프로그래밍 언어별로 string 표현하는 방식이 다 다르므로
        // UTF8 버전 바이트를 사용하여 byte[]로 변환한다.
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
        byte[] hash = sha256.ComputeHash(bytes);

        string resultText = string.Empty;
        foreach (byte b in hash)
        {
            // byte를 다시 string으로 변환한다.
            resultText += b.ToString("X2");
        }
        // X2는 16진수로 변환한다는 의미이다.
        return resultText;
    }
    public void Login()
    {
        // 1. 아이디 입력을 확인한다.
        string id = LoginInputFields.IDInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            LoginInputFields.ResultText.text = "아이디를 입력해주세요.";
            return;
        }
        // 2. 비밀번호 입력을 확인한다.
        string password = LoginInputFields.PasswordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            LoginInputFields.ResultText.text = "비밀번호를 입력해주세요.";
            return;
        }
        // 3. PlayerPrefs.Get을 이용해서 아이디와 비밀번호가 맞는지 확인한다.
        if (!PlayerPrefs.HasKey(PREFIX + id))
        {
            LoginInputFields.ResultText.text = "아이디와 비밀번호를 확인해주세요";
            return;
        }
        string hashedPassword = PlayerPrefs.GetString(PREFIX + id);
        if (hashedPassword != Encryption(password + SALT))
        {
            LoginInputFields.ResultText.text = "아이디와 비밀번호를 확인해주세요";
            return;
        }
        // 4. 맞다면 로그인
        else
        {
            LoginInputFields.ResultText.text = "로그인 성공";
            // 로그인 성공 시 처리
            // LoadingScene으로 이동
            SceneManager.LoadScene(1);
        }
    }
    
    // 아이디와 비밀번호 InputField 값이 바뀌었을 경우에만.
    public void LoginCheck()
    {
        string id = LoginInputFields.IDInputField.text;
        string password = LoginInputFields.PasswordInputField.text;

        LoginInputFields.ConfirmButton.enabled = !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password);
    }
}
