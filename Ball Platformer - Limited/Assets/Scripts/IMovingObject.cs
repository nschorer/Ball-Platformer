using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovingObject
{
    void RewindTime(bool rewindOn);
    void PauseRB(bool pauseOn);
}
