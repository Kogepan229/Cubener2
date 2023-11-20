using UnityEngine;
using UnityEngine.XR;

public class DisablePositionTracking : MonoBehaviour
{
    private void LateUpdate()
    {
        var d = -InputTracking.GetLocalPosition(XRNode.CenterEye);
        d.y = transform.localPosition.y;
        transform.localPosition = d;
    }
}
