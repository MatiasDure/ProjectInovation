using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformObject : MonoBehaviour
{
    public void SetScale(Vector3 scale)
    {
        this.transform.localScale = scale;
    }

    public void SetPosition(Vector3 position)
    {
        this.transform.localPosition = position;
    }
}
