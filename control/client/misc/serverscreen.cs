//============================================================================
// control/client/misc/ServerScreen.cs
//
// Server query code module for 3D3E koob sample game
//
// Copyright (c) 2003, 2011 by Kenneth C.  Finney.
//============================================================================

//----------------------------------------
function ServerScreen::onWake()
{
   // Double check the status. Tried setting this the control
   // inactive to start with, but that didn't seem to work.
   SSJoinServer.SetActive(SSServerList.rowCount() > 0);
   ServerScreen.QueryLan();
}

//----------------------------------------
function ServerScreen::QueryLan(%this)
{
   QueryLANServers(
      28000,      // lanPort for local queries
      0,          // Query flags
      $Client::GameTypeQuery,       // gameTypes
      $Client::MissionTypeQuery,    // missionType
      0,          // minPlayers
      100,        // maxPlayers
      0,          // maxBots
      2,          // regionMask
      0,          // maxPing
      100,        // minCPU
      0           // filterFlags
      );
}


//----------------------------------------
function ServerScreen::Cancel(%this)
{
   CancelServerQuery();
}


//----------------------------------------
function ServerScreen::Join(%this)
{
   CancelServerQuery();
   %id = SSServerList.GetSelectedId();

   // The server info index is stored in the row along with the
   // rest of displayed info.
   %index = getField(SSServerList.GetRowTextById(%id),6);
   if (SetServerInfo(%index)) {
      %conn = new GameConnection(ServerConnection);
      %conn.SetConnectArgs($pref::Player::Name);
      %conn.SetJoinPassword($Client::Password);
      %conn.Connect($ServerInfo::Address);
   }
}

//----------------------------------------
function ServerScreen::Close(%this)
{
   cancelServerQuery();
   Canvas.SetContent(MenuScreen);
}

//----------------------------------------
function ServerScreen::Update(%this)
{
   // Copy the servers into the server list.
   SSQueryStatus.SetVisible(false);
   SSServerList.Clear();
   %sc = getServerCount();
   for (%i = 0; %i < %sc; %i++) {
      setServerInfo(%i);
      SSServerList.AddRow(%i,
         ($ServerInfo::Password? "Yes": "No") TAB
         $ServerInfo::Name TAB
         $ServerInfo::Ping TAB
         $ServerInfo::PlayerCount @ "/" @ $ServerInfo::MaxPlayers TAB
         $ServerInfo::Version TAB
         $ServerInfo::GameType TAB
         %i);  // ServerInfo index stored also
   }
   SSServerList.Sort(0);
   SSServerList.SetSelectedRow(0);
   SSServerList.scrollVisible(0);

   SSJoinServer.SetActive(SSServerList.rowCount() > 0);
}

//----------------------------------------
function onServerQueryStatus(%status, %msg, %value)
{
   // Update query status
   // States: start, update, ping, query, done
   // value = % (0-1) done for ping and query states
   if (!SSQueryStatus.IsVisible())
      SSQueryStatus.SetVisible(true);

   switch$ (%status) {
      case "start":

      case "ping":
         SSStatusText.SetText("Finding Hosts");
         SSStatusBar.SetValue(%value);

      case "query":

      case "done":
         SSQueryStatus.SetVisible(false);
         ServerScreen.update();
   }
}
