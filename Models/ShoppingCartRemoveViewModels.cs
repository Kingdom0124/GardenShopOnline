namespace GardenShopOnline.Models
{
    public class ShoppingCartRemoveViewModels
    {
        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public decimal ItemTotal { get; set; }
        public int DecreaseId { get; set; }
        public int DeleteId { get; set; }
    }
}