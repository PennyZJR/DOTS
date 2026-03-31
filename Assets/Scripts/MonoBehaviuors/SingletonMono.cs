using UnityEngine;

namespace MonoBehaviuors
{
    public class SingletonMono<T>:MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if(_instance!=null)
                    return _instance;
                GameObject obj=new GameObject(typeof(T).Name);
                _instance=obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
                return _instance;
            }
        }
        public static bool IsNull()
        {
            return _instance == null;
        }
    }
}