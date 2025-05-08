using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            lock (_lock)
            {
                // 씬에 이미 생성된 인스턴스가 있는지 검색
                _instance = FindAnyObjectByType<T>();
                if (_instance != null) return _instance;

                // 없으면 새 GameObject에 컴포넌트로 추가
                GameObject singletonObject = new GameObject(typeof(T).Name);
                _instance = singletonObject.AddComponent<T>();
                Debug.Log($"{typeof(T)} was created automatically.");
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 그 게임 오브젝트는 삭제하지 않겠다.
            Init();
        }
        else if(_instance != this)
        {
            Destroy(gameObject);
        }
    }
    protected virtual void Init() { }
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
