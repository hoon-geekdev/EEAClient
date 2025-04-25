using EEA.Protocols;

namespace EEA.Manager
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        public NetAdmin Admin { get; private set; } = new NetAdmin();
        public NetAccount Account { get; private set; } = new NetAccount();
        public NetItem Item { get; private set; } = new NetItem();
        public NetUpdatePacket UpdatePacket { get; private set; } = new NetUpdatePacket();

        override protected void OnInit()
        {
            HTTPInstance.Instance.SetUpdatePacket(UpdatePacket.ParseUpdateData);
        }

        public void ResistRefreshToken()
        {
            // TimerManager.Instance.StopTimer(TokenRefresh);
            // TimerManager.Instance.AddTimer(3000f, 0f, TokenRefresh);
        }

        private void TokenRefresh(float elapsedTime)
        {
            Account.RefreshToken();
        }
    }
}
