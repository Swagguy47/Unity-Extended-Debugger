using UnityEngine;
using UnityEngine.UI;

namespace ExtendedDebugFramework
{
    //  FPS counter widget
    public class EDFWidget_FpsCounter : MonoBehaviour
    {
        Text text;
        private void Start()
        {
            text= GetComponent<Text>();
            InvokeRepeating("UpdateCounter", 0.1f, 0.1f);
        }

        void UpdateCounter()
        {
            text.text = Mathf.RoundToInt(1.0f / Time.deltaTime).ToString();
        }
    }
}
