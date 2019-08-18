using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServerEvent {
    CLIENT_CLICKED
}

public enum ClientEvent {
    CLIENT_TURN_END,
    CLIENT_TURN_START,
    CLIENT_WIN,
    CLIENT_LOSE,
    DRAW
}
