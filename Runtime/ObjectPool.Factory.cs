using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleObjectPool{
    public static class ObjectPool{
        public static Func<T> MonoBehaviourCreateInstance<T>(T prefab,Transform parent) where T : MonoBehaviour => 
            () => {
                var inst = Object.Instantiate(prefab, parent);
                inst.transform.SetParent(parent);
                return inst;
            };
        
        public static Func<T> MonoBehaviourCreateInstance<T>(T prefab) where T : MonoBehaviour => 
            () => Object.Instantiate(prefab);
            
        public static void MonoBehaviourOnRelease(MonoBehaviour monoBehaviour) =>
            monoBehaviour.gameObject.SetActive(false);
        
        public static void MonoBehaviourOnTake(MonoBehaviour monoBehaviour) =>
            monoBehaviour.gameObject.SetActive(true);

        public static void MonoBehaviourOnClear(MonoBehaviour monoBehaviour) =>
            Object.Destroy(monoBehaviour.gameObject);
    }
}
