namespace TL4_SHOP.Data
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int SenderId { get; set; }        // Id người gửi
        public int ReceiverId { get; set; }      // Id người nhận
        public string SenderName { get; set; }   // Tên người gửi (Admin / Khách)
        public string Content { get; set; }      // Nội dung tin nhắn
        public DateTime Timestamp { get; set; }  // Thời gian gửi
    }
}
