using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBeatMapper
{
    public class Controller : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            NoteCreator creator;
            if (other.gameObject.TryGetComponent<NoteCreator>(out creator))
            {
                currentLane = creator.laneIndex;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            NoteCreator creator;
            if (other.gameObject.TryGetComponent<NoteCreator>(out creator))
            {
                if (currentLane == creator.laneIndex)
                {
                    currentLane = -1;
                }
            }
        }

        private int currentLane = -1;

        private UnityEngine.XR.InputDevice controllerReference;

        [SerializeField]
        public UnityEngine.XR.InputDeviceCharacteristics Device;

        [SerializeField]
        public GameObject Wand;

        [SerializeField]
        public float SpeedThreshold = 1.0f;

        [SerializeField]
        public GameObject Baubin;

        PhysicsTracker tracker = new PhysicsTracker();

        // Start is called before the first frame update
        void Start()
        {
            var devices = new List<UnityEngine.XR.InputDevice>();
            var desiredCharacteristics = Device;
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, devices);
            if (devices.Count == 1)
            {
                controllerReference = devices[0];
            }

            tracker.Reset(transform.localPosition, transform.localRotation, Vector3.zero, Vector3.zero);
        }

        // Update is called once per frame
        void Update()
        {
            tracker.Update(transform.localPosition, transform.localRotation, Time.smoothDeltaTime);
            bool triggerState;
            if (controllerReference.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerState))
            {
                if (triggerState)
                {
                    Wand.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    float speed = tracker.Speed;
                    if (speed >= SpeedThreshold)
                    {
                        Vector3 direc = tracker.Direction;
                        Debug.Log(direc);
                        if (currentLane != -1)
                        {
                            NoteType noteType;
                            if ((Device & UnityEngine.XR.InputDeviceCharacteristics.Left) != 0)
                            {
                                noteType = NoteType.LEFT;
                            }
                            else
                            {
                                noteType = NoteType.RIGHT;
                            }
                            SongManager.Instance.AddNote(currentLane, noteType, direc);
                            UnityEngine.XR.HapticCapabilities hapticCapabilities;
                            if (controllerReference.TryGetHapticCapabilities(out hapticCapabilities))
                            {
                                controllerReference.SendHapticImpulse(1, 0.5f, 0.125f);
                            }
                        }
                    }

                }
                else
                {
                    Wand.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                }
            }
        }
    }
}