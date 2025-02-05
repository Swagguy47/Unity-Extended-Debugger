using UnityEngine;
using UnityEngine.UI;

namespace ExtendedDebugFramework
{
    //  show main.camera position as widget
    public class EDFWidget_CamPos : MonoBehaviour
    {
        Text text;
        private void Start()
        {
            text = GetComponent<Text>();
            InvokeRepeating("UpdatePosition", 0.1f, 0.1f);
        }

        void UpdatePosition()
        {
            text.text =  "position: " + Camera.main.transform.position + "\nrotation: " + Camera.main.transform.rotation.eulerAngles;
        }
    }
}
