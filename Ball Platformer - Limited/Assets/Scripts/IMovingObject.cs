using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovingObject
{
    void Rewind();
    void Unrewind();
    void PauseRB(bool pauseOn);
}
