using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnScreenController : MonoBehaviour
{
    Vector3 ogControllerPosition;


    [SerializeField]
    GameObject controller;

    [SerializeField]
    float controllerRange;

    [SerializeField]
    PlayerMovement player; 


    // GameObject controller;


    bool buttonDown;


    // Start is called before the first frame update
    void Start()
    {

        ogControllerPosition = controller.transform.position;
        controller.transform.position = ogControllerPosition;

    }



    public void ButtonDown()
    {
        buttonDown = true;
    }

    public void ButtonUp()
    {
        buttonDown = false;
        player.Move(ControllsCalculations(controller.transform.position, ogControllerPosition)); 
    }

    void ControllerPositioning()
    {
        if (buttonDown) controller.transform.position = Input.mousePosition;
        else controller.transform.position = ogControllerPosition;

        Vector3 pos = controller.transform.position;

        if ((pos - ogControllerPosition).magnitude > controllerRange) controller.transform.position = (pos - ogControllerPosition).normalized * controllerRange + ogControllerPosition;

    }

    Vector3 ControllsCalculations(Vector2 pos, Vector2 ogPos)
    {
        Vector3 final = (ogPos - pos)/controllerRange;
        Debug.Log(final);
        return final; 
    }

    // Update is called once per frame
    void Update()
    {
        ControllerPositioning();
    }
}
