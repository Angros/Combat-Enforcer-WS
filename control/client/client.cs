//============================================================================
// control/client/client.cs
//
// This module contains client specific code for handling
// the set up and operation of the player's in-game interface.
//
// 3D3E emaga6 tutorial game
//
//  Copyright (c) 2003, 2006, 2011 by Kenneth C. Finney.
//============================================================================


//----------------------------------------------------------------------------
// Utility function necessary to quit game in the Demo only - would not be 
// used with the full version SDK.
//----------------------------------------------------------------------------
function quitGame()
{
   ServerConnection.delete();
   showCursor();
   quit();
   
   if ($Server::Dedicated)
      destroyServer();
   else
      disconnect();

   Parent::onExit();
}

function LaunchGame()
{
   createServer("SinglePlayer", "assets/maps/KoobB.mis");
   %conn = new GameConnection(ServerConnection);
   %conn.setConnectArgs("Reader");
   %conn.connectLocal();
}

function ShowMenuScreen()
{
   // Startup the client with the menu...
   Canvas.setContent( MenuScreen );
   Canvas.setCursor("DefaultCursor");
}

function SplashScreenInputCtrl::onInputEvent(%this, %dev, %evt, %make)
{
   if(%make)
   {
     ShowMenuScreen();
   }
}

//============================================================================

//============================================================================
// The following functions are called from the client core code modules.
// These stubs are added here to prevent warning messages from cluttering
// up the log file.
//============================================================================
function onServerMessage()
{
}
function onMissionDownloadPhase1()
{
}
function onPhase1Progress()
{
}
function onPhase1Complete()
{
}
function onMissionDownloadPhase2()
{
}
function onPhase2Progress()
{
}
function onPhase2Complete()
{
}
function onMissionDownloadPhase3()
{
}
function onPhase3Progress()
{
}
function onPhase3Complete()
{
}
function onMissionDownloadComplete()
{
}
//-------------------------------------------------------
//This function actuall needs to return a useful value
//  to be used by the editor tools
//-------------------------------------------------------
function getMouseAdjustAmount(%val)
{
   // based on a default camera FOV of 90'
   return(%val * 0.01);
}

function clientCmdUpdateScore(%value)
{
	 StopMusic();
     Scorebox.SetValue(%value);
}
