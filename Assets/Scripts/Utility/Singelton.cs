using UnityEngine;

#region Example
/*

    using Lama.Utility;

    public class GameManager : Singleton<GameManager>
    {
        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
*/
#endregion

namespace Utility
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

        public static T Instance {
            get {
                if (_instance == null) {
                    var newInstance = FindFirstObjectByType<T>();
                    if (newInstance == null)
                        Debug.LogError($"Failed to get singleton instance for {typeof(T).Name}");
                    else
                        _instance = newInstance;
                }

                return _instance;
            }
        }

        public static bool HasInstance() => _instance != null; 

        private static T _instance;

    }
}