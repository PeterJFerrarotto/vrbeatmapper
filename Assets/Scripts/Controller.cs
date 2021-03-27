using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    private UnityEngine.XR.InputDevice controllerReference;

    [SerializeField]
    public UnityEngine.XR.InputDeviceCharacteristics Device;

    [SerializeField]
    public GameObject Wand;

    PhysicsTracker tracker = new PhysicsTracker();

    // Start is called before the first frame update
    void Start()
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = Device;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, devices);
        if (devices.Count == 1) {
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
                if (speed >= 1.0f)
                {
                    UnityEngine.XR.HapticCapabilities hapticCapabilities;
                    if (controllerReference.TryGetHapticCapabilities(out hapticCapabilities))
                    {
                        controllerReference.SendHapticImpulse(1, 0.5f, 0.125f);
                    }
                    Vector3 direc = tracker.Direction;
                    Debug.Log(direc);
                }

            } else
            {
                Wand.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            }
        }
    }
}
