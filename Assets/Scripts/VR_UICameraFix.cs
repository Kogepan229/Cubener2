using UnityEngine;

namespace raspberly.ovr
{
    public class VR_UICameraFix : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Camera>().enabled = false;
            GetComponent<Camera>().enabled = true;
        }
    }
}