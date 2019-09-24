function addTree(%id, %location){

%tree = new TSStatic(%id) {
   shapeName = "assets/models/trees/tree2.dts";
   playAmbient = "1";
   receiveSunLight = "1";
   receiveLMLighting = "1";
   useCustomAmbientLighting = "0";
   customAmbientLighting = "0 0 0 1";
   collisionType = "Visible Mesh";
   decalType = "Collision Mesh";
   allowPlayerStep = "1";
   renderNormals = "0";
   forceDetail = "-1";
   position = "315.18 -180.418 244.313";
   rotation = "0 0 1 195.952";
   scale = "1 1 1";
   isRenderEnabled = "true";
   canSaveDynamicFields = "1";
};

missionCleanup.add(%tree);
%x = getWord(%location, 0);
%y = getWord(%location, 1);
%z = getWord(%x SPC %y );

%location = %x SPC %y SPC %z;
%tree.setTransform(%location);
}