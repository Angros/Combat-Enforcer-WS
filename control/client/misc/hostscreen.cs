

//----------------------------------------
function StartHost()
{
   %id = HostMissionList.getSelectedId();
   %mission = getField(HostMissionList.getRowTextById(%id), 1);

   createServer("MultiPlayer", %mission);
   %conn = new GameConnection(ServerConnection);
   RootGroup.add(ServerConnection);
   %conn.setConnectArgs($pref::Player::Name);
   %conn.setJoinPassword($Client::Password);
   %conn.connectLocal();
}


//----------------------------------------
function HostScreen::onWake()
{
   HostMissionList.clear();
   %i = 0;
   for(%file = findFirstFile($Server::MissionFileSpec);  %file !$= ""; %file = findNextFile($Server::MissionFileSpec))
      if (strStr(%file, "assets/maps/") != -1 )
         HostMissionList.addRow(%i++, getMissionDisplayName(%file) @ "\t" @ %file );
   HostMissionList.sort(0);
   HostMissionList.setSelectedRow(0);
   HostMissionList.scrollVisible(0);
}


