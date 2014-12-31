using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	const string gameType = "ThisIsWar";
	int players=0;
	int playerID=0;
	HostData[] hostList;

	void Start()
	{
		MasterServer.ipAddress = "C192.168.0.104"; 
	}

	void StartServer(int numberOfPlayers=6,string roomName="Room", string password="")
	{
			Network.incomingPassword = password;
			Network.InitializeServer (numberOfPlayers, 25000, !Network.HavePublicAddress());
			MasterServer.RegisterHost (gameType, roomName);
			players=1;
	}

	void RefreshServerList()
	{
			MasterServer.RequestHostList (gameType);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
			if (msEvent == MasterServerEvent.HostListReceived)
					hostList = MasterServer.PollHostList();
	}

	void ConnectToServer(HostData server)
	{
		Network.Connect (server);
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
				players++;
				SendPlayers ();
	}

	void SendPlayers()
	{
			networkView.RPC ("SetPlayers", RPCMode.Others, players);
	}

	[RPC] void SetPlayers(int p)
	{
		players = p;
	}

	public int GetPlayerID()
	{
			return playerID;
	}

	void Update()
	{	

			if(playerID == 0 && Network.isClient)
			switch (players) {
				case 2:
						playerID = 1;
						break;
				case 3:
						playerID = 2;
						break;
				case 4:
						playerID = 3;
						break;
				case 5:
						playerID = 4;
						break;
				case 6:
						playerID = 5;
						break;
				}
	}

	void OnGUI()
	{
				if (!Network.isClient && !Network.isServer) {
						if (GUI.Button (new Rect (50, 50, 150, 100), "Start server"))
								StartServer ();
						if (GUI.Button (new Rect (50, 160, 150, 100), "Refresh server list"))
								RefreshServerList ();
						if (hostList != null) {
								int i=0;
								foreach (HostData server in hostList) {
										if (GUI.Button (new Rect (210, 50 + i * 110, 150, 100), server.gameName))
												ConnectToServer (server);
										i++;
								}
						}
				}
				if (Network.isServer || Network.isClient)
						GUI.Box (new Rect (200, 40, 90, 30), players.ToString () +" ID:"+GetPlayerID().ToString());
		}
}
