using System.Collections.Generic;
using UnityEngine;
using System;

namespace SNEngine.Polling
{
    public class PoolMono<T> where T : MonoBehaviour
    {
        private List<T> _pool;

        public T Prefab { get; private set; }
        public Transform Container { get; private set; }
        public bool AutoExpand { get; private set; }
        public IEnumerable<T> Objects => _pool;

        public PoolMono(T prefab, Transform container, int count, bool autoExpand = false)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab), $"PoolMono<{typeof(T).Name}>: Prefab is null.");
            }

            Prefab = prefab;
            Container = container;
            AutoExpand = autoExpand;
            _pool = new List<T>();

            for (int i = 0; i < count; i++)
            {
                CreateObject();
            }
        }

        private T CreateObject(bool isActiveByDefault = false)
        {
            var createdObject = UnityEngine.Object.Instantiate(Prefab, Container);
            createdObject.gameObject.SetActive(isActiveByDefault);
            _pool.Add(createdObject);
            createdObject.gameObject.name = $"{typeof(T).Name}_{_pool.Count}";
            return createdObject;
        }

        private bool HasFreeElement(out T element)
        {
            foreach (var mono in _pool)
            {
                if (mono != null && !mono.gameObject.activeSelf)
                {
                    element = mono;
                    return true;
                }
            }
            element = null;
            return false;
        }

        public T GetFreeElement()
        {
            if (HasFreeElement(out T element))
            {
                return element;
            }

            if (AutoExpand)
            {
                return CreateObject(true);
            }

            throw new Exception($"PoolMono<{typeof(T).Name}>: No free elements.");
        }

        public void HideAllElements()
        {
            foreach (var item in _pool)
            {
                if (item != null) item.gameObject.SetActive(false);
            }
        }
    }
}