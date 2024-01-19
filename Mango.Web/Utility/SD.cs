namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBaseUrl { get; set; }
        public static string AuthAPIBaseUrl { get; set; }
        public static string ProductAPIBaseUrl { get; set; }

        public const string RoleAdmin = "Admin";
        public const string RoleCustomer = "Customer";
        public const string TokenCookie = "JWTToken";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
