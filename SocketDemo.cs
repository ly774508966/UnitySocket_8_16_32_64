using UnityEngine;
using System;
using System.Collections;
using LSocket.Net;
using LSocket.Type;
using LSocket.cmd;
using System.Net.Sockets;

    public class SocketDemo : MonoBehaviour
    {
        public UnitySocket[] socket;
        public String[] textAreaString;
        public String[] textFieldString;
        public GUISkin mySkin;
        // Use this for initialization
        void Start()
        {
            socket = new UnitySocket[4];
            textAreaString = new String[12];
            for (int i = 0; i < 12; i++)
            {
                textAreaString[i] = "";
            }
            textFieldString = new String[4];
            for(int i=0;i<4;i++){
                textFieldString[i] = "";
            }
        }

        // Update is called once per frame
        void Update()
        {
			
        }

        void OnGUI()
        {
            GUI.skin = mySkin;
            for (int i = 0; i < 4; i++)
            {
                String s = textAreaString[i * 3] + "\n" + textAreaString[i * 3 + 1] + "\n" + textAreaString[i * 3 + 2];
                GUI.TextArea(new Rect(i % 2 * Screen.width / 2, i / 2 * (Screen.height / 2) + 50, 100, 60), s);
                textFieldString[i] = GUI.TextField(new Rect(i % 2 * Screen.width / 2+50, i / 2 * (Screen.height / 2), 100, 20), textFieldString[i]);
                if (GUI.Button(new Rect(i % 2 * Screen.width / 2, i / 2 * (Screen.height / 2), 40, 20), "连接"))
                {
                    socket[i] = null;
                    socket[i] = new UnitySocket();
                    socket[i].SocketConnection("127.0.0.1", 7981, this, i);
                    socket[i].DoLogin(textFieldString[i]);
                }
                else if (GUI.Button(new Rect(i % 2 * Screen.width / 2, i / 2 *( Screen.height / 2) + 25, 40, 20), "关闭"))
                {
                    if (socket[i] != null)
                    {
                        socket[i].close();
                        socket[i] = null;
                    }
                }
            }

            if (GUI.Button(new Rect(Screen.width - 60, Screen.height - 30, 60, 30), "退出")) {
                Application.Quit();
            }

        }
    }