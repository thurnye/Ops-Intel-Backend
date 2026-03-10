namespace OperationIntelligence.Core
{
    public static class AuthConstants
    {
        public const int AccessTokenLifetimeMinutes = 30;
        public const int RefreshTokenLifetimeDays = 7;
        public const int EmailVerificationLifetimeHours = 24;
        public const int PasswordResetLifetimeMinutes = 30;
        public const int PasswordHistoryDepth = 5;

        public const int MaxFailedAccessAttempts = 5;
        public const int LockoutMinutes = 15;
    }
}
