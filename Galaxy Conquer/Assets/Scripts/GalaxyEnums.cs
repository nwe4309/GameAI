using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Enums
{
    public enum Team
    {
        Neutral,
        Red,
        Blue,
        Orange,
        Green
    }

    public enum ShipState
    {
        Idle,
        Detect,
        Move,
        Capture,
        Fight
    }
}
