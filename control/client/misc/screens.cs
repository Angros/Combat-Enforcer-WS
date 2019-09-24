//============================================================================
// control/client/misc/screens.cs
//
// Copyright (c) 2003, 2006 Kenneth C. Finney
//============================================================================

//-----------------------------------------------------------------------------
// PlayerInterface is the main TSControl through which the game is viewed.
// The PlayerInterface also contains the hud controls.
//-----------------------------------------------------------------------------



function PlayerInterface::onWake(%this)
{
   $enableDirectInput = "1";
   activateDirectInput();
   Canvas.pushDialog( MainChatBox );
   chatBox.attach(ChatBoxMessageVector);
   
   // just update the action map here
   playerKeymap.push();

   // hack city - these controls are floating around and need to be clamped
   schedule(0, 0, "refreshCenterTextCtrl");
   schedule(0, 0, "refreshBottomTextCtrl");
}

function PlayerInterface::onSleep(%this)
{
   if ( isObject( MainChatBox ) )
      Canvas.popDialog( MainChatBox );
   // pop the keymaps
   playerKeymap.pop();
}

//-----------------------------------------------------------------------------

function refreshBottomTextCtrl()
{
   BottomPrintText.position = "0 0";
}

function refreshCenterTextCtrl()
{
   CenterPrintText.position = "0 0";
}





//------------------------------------------------------------------------------
function LoadScreen::onAdd(%this)
{
   %this.qLineCount = 0;
}

//------------------------------------------------------------------------------
function LoadScreen::onWake(%this)
{
   // Play sound...
   CloseMessagePopup();
}

//------------------------------------------------------------------------------
function LoadScreen::onSleep(%this)
{
   // Clear the load info:
   if ( %this.qLineCount !$= "" )
   {
      for ( %line = 0; %line < %this.qLineCount; %line++ )
         %this.qLine[%line] = "";
   }
   %this.qLineCount = 0;

   LOAD_MapName.setText( "" );
   LOAD_MapDescription.setText( "" );
   LoadingProgress.setValue( 0 );
   LoadingProgressTxt.setValue( "WAITING FOR SERVER" );

   // Stop sound...
}
