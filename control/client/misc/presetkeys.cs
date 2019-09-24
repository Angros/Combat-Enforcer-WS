//============================================================================
// control/client/misc/presetkeys.cs
//
// Copyright (c) 2003, 2006 Kenneth C. Finney
//============================================================================

if ( IsObject(PlayerKeymap) )  // If we already have a player key map,
   PlayerKeymap.delete();        // delete it so that we can make a new one
$MoveMap = new ActionMap(PlayerKeymap);

$movementSpeed = 1;             // m/s   for use by movement functions




//------------------------------------------------------------------------------
// Non-remapable binds
//------------------------------------------------------------------------------

function DoExitGame()
{
   MessageBoxYesNo( "Quit Mission", "Exit from this Mission?", "quitGame();", "");
}

function quitGame()
{
   ServerConnection.delete();
   showCursor();
   quit();
}

//============================================================================
// Motion Functions
//============================================================================

function GoLeft(%val)
//----------------------------------------------------------------------------
// "strafing"
//----------------------------------------------------------------------------
{
   $mvLeftAction = %val;
}

function GoRight(%val)
//----------------------------------------------------------------------------
// "strafing"
//----------------------------------------------------------------------------
{
   $mvRightAction = %val;
}

function GoAhead(%val)
//----------------------------------------------------------------------------
// running forward
//----------------------------------------------------------------------------
{
   $mvForwardAction = %val;
}

function BackUp(%val)
//----------------------------------------------------------------------------
// running backwards
//----------------------------------------------------------------------------
{
   $mvBackwardAction = %val;
}

function DoYaw(%val)
//----------------------------------------------------------------------------
// looking, spinning or aiming horizontally by mouse or joystick control
//----------------------------------------------------------------------------
{
   $mvYaw += %val * ($cameraFov / 90) * 0.02;
}

function DoPitch(%val)
//----------------------------------------------------------------------------
// looking vertically by mouse or joystick control
//----------------------------------------------------------------------------
{
   $mvPitch += %val * ($cameraFov / 90) * 0.02;
}

function DoJump(%val)
//----------------------------------------------------------------------------
// momentary upward movement, with character animation
//----------------------------------------------------------------------------
{
   $mvTriggerCount2++;
}

//============================================================================
// View Functions
//============================================================================

function Toggle3rdPPOVLook( %val )
//----------------------------------------------------------------------------
// enable the "free look" feature. As long as the mapped key is pressed,
// the player can view his avatar by moving the mouse around.
//----------------------------------------------------------------------------
{
   if ( %val )
      $mvFreeLook = true;
   else
      $mvFreeLook = false;
}

function MouseAction(%val)
{
   $mvTriggerCount0++;
}

$firstPerson = true;
function Toggle1stPPOV(%val)
//----------------------------------------------------------------------------
// switch between 1st and 3rd person point-of-views.
//----------------------------------------------------------------------------
{
   if (%val)
   {
      $firstPerson = !$firstPerson;
      ServerConnection.setFirstPerson($firstPerson);
   }
}








function dropCameraAtPlayer(%val)
{
   if (%val)
      commandToServer('dropCameraAtPlayer');
}

function dropPlayerAtCamera(%val)
{
   if (%val)
      commandToServer('DropPlayerAtCamera');
}
function toggleCamera(%val)
{
   if (%val)
   {
      commandToServer('ToggleCamera');
      }
}

function toggleMap(%val)
{
   if (%val)
   {
      commandToServer('toggleMap');
      }
}

//============================================================================
// keyboard control mappings
//============================================================================
// these ones available when player is in game
PlayerKeymap.Bind( mouse, button0, MouseAction ); // left mouse button
PlayerKeymap.Bind(keyboard, w, GoAhead);
PlayerKeymap.Bind(keyboard, s, BackUp);
PlayerKeymap.Bind(keyboard, a, GoLeft);
PlayerKeymap.Bind(keyboard, d, GoRight);
PlayerKeymap.Bind(keyboard, space, DoJump );
PlayerKeymap.Bind(keyboard, z, Toggle3rdPPOVLook );
PlayerKeymap.Bind(keyboard, tab, Toggle1stPPOV );
PlayerKeymap.Bind(mouse, xaxis, DoYaw );
PlayerKeymap.Bind(mouse, yaxis, DoPitch );

// these ones are always available

GlobalActionMap.Bind(keyboard, escape, DoExitGame);
GlobalActionMap.Bind(keyboard, tilde, ToggleConsole);

PlayerKeymap.bind(keyboard, "F8", dropCameraAtPlayer);
PlayerKeymap.bind(keyboard, "F7", dropPlayerAtCamera); 
PlayerKeymap.bind(keyboard, "alt c", toggleCamera);

PlayerKeymap.bindCmd(keyboard, "n", "toggleNetGraph();", "");
PlayerKeymap.bindCmd(keyboard, "m", "toggleMap();", "");


function SendMacro(%value)
{
 switch$ (%value)
 {
 case 1:
 %msg = "Hello World!";
 case 2:
 %msg = "Hello? Is this thing on?";
 default:
 %msg = "Nevermind!";
 }
 CommandToServer('TellEveryone', %msg);
}
function clientCmdTellMessage(%sender, %msgString)
{
 MessagePopup( "HELLO EVERYBODY", %msgString, 1000);
}
PlayerKeymap.bindCmd(keyboard, "1", "SendMacro(1);", "");
PlayerKeymap.bindCmd(keyboard, "2", "SendMacro(2);", "");
PlayerKeymap.bindCmd(keyboard, "3", "SendMacro(3);", "");

function pageMessageBoxUp( %val )
{
   if ( %val )
      PageUpMessageBox();
}
function pageMessageBoxDown( %val )
{
   if ( %val )
      PageDownMessageBox ();
}
PlayerKeymap.bind(keyboard, "t", ToggleMessageBox );
PlayerKeymap.bind(keyboard, "PageUp", PageMessageBoxUp );
PlayerKeymap.bind(keyboard, "PageDown", PageMessageBoxDown );

function eject()
{
commandToServer('dismountVehicle');
}
PlayerKeymap.bindCmd(keyboard, "ctrl x","eject();","");