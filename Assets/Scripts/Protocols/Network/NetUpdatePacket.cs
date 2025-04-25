using EEA.Define;
using EEA.Manager;
using EEA.UI;
using Protocols;
using Protocols.DTOs;
using System.Collections.Generic;
using TableData;
using UnityEngine;

namespace EEA.Protocols
{
    public class NetUpdatePacket
    {
        public void ParseUpdateData(ResponsePacketBase packet)
        {
            if (packet == null || packet.UpdateData == null)
                return;

            ParseItemUpdateData(packet.UpdateData.Items);
            ParseAccountUpdateData(packet.UpdateData.Account);
        }

        private void ParseAccountUpdateData(AccountDto account)
        {
            // if (account == null)
            //     return;

            // int prevLevelCode = GameManager.Instance.Account.LevelCode;
            
            // GameInfo.Instance.SetAccount(account);

            // if (prevLevelCode > 0 && prevLevelCode != account.LevelCode)
            // {
            //     string title = LocalizationUtil.GetName(92022001);
            //     int levelCode = GameInfo.Instance.Account.LevelCode;
            //     AccountTable accountTable = GameDataManager.Instance.GetData<AccountTable>(levelCode);
            //     if (accountTable == null)
            //     {
            //         Debug.LogError($"AccountTable is null - Level code: {levelCode}");
            //         return;
            //     }

            //     UIManager.Instance.CreateUIAsyncWithStart<UILevelupPopup>(AssetPathUI.UILevelupPopup, (res) => { });
            // }

            // UILobby ui = UIManager.Instance.GetUI<UILobby>();
            // if (ui != null)
            //     ui.Refresh();
        }

        private void ParseItemUpdateData(List<ItemDto> items)
        {
            if (items == null)
                return;            

            foreach (var item in items)
            {
                if (item.ItemCount > 0)
                    ItemManager.Instance.AddItem(item);
                else
                    ItemManager.Instance.RemoveItem(item);
            }

            UIManager.Instance.RefreshHeaderMatCount();
        }
    }
}
