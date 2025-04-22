using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    // �ʿ� �Ӽ�
    // - ��ź �߻� ��ġ
    public GameObject FirePosition;
    // - ��ź ������
    public GameObject BombPrefab;

    public float ThrowPower = 15f;
    public float FireCoolTime = 0.1f;
    public int FireMaxCount = 50;

    private float _buttonDownTimer = 0.0f;
    private float _fireTimer = 0.1f;
    private int _currentBoomCount = 3;
    private int _fireCurrentCount = 50;

    private bool _isReloading = false;

    public ParticleSystem BulletEffect;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void FireBoom()
    {
        UI_Manager.Instance.RemoveBoom();
        _currentBoomCount = UI_Manager.Instance.GetBoomCount();
        BombPool.Instance.Create(FirePosition.transform.position, ThrowPower * _buttonDownTimer);
    }
    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (_currentBoomCount <= 0) return;
            _buttonDownTimer += Time.deltaTime;
            if (_buttonDownTimer >= 3.0f) _buttonDownTimer = 3.0f;
            
        }
        if(Input.GetMouseButtonUp(1))
        {
            if (_currentBoomCount <= 0) return;
            FireBoom();
            _buttonDownTimer = 0.0f;
        }
        // �Ѿ� �߻�(������ ���)
        // 1. ���� ��ư �Է� �ޱ�
        if (Input.GetMouseButton(0))
        {
            if(UI_Manager.Instance.IsReloading == true) return;
            _fireTimer -= Time.deltaTime;
            if(_fireTimer <= 0.0f && _fireCurrentCount > 0)
            {
                _fireCurrentCount--;
                // 2. ���̸� �����ϰ� �߻� ��ġ�� ���� ������ ����
                Ray ray = new Ray(FirePosition.transform.position, Camera.main.transform.forward);
                // 3. ���̿� �ε��� ��ü�� ������ ������ ������ ����
                RaycastHit hitInfo = new RaycastHit();
                // 4. ���̸� �߻��Ѵ��� 
                bool isHit = Physics.Raycast(ray, out hitInfo);
                if (isHit) //  ������ �����Ͱ� �ִٸ�(�ε����ٸ�)
                {
                    // �ǰ� ����Ʈ ����(ǥ��)
                    BulletEffect.transform.position = hitInfo.point;
                    BulletEffect.transform.forward = hitInfo.normal;
                    BulletEffect.Play();
                }
                UI_Manager.Instance.SetBulletCount(_fireCurrentCount);
                _fireTimer = FireCoolTime;
            }
        }
            // Ray : ������ (������ġ, ����)
            // RayCast : �������� �߻�
            // RayCastHit : �������� ��ü�� �ε����ٸ�, �� ������ �����ϴ� ����ü

        if(Input.GetKeyDown(KeyCode.R))
        {
            if (_fireCurrentCount < FireMaxCount)
            {
                UI_Manager.Instance.Reload();
                _fireCurrentCount = FireMaxCount;
            }
        }
    }
}
