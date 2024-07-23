namespace ASI.Basecode.Resources.Constants
{
    /// <summary>
    /// Class for variables with constant values
    /// </summary>
    public class Const
    {
        /// <summary>
        ///API result success
        /// </summary>
        public const string ApiResultSuccess = "Success";

        /// <summary>
        ///API result error
        /// </summary>
        public const string ApiResultError = "Error";

        /// <summary>
        /// System
        /// </summary>
        public const string System = "sys";

        /// <summary>
        /// Api Key Header Name
        /// </summary>
        public const string ApiKey = "X-Basecode-API-Key";

        /// <summary>
        /// authentication scheme Name
        /// </summary>
        public const string AuthenticationScheme = "ASI_Basecode";

        /// <summary>
        /// authentication Issuer
        /// </summary>
        public const string Issuer = "asi.basecode";

        /// <summary>
        /// System's Guid
        /// </summary>
        public static readonly Guid SystemId = new Guid("f70950f4-910a-44f2-9819-70f38f27249e");
    }
}
