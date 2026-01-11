using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SNEngine.Source.SNEngine.MessageSystem
{
    public class MessageView : MonoBehaviour
    {
        [SerializeField] private Image _bubbleImage;
        [SerializeField] private TextMeshProUGUI _characterName;
        [SerializeField] private TextMeshProUGUI _printTime;
        public MessagePrinterText Printer => _printer;
        [SerializeField] private MessagePrinterText _printer;
        private TextMeshProUGUI _messageText;

        public void ShowMessage(string text)
        {
            gameObject.SetActive(true);
            _printer.Print(text);
            _printTime.text = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}