using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class Client : MonoBehaviour {
    public InterfaceManager UI;
    public bool enabled;
    public GameObject left;
    public GameObject right;
    public GameObject center;
    WebSocket ws;

    // Start is called before the first frame update
    void Start() {
        if (enabled) {
            ws = new WebSocket("ws://localhost:8080");
            ws.Connect();
            SendTele();
        }
    }

    void SendTele() {
        float hz = 120f;
        if (UI.masterToggle && UI.trackingRateToggle) {
            if (UI.privacyLevel == 1) {
                hz = 90f;
            } else if (UI.privacyLevel == 2) {
                hz = 72f;
            } else if (UI.privacyLevel == 3) {
                hz = 60f;
            }
        }
        Invoke("SendTele", 1f / hz);
        ws.Send(left.transform.position.ToString() + "\n" + right.transform.position.ToString() + "\n" + center.transform.position.ToString());
    }
}
