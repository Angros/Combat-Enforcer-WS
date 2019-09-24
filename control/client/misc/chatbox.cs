
new MessageVector(ChatBoxMessageVector);
$LastframeTarget = 0;

if ($pref::ChatBoxLength $= "")
   $pref::ChatBoxLength = 1;
if ($pref::frameMessageLogSize $= "")
   $pref::frameMessageLogSize = 40;
$outerChatLenY[1] = 4;
$outerChatLenY[2] = 9;
$outerChatLenY[3] = 13;
function onChatMessage(%message, %voice, %pitch)
{
   if (GetWordCount(%message)) {
      ChatBox.AddLine(%message);
   }
}
function onServerMessage(%message)
{
   if (GetWordCount(%message)) {
      ChatBox.AddLine(%message);
   }
}

function MainChatBox::onWake( %this )
{
   // set the chat hud to the users pref
   %this.setChatBoxLength( $Pref::ChatBoxLength );
}


//------------------------------------------------------------------------------

function MainChatBox::setChatBoxLength( %this, %length )
{
   %textHeight = ChatBox.Profile.fontSize + ChatBox.lineSpacing;
   if (%textHeight <= 0)
      %textHeight = 12;
   %lengthInPixels = $outerChatLenY[%length] * %textHeight;
   %chatMargin = getWord(OuterChatFrame.extent, 1) - getWord(ChatScrollFrame.Extent, 1)
                  + 2 * ChatScrollFrame.profile.borderThickness;
   OuterChatFrame.setExtent(firstWord(OuterChatFrame.extent), %lengthInPixels + %chatMargin);
   ChatScrollFrame.scrollToBottom();
   ChatPageDown.setVisible(false);
}


//------------------------------------------------------------------------------

function MainChatBox::nextChatBoxLen( %this )
{
   %len = $pref::ChatBoxLength++;
   if ($pref::ChatBoxLength == 4)
      $pref::ChatBoxLength = 1;
   %this.setChatBoxLength($pref::ChatBoxLength);
}

function ChatBox::addLine(%this,%text)
{
   %textHeight = %this.profile.fontSize + %this.lineSpacing;
   if (%textHeight <= 0)
      %textHeight = 12;
   %chatScrollHeight = getWord(%this.getGroup().extent, 1);
   %chatPosition = getWord(%this.extent, 1) - %chatScrollHeight +
        getWord(%this.position, 1);
   %linesToScroll = mFloor((%chatPosition / %textHeight) + 0.5);
   if (%linesToScroll > 0)
      %origPosition = %this.position;
   while( !chatPageDown.isVisible() && ChatBoxMessageVector.getNumLines() &&
         (ChatBoxMessageVector.getNumLines() >= $pref::frameMessageLogSize))
   {
      %tag = ChatBoxMessageVector.getLineTag(0);
      if(%tag != 0)
         %tag.delete();
      ChatBoxMessageVector.popFrontLine();
   }
   ChatBoxMessageVector.pushBackLine(%text, $LastframeTarget);
   $LastframeTarget = 0;
   if (%linesToScroll > 0)
   {
      chatPageDown.setVisible(true);
      %this.position = %origPosition;
   }
   else
      chatPageDown.setVisible(false);
}