using System.Collections;
using UnityEngine;

public class MouseController : CreatureController
{
    protected override void Actions()
    {
        if(IsMovePossible)
            CharactorMove();
        else
            CharactorRotate();
    }
}