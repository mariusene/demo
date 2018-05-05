namespace UCB.JapanCimzia.API.Messages
{
    public class OrderMessage
    {
        public int Id { get; set; }
        public CustomerMessage Customer { get; set; }
        public string Merchand { get; set; }
    }
}