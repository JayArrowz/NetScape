namespace ASPNetScape.Modules.LoginProtocolThreeOneSeven.IO.Model
{
    public enum LoginStatus
    {
        StatusExchangeData = 0,
        StatusDelay = 1,
        StatusOk = 2,
        StatusInvalidCredentials = 3,
        StatusAccountDisabled = 4,
        StatusAccountOnline = 5,
        StatusGameUpdated = 6,
        StatusServerFull = 7,
        StatusLoginServerOffline = 8,
        StatusTooManyConnections = 9,
        StatusBadSessionId = 10,
        StatusLoginServerRejectedSession = 11,
        StatusMembersAccountRequired = 12,
        StatusCouldNotComplete = 13,
        StatusUpdating = 14,
        StatusReconnectionOk = 15,
        StatusTooManyLogins = 16,
        StatusInMembersArea = 17,
        StatusInvalidLoginServer = 20,
        StatusProfileTransfer = 21,
        TypeStandard = 16,
        TypeReconnection = 18
    }
}
