namespace StockMarketLive.Domain.Constants;

/// <summary>
/// Projedeki sihirli metinlerin (magic strings) barındığı sabitler sınıfı. (Sıfır Hardcode kuralı)
/// </summary>
public static class AppConstants
{
    public const string CorsPolicyName = "AllowFrontend";
    
    public static class RabbitMq
    {
        public const string ExchangeName = "stock.market.exchange";
        public const string QueueName = "stock.market.live.queue";
    }

    public static class SignalR
    {
        public const string HubEndpoint = "/hubs/stock";
        public const string ReceiveEventName = "ReceiveStockUpdate";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
    }

    public static class ErrorCodes
    {
        public static class Auth
        {
            public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
            public const string UserAlreadyExists = "AUTH_USER_ALREADY_EXISTS";
            public const string UserNotFound = "AUTH_USER_NOT_FOUND";
            
            // Validation
            public const string UsernameEmpty = "AUTH_USERNAME_EMPTY";
            public const string UsernameTooShort = "AUTH_USERNAME_TOO_SHORT";
            public const string PasswordEmpty = "AUTH_PASSWORD_EMPTY";
            public const string PasswordTooShort = "AUTH_PASSWORD_TOO_SHORT";
            public const string EmailEmpty = "AUTH_EMAIL_EMPTY";
            public const string EmailInvalid = "AUTH_EMAIL_INVALID";
        }

        public static class Role
        {
            public const string RoleAlreadyExists = "ROLE_ALREADY_EXISTS";
            public const string RoleNotFound = "ROLE_NOT_FOUND";
            public const string PermissionNotFound = "PERMISSION_NOT_FOUND";
            public const string UserAlreadyHasRole = "USER_ALREADY_HAS_ROLE";
            public const string RoleAlreadyHasPermission = "ROLE_ALREADY_HAS_PERMISSION";

            // Validation
            public const string NameEmpty = "ROLE_NAME_EMPTY";
            public const string NameTooShort = "ROLE_NAME_TOO_SHORT";
        }
        
        public static class General
        {
            public const string InvalidRequest = "GENERAL_INVALID_REQUEST";
        }
    }
}
