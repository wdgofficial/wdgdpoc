namespace FiiiChain.RabbitMQ
{
    public class RabbitMqSetting
    {
        public static readonly string HOSTNAME = "localhost";
        public static readonly string USERNAME = "king";
        public static readonly string PASSWORD = "880505";
        public static readonly string EXCHANGENAME = "direct";


        public static readonly string CONNECTIONSTRING = "amqp://king:11111@localhost:5672/king";

    }

    public class RabbitMqName
    {
        public const string StartMining = "StartMining";
        public const string StopMining = "StopMining";
        public const string Login = "Login";
        public const string ForgetBlock = "ForgetBlock";
        public const string HeartPool = "HeartPool";
        public const string Default = "Default";
        public const string FiiiPosInviteReward = "FiiiPosInviteReward";
    }
}
