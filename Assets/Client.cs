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
    float pong = 5f;

    // Start is called before the first frame update
    void Start() {
        if (enabled) {
            ws = new WebSocket("ws://localhost:8080");
            ws.Connect();
            ws.OnMessage += (sender, e) => {
                float ping = 0f;
                if (UI.masterToggle && UI.geolocationToggle) {
                    if (UI.privacyLevel == 1) {
                        pong = 0.025f - ping;
                    } else if (UI.privacyLevel == 2) {
                        pong = 0.030f - ping;
                    } else if (UI.privacyLevel == 3) {
                        pong = 0.050f - ping;
                    }
                } else {
                    pong = 0f;
                }
            };
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
        if (pong <= 0f) {
            pong = 5f;
            ws.Send("pong");
        } else {
            pong -= 1f / hz;
        }
    }
}
