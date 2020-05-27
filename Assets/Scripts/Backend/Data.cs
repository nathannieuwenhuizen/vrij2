using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{

    public static bool ControllerConnected()
    {
        string[] names = Input.GetJoystickNames();
        Debug.Log(names.Length);
        return names.Length > 0;
    }
}
