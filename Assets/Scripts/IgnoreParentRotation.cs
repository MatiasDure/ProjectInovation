using UnityEngine;

public class IgnoreParentRotation : MonoBehaviour
{
    private Quaternion _originalRotation;

    void Start()
    {
        // Store the original rotation
        _originalRotation = transform.rotation;
    }

    void Update()
    {
        // Cancel out the parent's rotation
        if (transform.parent != null)
        {
            transform.rotation = Quaternion.Euler(0, 0, -transform.parent.rotation.z);
            transform.localPosition = Vector3.zero;
        }
    }
}