using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Katana.Hud
{
    // 画面中央に表示されるメッセージ
    // 1. 固定時間でフェードアウト
    // 2. 
    [RequireComponent(typeof(Text))]
    public class HudMessage : HudComponent
    {
        // 1メッセージの描画時間
        private const float DisplayTime = 7.0f;

        // 表示するテキスト
        private Text _messageText;

        public enum MessageKind : int
        {
            Hint
        }

        struct Message
        {
            public readonly string text;
            public readonly MessageKind kind;

            public Message(string t, MessageKind k)
            {
                text = t;
                kind = k;
            }
        }
        
        List<Message> _messages = new List<Message>();
        private float _timer = -1;

        // テキストの透明度
        float _alpha
        {
            get { return _messageText.color.a; }
            set
            {
                var a = Mathf.Clamp(value, 0, 1);
                _messageText.color = new Color(_messageText.color.r, _messageText.color.g, _messageText.color.b, a);
            }
        }

        // テキストの内容
        string _text
        {
            get { return _messageText.text; }
            set { _messageText.text = value; }
        }

        public void AddMessage(string message, MessageKind kind = MessageKind.Hint)
        {
            _messages.Add(new Message(message, kind));
            MessageStart();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _messageText = GetComponent<Text>();

            // 表示は切る
            _messageText.enabled = false;
        }

        protected override void OnDraw()
        {
            base.OnDraw();

            if(_messages.Count == 0)
                return;

            _timer -= Time.deltaTime;

            if (_timer < 0)
            {
                MessageEnd();
                return;
            }

            _alpha = Mathf.Sin( (_timer / DisplayTime * 3 - 1) * Mathf.PI / 2);
        }

        void MessageStart()
        {
            if(_messages.Count == 0 || _timer >= 0)
                return;
            Debug.Log("Message Start");
            _text  = _messages[0].text;
            _alpha = Mathf.Sin((_timer / DisplayTime * 3 - 1) * Mathf.PI / 2);
            _messageText.enabled = true;
            _timer = DisplayTime;
        }

        void MessageEnd()
        {
            if(_messages.Count == 0)
                return;

            Debug.Log("Message End");
            _messages.RemoveAt(0);
            _messageText.enabled = false;
            _timer = -1;

            MessageStart();
        }
    }
}
