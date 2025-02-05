using System.Collections;
using UnityEngine;

namespace ExtendedDebugFramework
{
    //  Destroy gameobject after coroutine delay
    public class EDFWidget_Temporary : MonoBehaviour
    {
        public float delay;

        private void Start()
        {
            StartCoroutine(BeginDelay(delay));
        }

        IEnumerator BeginDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            Destroy(gameObject);
        }
    }
}
