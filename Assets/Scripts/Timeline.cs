using UnityEngine;
using System.Collections;

namespace VRBeatMapper
{
    public class Timeline : MonoBehaviour
    {
        public GameObject AudioObject;
        private float scrollPos = 0f;//Position of Scroll
        bool Trig;// Used as a "gate"

        private void Start()
        {
        }

        private void OnGUI()
        {

            scrollPos = GUI.HorizontalSlider(new Rect(0f, 50f, Screen.width, 50f), scrollPos, 0, AudioObject.GetComponent<AudioSource>().clip.length);

            if (GUI.changed == true)
            {
                Trig = true;// Open "gate"
            }
            if (GUI.changed == false && !Input.GetMouseButton(0) && Trig == false)
            {
                scrollPos = AudioObject.GetComponent<AudioSource>().time;// Makes slider follow the audio when not used (Clicked)
            }
            if (Input.GetMouseButtonUp(0) && Trig == true)
            {
                AudioObject.GetComponent<AudioSource>().time = scrollPos;// Will only change the audio position once the mouse is released
                Trig = false;
            }

            GUI.Label(new Rect(10f, 80f, 100f, 30f), (AudioObject.GetComponent<AudioSource>().time).ToString());

        }

    }
}