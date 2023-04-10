using System.Collections.Generic;

namespace GardenShopOnline.Models
{
    public class CustomerOrderViewModels
    {
        public CustomerOrder CustomerOrder { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}