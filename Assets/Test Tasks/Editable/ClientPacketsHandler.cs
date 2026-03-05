using System;
using System.Collections.Generic;
using TestTask.NonEditable;
using UnityEngine;

namespace TestTask.Editable
{
    public static class ClientPacketsHandler
    {
        #region Packet Handlers
        public static void LoginDataReceived(Packet packet)
        {
            int responseCode = packet.ReadInt();
            int clientId = packet.ReadInt();

            ClientManager.Instance.SetClientLogInStatus(responseCode, clientId);
        }
        public static void MonsterDataReceived(Packet packet)
        {
            int id = packet.ReadInt();
            int typeInt = packet.ReadInt();
            float maxHealth = packet.ReadFloat();
            float currentHealth = packet.ReadFloat();

            ClientManager.Instance.ClientMobsManager.UpdateMonster(id, typeInt, maxHealth, currentHealth);
        }
        public static void MonsterUpdateReceived(Packet packet)
        {
            int monsterId = packet.ReadInt();
            float newHP = packet.ReadFloat();

            if (ClientManager.Instance.ClientMobsManager.CurrentMonsterId == monsterId)
            {
                ClientManager.Instance.ClientMobsManager.UpdateHealthBar(newHP);
            }
        }
        public static void ColorsReceived(Packet packet)
        {
            int count = packet.ReadInt();
            List<Color> receivedColors = new List<Color>();

            for (int colorIndex = 0; colorIndex < count; colorIndex++)
            {
                float r = packet.ReadFloat();
                float g = packet.ReadFloat();
                float b = packet.ReadFloat();
                float a = packet.ReadFloat(); 
                receivedColors.Add(new Color(r, g, b, a));
            }

            ClientManager.Instance.ClientColorManager.UpdateColors(receivedColors);
        }

        #endregion

        #region Packet Senders
        public static void SendLoginRequest()
        {
            Packet packet = new Packet(1);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }
        public static void SendDamageRequest(int monsterID)
        {
            using (Packet packet = new Packet(3))
            {
                packet.Write(monsterID);
                ClientManager.Instance.PacketSenderClient.SendToServer(packet);
            }
        }
        public static void SendColorRequest()
        {
            using (Packet packet = new Packet(5))
            {
                ClientManager.Instance.PacketSenderClient.SendToServer(packet);
            }
        }
        #endregion
    }
}
