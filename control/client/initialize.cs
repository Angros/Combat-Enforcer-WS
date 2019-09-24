//============================================================================
// control/client/initialize.cs
//
//  client control initialization module for 3D3E emaga6 sample game
//
//  Copyright (c) 2003, 2011 by Kenneth C.  Finney.
//============================================================================

$pref::SFX::autoDetect = true;
$pref::SFX::provider = "";
$pref::SFX::device = "OpenAL";
$pref::SFX::useHardware = false;
$pref::SFX::maxSoftwareBuffers = 16;
$pref::SFX::frequency = 44100;
$pref::SFX::bitrate = 32;
$pref::SFX::masterVolume = 0.8;
$pref::SFX::channelVolume1 = 1;
$pref::SFX::channelVolume2 = 1;
$pref::SFX::channelVolume3 = 1;
$pref::SFX::channelVolume4 = 1;
$pref::SFX::channelVolume5 = 1;
$pref::SFX::channelVolume6 = 1;
$pref::SFX::channelVolume7 = 1;
$pref::SFX::channelVolume8 = 1;

$GuiAudioType        = 1;  // Interface.
$SimAudioType        = 2;  // Game.
$MessageAudioType    = 3;  // Notifications.
$MusicAudioType      = 4;  // Music.

$AudioChannels[ 0 ] = AudioChannelDefault;
$AudioChannels[ $GuiAudioType ] = AudioChannelGui;
$AudioChannels[ $SimAudioType ] = AudioChannelEffects;
$AudioChannels[ $MessageAudioType ] = AudioChannelMessages;
$AudioChannels[ $MusicAudioType ] = AudioChannelMusic;

new SFXDescription(AudioGui)
{
   volume   = 1.0;
   isLooping= false;
   is3D     = false;
   type     = $GuiAudioType;
};

new SFXDescription(AudioMessage)
{
   volume   = 1.0;
   isLooping= false;
   is3D     = false;
   type     = $MessageAudioType;
};

new SFXProfile(AudioButtonOver)
{
   filename = "assets/sound/buttonOver.ogg";
   description = "AudioGui";
       preload = true;
};




function InitializeClient()
//----------------------------------------------------------------------------
// Prepare some global client information, fire up the graphics engine,
// and then connect to the server code that is already running in another
// thread.
//----------------------------------------------------------------------------
{
  $Client::GameTypeQuery = "3D3E";
  $Client::MissionTypeQuery = "Any";

   Echo("\n++++++++++++ Initializing module: emaga6 client ++++++++++++");


   Echo("\n--------- Initializing module: emaga client ---------");

   exec( "core/scripts/client/defaults.cs" );

  // these are necessary graphics settings
  $pref::Video::allowOpenGL   = true;
  $pref::Video::displayDevice = "D3D9";
  $pref::Video::mode = "1024 768 false 32 60 0";
  $pref::Video::windowedRes = "1024 768";
  $pref::Video::Resolution = "1024 768";
  $pref::Video::fullScreen = 0;
  $pref::Video::BitsPerPixel = "32";
  $pref::Video::RefreshRate = "60";
  
   $pref::lightManager = "Basic Lighting";
   InitBaseClient(); // basic client features defined in the core modules

   Exec("./misc/sndprofiles.cs");
  
   // Use our prefs to configure our Canvas/Window
   // Make sure a canvas has been built before any gui scripts are
   // executed because many of the controls depend on the canvas to
   // already exist when they are loaded.
   // Actual Canvas creation is triggered by a callback in the core
   // code, which calls CreateCanvas, located in our root main.cs module
   configureCanvas();
 
   PlayMusic(AudioIntroMusicProfile);

   loadMaterials();

   Exec("./client.cs");

   // Interface definitions
   Exec("./profiles.cs");
   Exec("./default_profile.cs");
   Exec("./interfaces/splashscreen.gui");
   Exec("./interfaces/MenuScreen.gui");
   Exec("./interfaces/loadscreen.gui");
   Exec("./interfaces/playerinterface.gui");
   Exec("./interfaces/serverscreen.gui");
   Exec("./interfaces/myGui.gui");
   Exec("./interfaces/map.gui");
   Exec("./interfaces/helpDlg.gui");

   Exec("./interfaces/hostscreen.gui");
   Exec("./interfaces/soloscreen.gui");
   Exec("./interfaces/chatbox.gui");
   Exec("./interfaces/messagebox.gui");

   // Interface scripts
   Exec("./misc/Screens.cs");
   Exec("./misc/serverscreen.cs");
   Exec("./misc/myGui.cs");
   Exec("./misc/map.cs");
   
   Exec("./misc/hostscreen.cs");
   Exec("./misc/soloscreen.cs");
   Exec("./misc/chatbox.cs");
   Exec("./misc/messagebox.cs");
   
   Exec("./misc/presetkeys.cs");
   Exec("./client.cs");

   // these modules rely on things defined in the core code
   // that are activated in the InitBaseClient() function above
   // so must be located after it has been called
   Exec("./misc/transfer.cs");
   Exec("./misc/connection.cs");

   SetNetPort(0);
   
   if ($JoinGameAddress !$= "") 
      connect($JoinGameAddress, "", $Pref::Player::Name); 
   else
      Canvas.setContent(SplashScreen);
}
