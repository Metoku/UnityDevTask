using TestTask.NonEditable;
using UnityEngine;

namespace TestTask.Editable
{
    public static class ServerPacketsHandler
    {
        #region Packet Handlers
        public static void LoginRequest(Packet packet)
        {
            var clientLogInResponse = ServerMock.Instance.TryConnectClient(out var clientId);
            SendLoginResponse(clientLogInResponse, clientId);

            if (clientLogInResponse == LoginResponse.Success)
            {
                var mobsManager = ServerMock.Instance.ServerMobsManager;

                SendMonsterSpawn(ServerMock.Instance.ServerMobsManager.MonsterData);

                //subscribe to monster death event
                mobsManager.MonsterData.MonsterDeath += MonsterDeathHandle;
            }
        }

        public static void DamageRequest(Packet packet)
        {
            int monsterID = packet.ReadInt();
            var serverMonster = ServerMock.Instance.ServerMobsManager.MonsterData;

            if (serverMonster != null && serverMonster.MonsterId == monsterID)
            {
                serverMonster.TakeDamage(20f);

                SendMonsterUpdate(serverMonster);
            }
        }
        public static void MonsterDeathHandle()
        {
            //retrieve new monster and send the it to spawn after the current one dies
            var newMonster = ServerMock.Instance.ServerMobsManager.MonsterData;
            SendMonsterSpawn(newMonster);

            newMonster.MonsterDeath -= MonsterDeathHandle;
            newMonster.MonsterDeath += MonsterDeathHandle;
        }

        public static void ColorRequest(Packet packet)
        {
            var colors = ServerMock.Instance.ServerColors.GeneratedColors;

            using (Packet response = new Packet(6))
            {
                response.Write(colors.Count);

                foreach (Color color in colors)
                {
                    response.Write(color.r);
                    response.Write(color.g);
                    response.Write(color.b);
                    response.Write(color.a);
                }
                ServerMock.Instance.PacketSenderServer.SendToClient(response);
            }
        }
        #endregion

        #region Packet Senders
        public static void SendLoginResponse(LoginResponse response, int clientId)
        {
            using (Packet packet = new Packet(1))
            {
                packet.Write((int)response);
                packet.Write(clientId);

                ServerMock.Instance.PacketSenderServer.SendToClient(packet);
            }
        }
        public static void SendMonsterSpawn(MonsterData monsterData)
        {
            using (Packet packet = new Packet(2))
            {
                packet.Write(monsterData.MonsterId);
                packet.Write((int)monsterData.MonsterType);
                packet.Write(monsterData.MonsterMaxHealth);
                packet.Write(monsterData.MonsterCurrentHealth);

                ServerMock.Instance.PacketSenderServer.SendToClient(packet);
            }
        }

        public static void SendMonsterUpdate(MonsterData monster)
        {
            using (Packet packet = new Packet(4))
            {
                packet.Write(monster.MonsterId);
                packet.Write(monster.MonsterCurrentHealth);
                ServerMock.Instance.PacketSenderServer.SendToClient(packet);
            }
        }
        #endregion
    }
}

public enum LoginResponse
{
    Success = 0,
    Failure = 1,
}