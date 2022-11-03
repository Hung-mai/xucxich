using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    private static T _instance;

    public static T ins {
        get {
            if (_instance == null)  Debug.LogError($"Lỗi null  '{typeof(T)}' ");
            return _instance;
        }
    }

    public virtual void Awake() {
        if (_instance != null) {
            Destroy(this.gameObject); 
            return;
        }
        _instance = this as T;
    }
}
