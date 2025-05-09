using DG.Tweening;
using UnityEngine;

public class Money : MonoBehaviour
{
    
   [SerializeField]private SphereCollider _collider;
   [SerializeField]private Transform _playerTransform;
   [SerializeField]private bool _isSpawned = false;
   [SerializeField]private Ease _magnetEase = Ease.OutQuad;
   [SerializeField]private Tweener _magnetTween;

    private Player _player;
    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if(obj != null)
        {
            _playerTransform = obj.transform;
            _player = obj.GetComponent<Player>();
        }
        else
        {
            Debug.LogError("Player not found");
        }
    }
    public void Init()
    {
        _collider.enabled = false;
        gameObject.SetActive(false);
    }

    public void Spawn()
    {
        _collider.enabled = true;
        gameObject.SetActive(true);

        _magnetTween?.Kill();

        _magnetTween = transform
            .DOMove(_playerTransform.position, 1f)      // 초기 duration은 값에 상관없음, SetSpeedBased 사용 :contentReference[oaicite:1]{index=1}
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
        _magnetTween?.Kill();
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
        if (_magnetTween == null || _playerTransform == null) return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = _playerTransform.position;
        float remainingDuration = _magnetTween.Duration(false) - _magnetTween.Elapsed(false);

        _magnetTween.ChangeValues(currentPos, targetPos, remainingDuration);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Player와 충돌 시 처리
            Debug.Log($"Money Player와 충돌");
            _player.PlayerMoney += 1;
            UI_Manager.Instance.SetMoney(_player.PlayerMoney);
            OnArrived();
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (_isSpawned == false) return;


    }
}
