using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Ready,
    Play,
    Pause,
    GameOver
}

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentGameState { get; private set; } = GameState.Ready;

    [Header("UI Elements")]
    [SerializeField] private Image stateImage; // 상태를 표시할 이미지
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("State Sprites")]
    [SerializeField] private Sprite readySprite; 
    [SerializeField] private Sprite playSprite;  
    [SerializeField] private Sprite gameOverSprite; 

    [Header("Game Settings")]
    [SerializeField] private float readyDuration = 3f; // 준비 시간(초)
    [SerializeField] private float gameOverDisplayDuration = 3f; // 게임오버 표시 시간(초)

    // 게임이 시작될 때 호출
    void Start()
    {
        // Ready 상태로 시작
        SetGameState(GameState.Ready);

        // 준비 화면 표시하는 코루틴 시작
        StartCoroutine(ReadySequence());
    }

    void Update()
    {
        // 게임오버 상태에서 재시작 가능하도록
        if (CurrentGameState == GameState.GameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    public void Pause()
    {
        SetGameState(GameState.Pause);
    }
    public void Continue()
    {
        // 게임 재개
        SetGameState(GameState.Play);
    }

    public void Restart()
    {
        CurrentGameState = GameState.Ready;
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Confined;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    // 게임 상태 변경 메서드
    public void SetGameState(GameState newState)
    {
        CurrentGameState = newState;

        // 상태에 따른 처리
        switch (CurrentGameState)
        {
            case GameState.Ready:
                // Ready 상태에서는 게임 시간 멈춤 (모든 물리/애니메이션 멈춤)
                Time.timeScale = 0f;

                // Ready 이미지 표시
                if (stateImage != null && readySprite != null)
                {
                    stateImage.gameObject.SetActive(true);
                    stateImage.sprite = readySprite;
                }
                break;

            case GameState.Play:
                // Play 상태에서는 게임 시간 정상 진행
                Time.timeScale = 1f;
                if (CameraManager.Instance.CameraType != CameraType.ISO)
                    Cursor.lockState = CursorLockMode.Locked;
                // Play 이미지 표시

                break;

            case GameState.GameOver:
                // GameOver 상태에서는 게임 시간 멈춤 (모든 물리/애니메이션 멈춤)
                Time.timeScale = 0f;

                // GameOver 이미지 표시
                if (stateImage != null && gameOverSprite != null)
                {
                    stateImage.gameObject.SetActive(true);
                    stateImage.sprite = gameOverSprite;
                }
                break;
            case GameState.Pause:
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
                PopupManager.Instance.Open(EPopupType.UI_OptionPopup, Continue);
                break;
        }
    }

    // 준비 상태 코루틴
    private IEnumerator ReadySequence()
    {
        // Ready 상태 설정
        SetGameState(GameState.Ready);

        // 카운트다운 시작 (timeScale=0이므로 realtime으로 대기)
        for (int i = (int)readyDuration; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(true);
                countdownText.text = i.ToString();
            }
            yield return new WaitForSecondsRealtime(1f); // realtime 사용
        }

        yield return new WaitForSecondsRealtime(0.5f); // realtime 사용

        // 카운트다운 텍스트 숨기기
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // Play 상태로 전환
        if (stateImage != null && playSprite != null)
        {
            stateImage.gameObject.SetActive(true);
            stateImage.sprite = playSprite;
        }
        SetGameState(GameState.Play);

        // Play 이미지 잠시 표시 후 숨기기
        yield return new WaitForSeconds(1.5f); // 일반 WaitForSeconds (timeScale 영향 받음)
        if (stateImage != null)
            stateImage.gameObject.SetActive(false);
    }

    // 게임 종료 메서드 (외부에서 호출)
    public void EndGame()
    {
        if (CurrentGameState != GameState.Play)
            return;

        StartCoroutine(GameOverSequence());
    }

    // 게임오버 코루틴
    private IEnumerator GameOverSequence()
    {
        // GameOver 상태 설정
        SetGameState(GameState.GameOver);

        // 일정 시간 후에 재시작 안내 표시 (timeScale=0이므로 realtime으로 대기)
        yield return new WaitForSecondsRealtime(gameOverDisplayDuration);

        // 재시작 안내 (이미지는 계속 표시)
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "R 키를 눌러 재시작";
        }
    }

    // 게임 재시작 메서드
    public void RestartGame()
    {
        // 모든 코루틴 중지
        StopAllCoroutines();

        // 텍스트 초기화
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // 게임 준비 시퀀스 재시작
        StartCoroutine(ReadySequence());
    }

    // 게임 종료 시 호출되는 메서드(Scene 전환 등)
    private void OnDestroy()
    {
        // 씬 전환 시에도 timeScale이 정상으로 복원되도록 함
        Time.timeScale = 1f;
    }

    // 다른 스크립트에서 현재 게임 상태 확인용
    public bool IsGameActive()
    {
        return CurrentGameState == GameState.Play;
    }
}