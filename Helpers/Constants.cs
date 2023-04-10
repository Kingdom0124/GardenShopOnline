using System.Collections.Generic;

namespace GardenShopOnline.Helpers
{
    public class Constants
    {
        /// <summary>
        /// Account Web
        /// </summary>
        public const string ACCOUNT_BONSAIGARDEN = "bonsaigarden6@gmail.com";

        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary>
        public static int WAIT_FOR_CONFIRMATION = 1;
        public static int APPROVED = 2;
        public static int DELIVERING = 3;
        public static int COMPLETED = 4;
        public static int PAY_IN_ADVANCE = 5;
        public static int CANCELED = 6;
        public static readonly Dictionary<int, string> StateOder = new Dictionary<int, string> {
            {WAIT_FOR_CONFIRMATION, "Wait for confirmation"},
            {APPROVED, "Approved" },
            {DELIVERING, "Delivering" },
            {COMPLETED, "Completed" },
            {PAY_IN_ADVANCE, "Pay in advance" },
            {CANCELED, "Canceled" }
        };

        /// <summary>
        /// Trạng thái ẩn/hiện
        /// </summary>
        public static int SHOW_STATUS = 1;
        public static int HIDDEN_STATUS = 2;
        public static int DELETED_STATUS = 3;


        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public static int BANK_METHOD = 2;
        public static int CASH_METHOD = 1;

        /// <summary>
        /// Trạng thái comment
        /// </summary>
        public static int NEW_COMMENT = 1;
        public static int APPROVED_COMMENT = 2;
        public static int REFUSED_COMMENT = 3;

        /// <summary>
        /// Loại tin nhắn
        /// </summary>
        public static int TYPE_TEXT = 1;
        public static int TYPE_IMAGE = 2;
    }
}