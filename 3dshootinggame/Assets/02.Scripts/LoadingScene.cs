using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    // 목표 : 다음 씬을 '비동기 방식'으로 로드하고 싶다.
    // 또한 로드 진행률을 시각적으로 표현하고 싶다. -> % 프로그레스 바와 %별 텍스트

    // 속성:
    // - 다음 씬 번호(인덱스)
    public int NextSceneIndex = 2;

    // - 프로그레스 슬라이더바
    public Slider ProgressSlider;

    // - 프로그레스 텍스트
    public TextMeshProUGUI ProgressText;

    private void Start()
    {
        StartCoroutine(LoadNextScene_Coroutine());
    }
    private IEnumerator LoadNextScene_Coroutine()
    {
        // 지정된 씬을 비동기로 로드한다.
        AsyncOperation ao = SceneManager.LoadSceneAsync(NextSceneIndex);

        ao.allowSceneActivation = false; // 비동기로 로드되는 씬의 모습이 화면에 보이지 않게 한다.

        // 로딩이 되는 동안 계속해서 반복문
        while(ao.isDone == false)
        {
            // 비동기로 실행할 코드들
            Debug.Log(ao.progress); // 0 ~ 1
            
            ProgressSlider.value = ao.progress; // 슬라이더바의 값 변경
            ProgressText.text = $"{ao.progress * 100f}%";

            // 서버와 통신해서 유저 데이터나 기획 데이터를 받아오면 된다.
            if(ao.progress <= 0.1f)
            {
                ProgressText.text = "전투 준비 중... 총알이랑 간식 챙기는 중";
            }
            else if(ao.progress <= 0.3f)
            {
                ProgressText.text = "좀비들 근처에 있음. 몰래 훔쳐보는 중...";
            }
            else if (ao.progress <= 0.5f)
            {
                ProgressText.text = "스코프 닦는 중... 흐릿하면 못 맞춤";
            }
            else if (ao.progress <= 0.7f)
            {
                ProgressText.text = "안개 조절 중... 너무 뿌옇게 하면 못 보잖아";
            }
            else if (ao.progress <= 0.9f)
            {
                ProgressText.text = "이제 쏠 일만 남았음. 긴장 ㄴㄴ, 다 계획임";
            }

            if (ao.progress >= 0.9f) // 90프로면 로딩 다 됐다는 뜻
            {
                ao.allowSceneActivation = true; // 씬을 활성화한다.
            }
            //yield return WaitForSeconds(1); // 1초 대기
            yield return null; // 1프레임 대기
        }
    }
}
