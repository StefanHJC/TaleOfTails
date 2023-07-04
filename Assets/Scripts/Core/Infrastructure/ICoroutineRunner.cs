using System;
using System.Collections;
using UnityEngine;

namespace Core.Infrastructure
{
    public interface ICoroutineRunner
    {
        public Coroutine StartCoroutine(IEnumerator coroutine);
        public void StopCoroutine(IEnumerator coroutine);
        public void StopCoroutine(Coroutine coroutine);
    }
}