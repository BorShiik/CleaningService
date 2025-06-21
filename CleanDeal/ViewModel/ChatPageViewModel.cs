using CleanDeal.DTOs;
using CleanDeal.Models;
using System.Collections.Generic;

namespace CleanDeal.ViewModel
{
    public class ChatPageViewModel
    {
        public List<CleaningOrder> Orders { get; set; }
        public int SelectedOrderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public List<ChatMessageDTO> Messages { get; set; }
        public string ReceiverId { get; set; }
        public ApplicationUser Receiver { get; set; }
    }
}