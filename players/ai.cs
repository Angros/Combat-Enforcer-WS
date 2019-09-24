
//-----------------------------------------------------------------------------
// Callback Handlers
//-----------------------------------------------------------------------------

//$MINIMUM_SCAN_INTERVAL = 2000;
//$MAXIMUM_SCAN_INTERVAL = 10000;

$MIN_SCAN_GAP = 1000;
$MAX_SCAN_GAP = 20000;
$MIN_TRIGGER_HOLD = 100;
$MAX_TRIGGER_HOLD = 200;
$MIN_ITCHY_FINGER = 2000;
$MAX_ITCHY_FINGER = 10000;
$MAX_THREAT_ENGAGE_RANGE = 100;
$MAX_AGGRESSIVENESS = 100;
$MAX_ATTENTION_LEVEL = 100;
$MAX_ALERTNESS = 100;
$MAX_ROVER_SPEED = 0.2;
$MIN_ROVER_SPEED = 0.1;
$MAX_PATROL_SPEED = 0.2;
$MIN_PATROL_SPEED = 0.1;
$STATIONARY = 0.0;

$cardinalDirection[0] = "0 10000 0";
$cardinalDirection[1] = "10000 0 0";
$cardinalDirection[2] = "0 -10000 0";
$cardinalDirection[3] = "-10000 0 0";

function AIBeast::onStuck(%this,%obj)
{
   // Invoked if the player is stuck while moving
   // This method is currently not being invoked correctly.
   if(!isObject(%obj))
      return;
   echo( "I'm stuck !! (from:"@%obj@")");
}

function AIBeast::unBlock(%this,%obj)
{
   //echo( "unBlock... (from:"@%obj@")");
   if(!isObject(%obj))
      return;
   cancel(%obj.nextBlockCheck);
   %this.onReachDestination(%obj);

}

function AIBeast::onReachDestination(%this,%obj)
{
   // Invoked when the player arrives at his destination point.
//   echo( "onReachDestination" );
   if(!isObject(%obj))
      return;
    cancel(%obj.nextBlockCheck);
   %theRole = %obj.role;

  // echo( "finding new dest !!! (from:"@%obj@")");
    %obj.setMoveSpeed($MIN_ROVER_SPEED);
    %victim = %obj.getAimObject();
    %this.setRandomDestination(%obj);
    %obj.nextBlockCheck = %this.schedule($MAX_SCAN_GAP*2, "unBlock", %obj);
//     echo( "----%this.scheduledCheck" @ %this.scheduledCheck);

}

function AIBeast::checkForThreat(%this,%obj)
{
//   echo( "checkForThreat...");
   if(!isObject(%obj))
      return;

   if (isObject( %obj) )
   {
      %idx = %obj.getClosestHuman();
      if (%obj.attentionLevel>0)  // if attentionLevel is non-zero, keep looking at max range
      {
         %testRange = %obj.range * (%obj.aggression*2);
      }
      else
      {
         %testRange = %obj.range;
      }
   }
   else
   {
      return 0;
   }
   if (%idx < 0)
      return 0;
   %target = ClientGroup.getObject( %idx );

   if (%target.player == %obj.currentTarget)
   {
      if (isObject( %obj) )
      {
         if ( %obj.GetTargetRange(%target.player) <  %testRange)
         {
            return %target.player;
         }
      }
      else
         return 0;
   }
   else     // new threat
   {
      if (isObject( %obj) )
      {
         if ( %obj.GetTargetRange(%target.player) <  %testRange)
         {
            return %target.player;
         }
      }
      else
         return 0;
   }
 //  echo( "...no threat");
   return 0;
}

function AIBeast::DoScan(%this,%obj)
{
   //echo( "doScan... (from:"@%obj@")");
   if(!isObject(%obj))
      return;
   cancel(%this.scheduledCheck);

   %theRole = %obj.role;

   if (%obj.attentionLevel<=0)  // if attentionLevel is non-zero, keep looking in same direction
      %obj.attentionLevel=0;
   else
      %obj.attentionLevel--;

  if (%obj.attentionLevel==0)  // if attentionLevel is non-zero, keep looking in same direction
  {
     %look = getRandom(3);
     if (%this.look != %look)
        %this.look = %look;
     else
     {
        %this.look = %look + 1;
        if (%this.look < 3)
           %this.look++;
        else
           %this.look = 0;
     }
  }

  %t = $cardinalDirection[%this.look];
  %obj.setAimLocation( %t);

   if ( (%tgtPlayer = %this.checkForThreat(%obj)) != 0)
   {
      if (%obj.currentTarget)
      {
         if (%obj.currentTarget==%tgtPlayer)
         {
           // echo( "STILL A THREAT (from:"@%obj@")");
            %obj.setAimObject( %tgtPlayer );
            %obj.attentionLevel = %obj.attention;
            if (%theRole !$= "Guard")
            {
               %obj.setMoveSpeed($MAX_ROVER_SPEED);
               %obj.setMoveDestination(%tgtPlayer.getPosition());
               %obj.nextBlockCheck = %this.schedule($MAX_SCAN_GAP*2, "unBlock", %obj);
            }
         }
         else
         {
           // echo( "CHANGED THREAT (from:"@%obj@")");
            %obj.currentTarget =  %tgtPlayer;
            %obj.setAimObject( %obj.currentTarget );
            if (%theRole !$= "Guard")
            {
               %obj.setMoveSpeed($MAX_ROVER_SPEED);
               %obj.setMoveDestination(%tgtPlayer.getPosition());
               %obj.nextBlockCheck = %this.schedule($MAX_SCAN_GAP*2, "unBlock", %obj);
            }
         }
       }
       else
       {
         //echo( "NEW THREAT!! (from:"@%obj@")");
         %obj.setAimObject( %tgtPlayer );
         %obj.currentTarget =  %tgtPlayer;
         %obj.attentionLevel = %obj.attention;
         if (%theRole !$= "Guard")
         {
            %obj.setMoveSpeed($MAX_ROVER_SPEED);
            %obj.setMoveDestination(%tgtPlayer.getPosition());
            %obj.nextBlockCheck = %this.schedule($MAX_SCAN_GAP*2, "unBlock", %obj);
         }
       }
   }
   else
   {
      if (%obj.getAimObject)
      {
         %obj.clearAim();
        // echo( "doScan >>> %obj.clearAim (from:"@%obj@")");
         %obj.currentTarget = 0; // forget this target
      }
      %this.nextScan = %this.schedule($MIN_SCAN_GAP+getRandom($MAX_SCAN_GAP/%this.alertness), "doScan", %obj);
   }
}

function AIBeast::openFire(%this,%obj)
{
   if(!isObject(%obj))
      return;
   %obj.setImageTrigger(0,false);
   %this.schedule($MIN_TRIGGER_HOLD+getRandom($MAX_TRIGGER_HOLD), "ceaseFire", %obj);
}

function AIBeast::ceaseFire(%this,%obj)
{
   //echo( "--------ceaseFire... (from:"@%obj@")");
   if(!isObject(%obj))
      return;
   %obj.setImageTrigger(0,false);
   %obj.clearAim();
   %obj.currentTarget = 0; // forget this target
}

function AIBeast::onTargetEnterLOS(%this,%obj)
{
   // If an aim target object is set, this method is invoked when
   // that object becomes visible.
  //  echo( "LOS TARGET ! (from:"@%obj@")");
   if(!isObject(%obj))
      return;
   %theRole = %obj.role;

   %obj.attentionLevel = %this.attention;
   %this.schedule($MIN_ITCHY_FINGER+getRandom($MAX_ITCHY_FINGER), "openFire", %obj);
   %this.schedule($MIN_SCAN_GAP+getRandom($MAX_SCAN_GAP/%this.alertness), "doScan", %obj);
   %obj.setImageTrigger(0,true);
}

function AIBeast::onTargetExitLOS(%this,%obj)
{
   // If an aim target object is set, this method is invoked when
   // the object is not longer visible.
  // echo( ".......Fuhgetaboutit (from:"@%obj@")");
   if(!isObject(%obj))
      return;
   %obj.setImageTrigger(0,false);
   %obj.clearAim();
  // echo( "onTargetExitLOS >>> %obj.clearAim (from:"@%obj@")");
   %obj.currentTarget = 0;  // forget this target
   %this.schedule($MIN_SCAN_GAP, "doScan", %obj);
}

function AIBeast::setRandomDestination(%this,%obj)
{
   if(!isObject(%obj))
      return;
   %pos = %obj.getTransform();
   %x = getWord(%pos, 0); %y = getWord(%pos, 1); %z = getWord(%pos, 2);
   %rnd1 = getrandom(20); %rnd2 = getrandom(20); %rnd3 = getrandom(20);

   if( %rnd1 < 10 )
      %rnd01 = %x + getrandom(30);
   else
      %rnd01 = %x - getrandom(30);

   if( %rnd2 < 10 )
      %rnd02 = %y + getrandom(30);
   else
      %rnd02 = %y - getrandom(30);

   if( %rnd3 < 10 )
      %rnd03 = %z + getrandom(30);
   else
      %rnd03 = %z - getrandom(30);

   %obj.setMoveDestination(%rnd01 SPC %rnd02 SPC %rnd03);
}

//
//-----------------------------------------------------------------------------

function AIPlayer::spawnBot(%index,%location)
{
   // An example function which creates a new AIPlayer object
   // using the the example player datablock.
   %me = new AIPlayer() {
      dataBlock = BeastAvatar;
      aiPlayer = true;
   };
   MissionCleanup.add(%me);
   AIGroup.add(%me);
   // Player setup
   // defaults

   $mebot = %me;
   %me.look = 0;
   %me.range = 100;
   %spawn=%location;

    %me.alertness = 75;
    %me.aggression = 75;
    %me.attention = 75;
    %me.range = 50;

   %me.index = %index;
   %me.setTransform(%spawn);
   %me.setEnergyLevel(60);
   %me.role = %role;
   %me.setShapeName("Beast");
   %me.attentionLevel = 0;
   %me.nextBlockCheck = 0;
   %weapon = new Item() {
      dataBlock = CrossBow;
   };
   %ammo = new Item() {
      dataBlock = CrossBowAmmo;
   };

   MissionCleanup.add(%weapon);
   MissionCleanup.add(%ammo);
   %me.pickup(%weapon, 1);
   %me.pickup(%ammo, 2);
   %me.look = 0;
   %me.setMoveSpeed(0);

   echo("Added Beast [" SPC %me SPC "] :" SPC %me.task SPC ":" SPC %me.role SPC "#" SPC  %me.index );

  %me.setAimLocation( $cardinalDirection[%me.look]);
  %me.getDataBlock().schedule(5000, "DoScan", %me);

   return %me;
}

function AIPlayer::GetTargetRange(%this, %target)
{
   $tgt = %target;
   %tgtPos = %target.getPosition();
   %eyePoint = %this.getWorldBoxCenter();
   %distance = VectorDist(%tgtPos, %eyePoint);
///   echo("Actual range to target: " @ %distance );
   return %distance;
}

function AIPlayer::getClosestHuman(%this) {
///      echo( "getClosestHuman...");

   %index = -1;
   %botPos = %this.getPosition();
   %count = ClientGroup.getCount();
   for(%i = 0; %i < %count; %i++)
   {
      %client = ClientGroup.getObject(%i);
      if (%client.player $= "" || %client.player == 0 )
         return -1;
      %playPos = %client.player.getPosition();

      %tempDist = VectorDist(%playPos, %botPos);
      if(%i == 0) {
         %dist = %tempDist;
         %index = %i;
      }
      else {
         if(%dist > %tempDist) {
            %dist = %tempDist;
            %index = %i;
         }
      }
   }
   return %index;
}

function CreateBots()
{
  new SimSet(AIGroup);
  AIPlayer::spawnBot(0,"382 -405 367 1 0 0 0");
  AIPlayer::spawnBot(1,"282 -356 349 1 0 0 0");
  AIPlayer::spawnBot(2,"351 -357 302 1 0 0 0");
  AIPlayer::spawnBot(3,"46 -400 301 1 0 0 0");
  AIPlayer::spawnBot(4,"428 -446 301 1 0 0 0");
  AIPlayer::spawnBot(5,"379 -314 396 1 0 0 0");
  AIPlayer::spawnBot(6,"207 -166 361 1 0 0 0");
  AIPlayer::spawnBot(7,"297 -413 375 1 0 0 0");
  AIPlayer::spawnBot(8,"344 -443 301 1 0 0 0");
  AIPlayer::spawnBot(9,"65 -442 365 1 0 0 0");
  AIPlayer::spawnBot(10,"-309 -238 346 1 0 0 0");
  AIPlayer::spawnBot(11,"-345 101 373 1 0 0 0");
  AIPlayer::spawnBot(12,"306 267 357 1 0 0 0");
  AIPlayer::spawnBot(13,"343 -232 340 1 0 0 0");
  AIPlayer::spawnBot(14,"460 -127 301 1 0 0 0");
}
