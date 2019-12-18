using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool m_IsSingletonDestroyed = false;

    private static T m_Instance;
    public static T Instance
    {
        get
        {
            if (m_IsSingletonDestroyed) 
            {
                Debug.LogWarning("Singleton was already destroyed!!!");
                return null;
            }

            if (!m_Instance) 
            {
                new GameObject(typeof(T).ToString()).AddComponent<T>();

                DontDestroyOnLoad(m_Instance);
            }

            return m_Instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_Instance == null && !m_IsSingletonDestroyed)
        {
            m_Instance = this as T;
        }
        else if (m_Instance != this)
        {
            Destroy(this);
        }
    }

    protected virtual void OnDestroy()
    {
        if (m_Instance != this)
        {
            return;
        }

        m_IsSingletonDestroyed = true;
        m_Instance = null;
    }
}
