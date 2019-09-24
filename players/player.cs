//============================================================================
// control/players/player.cs
//
//  player definition module for 3D3E emaga5 sample game
//
//  Copyright (c) 2003,2006, 2012  by Kenneth C.  Finney.
//============================================================================

datablock PlayerData(MaleAvatarDB)
{
   className = MyHeroClass;
   shapeFile = "assets/models/avatars/hero/myhero.dts";
   scale = "1.0 1.0 1.0";
   emap = true;
   renderFirstPerson = false;
   cameraMaxDist = 3;
   mass = 100;
   density = 10;
   drag = 0.1;
   maxdrag = 0.5;
   maxDamage = 100;
   maxEnergy =  100;
   maxForwardSpeed = 15;
   maxBackwardSpeed = 10;
   maxSideSpeed = 12;
   minJumpSpeed = 20;
   maxJumpSpeed = 30;
   runForce = 50 * 90;
   jumpForce = 10 * 90;
   runSurfaceAngle  = 70;
   jumpSurfaceAngle = 80;
   maxStepHeight = 1.5;  //half meter

   runEnergyDrain = 0.05;
   minRunEnergy = 1;
   jumpEnergyDrain = 20;
   minJumpEnergy = 20;
   recoverDelay = 30;
   recoverRunForceScale = 1.2;
   minImpactSpeed = 15;
   speedDamageScale = 3.0;
   repairRate = 0.1;

   maxInv[Copper] = 9999;
   maxInv[Silver] = 99;
   maxInv[Gold] = 9;

   maxInv[Crossbow] = 1;
   maxInv[CrossbowAmmo] = 20;
};


//============================================================================
// Avatar Datablock methods
//============================================================================

function MaleAvatarClass::onAdd(%this,%obj)
//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
{
   %obj.mountVehicle = false;

   // Default dynamic Avatar stats
   %obj.setRechargeRate(0.01);
   %obj.setRepairRate(%this.repairRate);
}

function MaleAvatarClass::onRemove(%this, %obj)
//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
{
   %client = %obj.client;
   if (%client.player == %obj)
   {
      %client.player = 0;
   }
}


function MaleAvatarClass::onCollision(%this,%obj,%col,%vec,%speed)
//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
{
  %obj_state = %obj.getState();
  %col_className = %col.getClassName();
  %col_dblock_className = %col.getDataBlock().className;
  %colName = %col.getDataBlock().getName();

  if ( %obj_state $= "Dead")
    return;

  if (%col_className $= "Item"  ) // Deal with all items
  {
    %obj.pickup(%col);                     // otherwise, pick the item up
  }
}


function MyHeroClass::onCollision(%this,%obj,%col,%vec,%speed)
//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
{
  %obj_state = %obj.getState();
  %col_className = %col.getClassName();
  %obj_dblock_className = %obj.getDataBlock().className;

  if ( %obj_state $= "Dead")
    return;

  if (%col_className $= "Item"  ) // Deal with all items
  {
    %obj.pickup(%col);                     // otherwise, pick the item up
  }
  
  %this = %col.getDataBlock();
   if ( %this.className $= "Car" )
   {
       %node = 0;   // Find next available seat
       %col.mountObject(%obj,%node);
       %obj.mVehicle = %col;
   }
}

function MyHeroClass::OnMount(%this,%obj,%vehicle,%node)
{
   %obj.setTransform("0 0 0 0 0 1 0");
   %obj.setActionThread(%vehicle.getDatablock().mountPose[%node]);
   if (%node == 0)
   {
      %obj.setControlObject(%vehicle);
      %obj.lastWeapon = %obj.getMountedImage($WeaponSlot);
      %obj.unmountImage($WeaponSlot);
   }
}
function MyHeroClass::doDismount(%this, %obj, %forced)
{
   %pos    = getWords(%obj.getTransform(), 0, 2);
   %oldPos = %pos;
   %vec[0] = " 1  1  1";
   %vec[1] = " 1  1  1";
   %vec[2] = " 1  1  -1";
   %vec[3] = " 1  0  0";
   %vec[4] = "-1  0  0";
   %impulseVec  = "0 0 0";
   %vec[0] = MatrixMulVector( %obj.getTransform(), %vec[0]);
   %pos = "0 0 0";
   %numAttempts = 5;
   %success     = -1;
   for (%i = 0; %i < %numAttempts; %i++)
   {
      %pos = VectorAdd(%oldPos, VectorScale(%vec[%i], 3));
      if (%obj.checkDismountPoint(%oldPos, %pos))
      {
         %success = %i;
         %impulseVec = %vec[%i];
         break;
      }
   }
   if (%forced && %success == -1)
      %pos = %oldPos;
   %obj.unmount();
   %obj.setControlObject(%obj);
   %obj.mountVehicle = false;
   %obj.setTransform(%pos);
   %obj.applyImpulse(%pos, VectorScale(%impulseVec, %obj.getDataBlock().mass));
}

function MyHeroClass::onUnmount(%this, %obj, %vehicle, %node)
{
   if (%node == 0)
   {
      %obj.mountImage(%obj.lastWeapon, $WeaponSlot);
      %obj.setControlObject("");
   }
}


//============================================================================
// MaleAvatarDB (ShapeBase) class methods
//============================================================================

function MaleAvatarDB::onImpact(%this,%obj,%collidedObject,%vec,%vecLen)
//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
{
   %obj.damage(0, VectorAdd(%obj.getPosition(),%vec),
      %vecLen * %this.speedDamageScale, "Impact");
}

function MaleAvatarDB::damage(%this, %obj, %sourceObject, %position, %damage, %damageType)
//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
{
   if (%obj.getState() $= "Dead")
      return;
   %obj.applyDamage(%damage);
   %location = "Body";

   // Deal with client callbacks here because we don't have this
   // information in the onDamage or onDisable methods
   %client = %obj.client;
   %sourceClient = %sourceObject ? %sourceObject.client : 0;

   if (%obj.getState() $= "Dead")
   {
      %client.onDeath(%sourceObject, %sourceClient, %damageType, %location);
   }
}

function MaleAvatarDB::onDamage(%this, %obj, %delta)
//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
{
   // This method is invoked by the ShapeBase code whenever the
   // object's damage level changes.
   if (%delta > 0 && %obj.getState() !$= "Dead")
   {
      // Increment the flash based on the amount.
      %flash = %obj.getDamageFlash() + ((%delta / %this.maxDamage) * 2);
      if (%flash > 0.75)
         %flash = 0.75;

      if (%flash > 0.001)
      {
        %obj.setDamageFlash(%flash);
      }
      %obj.setRechargeRate(0.01);
      %obj.setRepairRate(0.01);
   }
}

function MaleAvatarDB::onDisabled(%this,%obj,%state)
//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
{
   // The player object sets the "disabled" state when damage exceeds
   // it's maxDamage value.  This is method is invoked by ShapeBase
   // state mangement code.

   // If we want to deal with the damage information that actually
   // caused this death, then we would have to move this code into
   // the script "damage" method.
   %obj.clearDamageDt();

   %obj.setRechargeRate(0);
   %obj.setRepairRate(0);

   // Release the main weapon trigger
   %obj.setImageTrigger(0,false);

   // Schedule corpse removal.
   %obj.schedule(5000, "startFade", 5000, 0, true);
   %obj.schedule(10000, "delete");
}

function Player::playDeathAnimation(%this,%deathIdx)
{
   //%this.setActionThread("die1");      
   %rand = getRandom(1, 11);
   %this.setActionThread("Death" @ %rand);
}

function Player::playCelAnimation(%this, %anim)
{
   if (%this.getState() !$= "Dead")
      %this.setActionThread("cel"@%anim);
}
