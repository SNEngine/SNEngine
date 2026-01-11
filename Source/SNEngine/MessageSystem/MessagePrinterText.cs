namespace SNEngine.Source.SNEngine.MessageSystem
{
    public class MessagePrinterText : PrinterText
    {
        public override void Hide()
        {
            GetComponentInParent<MessageView>().gameObject.SetActive(false);
        }
    }
}