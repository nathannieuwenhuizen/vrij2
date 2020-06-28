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
        new SpokenLine("King", "My dear subordinates.", 0),
        new SpokenLine("David", "What did he do this time?"),
        new SpokenLine("Ronald", "He probably lost the gnome jewels again."),
        new SpokenLine("King", "They were STOLEN from me!", 1),
        new SpokenLine("King", "A human took it from where I left it!", 2),
        new SpokenLine("King", "They put my jewels inside their nest called a 'museum'. What outrageous!", 3),
        new SpokenLine("King", "Now I need you guys to steal them back for me.", 4),
        new SpokenLine("King", "So go forth! Oh, brave heroes of the gnome empire! ", 5),
        new SpokenLine("King", "Go and take back my Jewels from the human’s nest! ", 6),
        new SpokenLine("King", "And some extra’s if you’re at it.", 7)
    };
    public static SpokenLine[] alarmGoesOff = new SpokenLine[] {
        new SpokenLine("David", "Oh on! The whole museum is alerted!"),
        new SpokenLine("Ronald", "How do we get out?"),
        new SpokenLine("David", "Maybe the crown can help us out with it's magical powers?"),
        new SpokenLine("David", "Like going through walls?"),
        new SpokenLine("Ronald", "Yeah... like THAT is going to happen!"),
        new SpokenLine("David", "Well, try it!")
    };
    public static SpokenLine[] endDialogue = new SpokenLine[] {
        new SpokenLine("King", "My dear subordinates.", 0),
        new SpokenLine("King", "Thank you so much for retrieving the crown form the human nest!", 1),
        new SpokenLine("King", "You're all promoted to...", 2),
        new SpokenLine("King", "Executive right hand gnome assistent!", 3),
        new SpokenLine("King", "This was one succesful...", 4),
        new SpokenLine("King", "GNOME HEIST!", 5),
    };

}
