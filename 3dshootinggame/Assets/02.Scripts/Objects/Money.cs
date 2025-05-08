using DG.Tweening;
using UnityEngine;

public class Money : MonoBehaviour
{
    private MeshCollider _meshCollider;
    private Rigidbody _rigidbody;
    private Player _player;
    private bool _isSpawned = false;

    private Ease _magnetEase = Ease.OutQuad;
    private Tweener _magnetTween;
    public void Init()
    {
        _meshCollider.enabled = false;
        _rigidbody.isKinematic = true;
        gameObject.SetActive(false);
    }

    public void Spawn()
    {
        _meshCollider.enabled = true;
        _rigidbody.isKinematic = false;
        gameObject.SetActive(true);

        _magnetTween?.Kill();

        _magnetTween = transform
            .DOMove(_player.transform.position, 1f)      // 초기 duration은 값에 상관없음, SetSpeedBased 사용 :contentReference[oaicite:1]{index=1}
            .SetSpeedBased(true)               // speed 기반 이동으로 변경
            .SetEase(Ease.Linear)
            .SetAutoKill(false)                // 끝나도 자동으로 죽지 않게
            .OnUpdate(UpdateTarget)            // 매 프레임마다 목표를 갱신
            .Play();
    }
    private void OnArrived()
    {
        // 도착 시 처리 (점수 추가, 이펙트 등)
        // ...

        // 풀로 반환
        _magnetTween = null;
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        // 비활성화 시 트윈 해제
        _magnetTween?.Kill();
        _magnetTween = null;
    }
    private void UpdateTarget()
    {
        // tween의 목표 위치를 플레이어 현재 위치로 변경
        _magnetTween.ChangeEndValue(_player.transform.position, true);
    }
    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            _player = obj.GetComponent<Player>();
        }
    }

    void Update()
    {
        if (_isSpawned == false) return;


    }
}
