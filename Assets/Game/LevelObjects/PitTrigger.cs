using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitTrigger : TileTrigger
{
    public override void ActivateTrigger(PlayerCharacter character)
    {
        character.IsDead = true;
    }
}
