using System.Collections.Generic;

namespace GardenShopOnline.Models
{
    public class ShoppingCartViewModels
    {
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}