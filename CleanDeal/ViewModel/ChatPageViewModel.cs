using CleanDeal.DTOs;

public class ChatPageViewModel
{
    public IEnumerable<OrderListDTO> Orders { get; set; } = [];
    public int? SelectedOrderId { get; set; }
    public IEnumerable<ChatMessageDTO>? Messages { get; set; }
    public string? ReceiverId { get; set; }

}