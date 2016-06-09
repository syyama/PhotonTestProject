using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Photon Unity Networkingの機能を色々試すことができるやつ
/// </summary>
public class PhotonManager : Photon.MonoBehaviour
{
    private string roomName = "";
	private string playerName = "";
	private string friendName = "";

	private string propKey = "";
	private string propValue = "";

	private string masterName = "";
	private string lobbyPropKey;

	private List<FriendInfo> friendList = new List<FriendInfo>();
	private List<string> propListInLobby = new List<string>();

	private string log = "";

	private float time = 0f;
	private bool raiseFlg = false;

	/// <summary>
	/// マスターサーバに接続された際に呼ばれるイベントコールバック
	/// </summary>
	void OnConnectedToMaster()
	{
		Debug.Log("[Callback]: OnConnectedToMaster");
		log += "[Callback]: OnConnectedToMaster\n";
	}

	/// <summary>
	/// ロビーに入った際に呼ばれるイベントコールバック
	/// </summary>
    void OnJoinedLobby()
    {
        Debug.Log("[Callback]: OnPhotonJoiendLobby");
        log += "[Callback]: OnPhotonJoiendLobby\n";
    }

	/// <summary>
	/// ルームに入った際に呼ばれるイベントコールバック
	/// </summary>
    void OnJoinedRoom()
    {
        Debug.Log("[Callback]: OnPhotonJoiendRoom");
        log += "[Callback]: OnPhotonJoiendRoom\n";
    }

	/// <summary>
	/// ルームから出た際に呼ばれるイベントコールバック
	/// </summary>
    void OnLeftRoom()
    {
        Debug.Log("[Callback]: OnLeftRoom");
        log += "[Callback]: OnLeftRoom\n";
    }

	/// <summary>
	/// Photonから切断された際に呼ばれるイベントコールバック
	/// </summary>
    void OnDisconnectedFromPhoton()
    {
        Debug.Log("[Callback]: OnDisconnectedFromPhoton");
        log += "[Callback]: OnDisconnectedFromPhoton\n";
    }

	/// <summary>
	/// ルームの入室に失敗した際に呼ばれるイベントコールバック
	/// </summary>
	/// <param name="codeAndMsg">Code and message.</param>
    void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonJoinRoomFailed");
        log += "[Callback]: OnPhotonJoinRoomFailed\n";
        int errorCode = (int)codeAndMsg[0];
        Debug.Log(errorCode);
        log += "[Error]: " + errorCode + "\n";

    }

	/// <summary>
	/// JoinRandomRoomが失敗した際に呼ばれるイベントコールバック
	/// </summary>
	/// <param name="codeAndMsg">Code and message.</param>
    void OnPhotonJoinRandomRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonJoinRandomRoomFailed");
        log += "[Callback]: OnPhotonJoinRandomRoomFailed\n";
        int errorCode = (int)codeAndMsg[0];
        Debug.Log(errorCode);
    }

	/// <summary>
	/// ルーム作成に失敗した際に呼ばれるイベントコールバック
	/// </summary>
	/// <param name="codeAndMsg">Code and message.</param>
    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonCreateRoomFailed");
        log += "[Callback]: OnPhotonCreateRoomFailed\n";
        int errorCode = (int)codeAndMsg[0];
        Debug.Log(errorCode);
    }

	/// <summary>
	/// PhotonNetwork.Friendsが変更された際に呼ばれるイベントコールバック
	/// </summary>
    void OnUpdatedFriendList()
    {
        Debug.Log("OnUpdatedFriendList");
        log += "[Callback]: OnUpdatedFriendList\n";
    }

	/// <summary>
	/// プレイヤーのカスタムプロパティが変更された際に呼ばれるイベントコールバック
	/// </summary>
	/// <param name="playerAndUpdatedProps">プレイヤー</param>
	void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		Debug.Log("OnPhotonPlayerPropertiesChanged");
		log += "[Callback]: OnPhotonPlayerPropertiesChanged\n";

		PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
		//Hashtable props = playerAndUpdatedProps[1] as Hashtable;

		Debug.Log("[Parameters]: " + player.name + "のカスタムプロパティが変更されました");
		log += "[Parameters]: " + player.name + "のカスタムプロパティが変更されました\n";
	}

	// 6秒おきにイベント発生、フラグが成立するとイベントが起こる
	void Update()
	{
		time += Time.deltaTime;
		if (raiseFlg && time > 6f) {
			byte evCode = 0;
			byte[] content = new byte[] { 1,1,1,1 };
			bool reliable = true;
			PhotonNetwork.RaiseEvent(evCode, content, reliable, null);

			Debug.Log("[Method]: PhotonNetwork.RaiseEvent()");
			log += "[Method]: PhotonNetwork.RaiseEvent\n";
			time = 0f;
		}
	}

    void OnGUI()
    {

		GUILayout.BeginArea(new Rect(0, 0, 400, 800));

		// Photonのネットワーク状態
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

		// プレイヤー名
        GUILayout.BeginHorizontal();
        GUILayout.Label("プレイヤー名: ");
        playerName = GUILayout.TextField(playerName);
		PhotonNetwork.playerName = playerName;
        GUILayout.EndHorizontal();

		// 接続管理
		GUILayout.BeginHorizontal();
		// Photonに接続
        if (GUILayout.Button("Connect"))
        {
			// WebSocketを使う場合はプロトコルを明示的に指定する
			//PhotonNetwork.PhotonServerSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.WebSocket; 
            PhotonNetwork.ConnectUsingSettings("0.1");

			Debug.Log("[Method]: ConnectUsingSettings()");
			log += "[Method]: ConnectUsingSettings\n";

        }
		// Photonから切断
        if (GUILayout.Button("Disconnect"))
        {
            PhotonNetwork.Disconnect();

			Debug.Log("[Method]: Disconnect()");
			log += "[Method]: Disconnect\n";
        }
		GUILayout.EndHorizontal();


		// ロビー入退室周り
		GUILayout.BeginHorizontal();
		// ロビー1に入室
		if (GUILayout.Button ("JoinLobby1")) {
			PhotonNetwork.JoinLobby(new TypedLobby("Lobby1", new LobbyType()));
			Debug.Log("[Parameters]: PhotonNetwork.lobby.Name" + PhotonNetwork.lobby.Name);
			log += "[Parameters]: PhotonNetwork.lobby.Name: " + PhotonNetwork.lobby.Name + "\n";
		}
		// ロビー2に入室
		if (GUILayout.Button ("JoinLobby2")) {
			PhotonNetwork.JoinLobby(new TypedLobby("Lobby2", new LobbyType()));
			Debug.Log("[Parameters]: PhotonNetwork.lobby.Name" + PhotonNetwork.lobby.Name);
			log += "[Parameters]: PhotonNetwork.lobby.Name: " + PhotonNetwork.lobby.Name + "\n";
		}
		// ロビーから退室
		if (GUILayout.Button ("LeaveLobby")) {
			PhotonNetwork.LeaveLobby ();
		}
		GUILayout.EndHorizontal();

		// ルーム名の入力
        GUILayout.BeginHorizontal();
        GUILayout.Label("ルーム名");
        roomName = GUILayout.TextField(roomName);
        GUILayout.EndHorizontal();

		// ルーム作成
		GUILayout.BeginHorizontal();
		// ルーム名を指定いて作成
        if (GUILayout.Button("CreateRoom"))
        {
            Debug.Log("[Method]: CreateRoom() :" + roomName);
            log += "[Method]: CreateRoom : " + roomName + "\n";

			// カスタムプロパティを指定
            Hashtable customRoomProperties = new Hashtable() { { "map", 1 } };
            string[] customRoomPropertiesForLobby = { "map", "ai" };

			// ルームオプションを指定して作成
            PhotonNetwork.CreateRoom(roomName, new RoomOptions()
            {
                isVisible = false,
                isOpen = true,
                maxPlayers = 20,
                customRoomProperties = customRoomProperties,
                customRoomPropertiesForLobby = customRoomPropertiesForLobby
            }, new TypedLobby());

			Debug.Log ("ネットワークを切断します...");
        }
		// Room1としてルームを作成
        if (GUILayout.Button("CreateRoom as Room1"))
        {
            Debug.Log("[Method]: CreateRoom() : Room1");
            log += "[Method]: CreateRoom : Room1\n";

			PhotonNetwork.CreateRoom("Room1", new RoomOptions(), new TypedLobby());
        }
		// ルーム名を指定して作成、すでに存在する場合は入室
        if (GUILayout.Button("JoinOrCreateRoom"))
        {
            Debug.Log("[Method]: JoinOrCreateRoom()");
            log += "[Method]: JoinOrCreateRoom\n";

            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), new TypedLobby());
        }
		GUILayout.EndHorizontal();

		// ルーム入退室
		GUILayout.BeginHorizontal();
		// ルーム名を指定して入室
		if (GUILayout.Button("JoinRoom"))
		{
			Debug.Log("[Method]: CreateRoom() : Room1");
			log += "[Method]: CreateRoom : Room1\n";

			PhotonNetwork.JoinRoom (roomName);
		}
		// ランダムに入室
        if (GUILayout.Button("JoinRandomRoom"))
        {
            Debug.Log("[Method]: JoinRandomRoom()");
            log += "[Method]: JoinRandomRoom\n";

            PhotonNetwork.JoinRandomRoom();
        }
		// ルームから退室
        if (GUILayout.Button("LeaveRoom"))
        {
            Debug.Log("[Method]: LeaveRoom()");
            log += "[Method]: LeaveRoom\n";

            PhotonNetwork.LeaveRoom();
        }
		GUILayout.EndHorizontal();

		// フレンド名を入力
        GUILayout.BeginHorizontal();
        GUILayout.Label("登録するフレンド名: ");
        friendName = GUILayout.TextField(friendName);
        GUILayout.EndHorizontal();

        // フレンド関係
		GUILayout.BeginHorizontal();
		// フレンドを登録
        if (GUILayout.Button("SetFriendName"))
        {
            Debug.Log("[Method]: SetFriendName(): " + friendName + "をフレンド登録しました");
            log += "[Method]: SetFriendName(): " + friendName + "をフレンド登録しました\n";

            friendList.Add(new FriendInfo() { Name = friendName });
            PhotonNetwork.Friends = friendList;
            Debug.Log("test");
        }
        // フレンドリストの取得
        if (GUILayout.Button("PhotonNetwork.Friends"))
        {
            Debug.Log("[Parameters]: PhotonNetwork.Friends");
            log += "[Parameters]: PhotonNetwork.Friends\n";

            List<FriendInfo> friends = PhotonNetwork.Friends;
            foreach (FriendInfo friend in friends)
            {
                Debug.Log("Name: " + friend.Name);
                log += "Name: " + friend.Name + ", IsOnline: " + friend.IsOnline + "\n";
            }
        }
		GUILayout.EndHorizontal();

		// その他機能
		GUILayout.BeginHorizontal();
        // インスタンスの作成
        if (GUILayout.Button("Instantiate"))
        {
            Debug.Log("[Method]: Instantiate()");
            log += "[Method]: Instantiate\n";

            PhotonNetwork.Instantiate("Cube", Vector3.zero, Quaternion.identity, 0);
        }

        // オフラインモードの設定
        if (GUILayout.Button("Offline Mode"))
        {
            Debug.Log("[Parameters]: Offline Mode: " + !PhotonNetwork.offlineMode);
            log += "[Parameters]: Offline Mode: " + !PhotonNetwork.offlineMode + "\n";

            PhotonNetwork.offlineMode = !PhotonNetwork.offlineMode;
        }
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
        // ルームリストの取得
        if (GUILayout.Button("GetRoomList()"))
        {
            Debug.Log("[Method]: GetRoomList()");
            log += "[Method]: GetRoomList()\n";

            RoomInfo[] roomList = PhotonNetwork.GetRoomList();
            foreach (RoomInfo roomInfo in roomList)
            {
                Debug.Log("Name: " + roomInfo.name + " ,Open: " + roomInfo.open.ToString() + " ,Visible" + roomInfo.visible.ToString());
                log += "Name: " + roomInfo.name + " ,Open: " + roomInfo.open.ToString() + " ,Visible" + roomInfo.visible.ToString() + "\n";
            }
        }
        // プレイヤーリストの取得
        if (GUILayout.Button("PhotonNetwork.playerList"))
        {
            Debug.Log("[Parameters]: PhotonNetwork.playerList");
            log += "[Parameters]: PhotonNetwork.playerList\n";

            PhotonPlayer[] playerList = PhotonNetwork.playerList;
            foreach (PhotonPlayer player in playerList)
            {
                Debug.Log("Name: " + player.name + " ,isMasterClient: " + player.isMasterClient.ToString());
                log += "Name: " + player.name + " ,isMasterClient: " + player.isMasterClient.ToString() + "\n";
            }
        }
		GUILayout.EndHorizontal();

		// カスタムプロパティ
		GUILayout.BeginHorizontal();
		GUILayout.Label("Key: ");
		propKey = GUILayout.TextField(propKey);
		GUILayout.Label("Value: ");
		propValue = GUILayout.TextField (propValue);
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("プレイヤーのカスタムプロパティ");
		if (GUILayout.Button("SetCustomProperties")) {
			Hashtable playerProp = new Hashtable() { { propKey, propValue} };
			foreach(PhotonPlayer otherPlayer in PhotonNetwork.playerList) {
				otherPlayer.SetCustomProperties (playerProp);
				Debug.Log("[Parameters]: SetCustomProperties[" + playerProp);
				log += "[Parameters]: SetCustomProperties[" + playerProp + "\n";
			}
		}
		GUILayout.EndHorizontal ();

		// ルームのカスタムプロパティ
		GUILayout.BeginHorizontal();
		GUILayout.Label ("ルームのカスタムプロパティ");
		if (GUILayout.Button("SetCustomProperties")) {
			Hashtable roomProp = new Hashtable() { { propKey, propValue} };
			PhotonNetwork.room.SetCustomProperties (roomProp);
			Debug.Log("[Parameters]: SetCustomProperties[" + roomProp);
			log += "[Parameters]: SetCustomProperties[" + roomProp + "\n";
		}
		GUILayout.EndHorizontal ();

		// ルームのカスタムプロパティをロビーから見えるようにする

		lobbyPropKey = GUILayout.TextField(lobbyPropKey);

		if (GUILayout.Button ("SetPropertiesListedInLobby")) {
			propListInLobby.Add (lobbyPropKey);
			string[] propListInLobbyStr = new string[propListInLobby.Count];

			for (int i = 0; i < propListInLobby.Count-1; i++) {
				propListInLobbyStr [i] = propListInLobby [i];
				PhotonNetwork.room.SetPropertiesListedInLobby (propListInLobbyStr);
				lobbyPropKey = "";

				Debug.Log("[Parameters]: SetPropertiesListedInLobby[" + i + "]: " + propListInLobbyStr [i]);
				log += "[Parameters]: SetPropertiesListedInLobby[" + i + "]: " + propListInLobbyStr [i] + "\n";
			}

		}

		// マスタークライアントの設定
		GUILayout.BeginHorizontal ();
		GUILayout.Label("マスタークライアント: ");
		masterName = GUILayout.TextField(masterName);

		if (GUILayout.Button ("SetMasterClient")) {
			foreach (PhotonPlayer player in PhotonNetwork.playerList) {
				if (player.name == masterName) {
					PhotonNetwork.SetMasterClient (player);
					Debug.Log("[Parameters]: PhotonNetwork.SetMasterClient: " + masterName);
					log += "[Parameters]: PhotonNetwork.SetMasterClient:" + masterName + "\n";
				}
			}
		}
		GUILayout.EndHorizontal ();

		// ルームの設定変更
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Visible")) {
			// Visible = trueの場合はGetRoomListにリストされる
			PhotonNetwork.room.visible = !PhotonNetwork.room.visible;
			Debug.Log("[Parameters]: PhotonNetwork.room.visible: " + PhotonNetwork.room.visible);
			log += "[Parameters]: PhotonNetwork.room.visible:" + PhotonNetwork.room.visible + "\n";
		}
		if (GUILayout.Button ("Open")) {
			// Visible = trueの場合はルームに入室できるようになる
			PhotonNetwork.room.open = !PhotonNetwork.room.open;
			Debug.Log("[Parameters]: PhotonNetwork.room.open: " + PhotonNetwork.room.open);
			log += "[Parameters]: PhotonNetwork.room.open:" + PhotonNetwork.room.open + "\n";
		}
		GUILayout.EndHorizontal ();

		// RPC関係
		GUILayout.BeginHorizontal ();
		// 自分を含めて全員にRPCを送信します
		if (GUILayout.Button ("RPC to All")) {
			photonView.RPC("RPCMethod", PhotonTargets.All, "Hello RPC Method!");
		}
		// 自分以外の全員にRPCを送信します
		if (GUILayout.Button ("RPC to Others")) {
			photonView.RPC("RPCMethod", PhotonTargets.Others, "Hello RPC Method!");
		}
		// マスタークライアントにのみRPCを送信します
		if (GUILayout.Button ("RPC to MasterClient")) {
			photonView.RPC("RPCMethod", PhotonTargets.MasterClient, "Hello RPC Method!");
		}
		GUILayout.EndHorizontal ();

		// RaiseEvent関係
		GUILayout.BeginHorizontal ();
		// イベントを発生するフラグを設定
		if (GUILayout.Button ("RaiseEvent")) {
			raiseFlg = !raiseFlg;
			Debug.Log("[Parameters]: raiseFlg: " + raiseFlg);
			log += "[Parameters]: raiseFlg:" + raiseFlg + "\n";
		}
		GUILayout.EndHorizontal ();

		// ログのクリア
		if (GUILayout.Button ("ClearLog")) {
			log = "";
		}
        
		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect(450, 0, 350, 800));
        GUILayout.Label(log);	
        GUILayout.EndArea();
    }

	/// <summary>
	/// RPCのパラメータを表示するためのメソッド
	/// </summary>
	/// <param name="param">表示する文字列</param>
	[PunRPC]
	public void RPCMethod(string param) {
		// RPCのパラメータを表示します
		Debug.Log("[Method]: RPCMethod: " + param);
		log += "[Method]: RPCMethod: " + param;
	}
}
