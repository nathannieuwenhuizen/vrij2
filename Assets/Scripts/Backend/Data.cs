using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{

    public static bool ControllerConnected()
    {
        string[] names = Input.GetJoystickNames();
        return names.Length > 0;
    }

    public static SpokenLine[] introDialogue = new SpokenLine[] {
        new SpokenLine("King", "My dear subordinates."),
        new SpokenLine("David", "What did he do this time?"),
        new SpokenLine("Ronald", "He probably lost the gnome jewels again."),
        new SpokenLine("King", "They were STOLEN from me!"),
        new SpokenLine("King", "A human took it from where I left it!"),
        new SpokenLine("King", "They put my jewels inside their nest called a 'museum'. What outrageous!"),
        new SpokenLine("King", "Now I need you guys to steal them back for me."),
        new SpokenLine("King", "So go forth! Oh, brave heroes of the gnome empire! "),
        new SpokenLine("King", "Go and take back my Jewels from the human’s nest! "),
        new SpokenLine("King", "And some extra’s if you’re at it.")
    };
}
