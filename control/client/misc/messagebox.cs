function MessageBox::Open(%this)                                                                           
{                                                                                                          
   %offset = 6;                                                                                            
   if(%this.isVisible())                                                                                   
      return;                                                                                              
   %windowPos = "8 " @ ( getWord( outerChatFrame.position, 1 ) + getWord( outerChatFrame.extent, 1 ) + 1 );
   %windowExt = getWord( OuterChatFrame.extent, 0 ) @ " " @ getWord( MessageBox_Frame.extent, 1 );         
   %textExtent = getWord(MessageBox_Text.extent, 0);                                                       
   %ctrlExtent = getWord(MessageBox_Frame.extent, 0);                                                      
   Canvas.pushDialog(%this);                                                                               
   MessageBox_Frame.position = %windowPos;                                                                 
   MessageBox_Frame.extent = %windowExt;                                                                   
   MessageBox_Edit.position = setWord(MessageBox_Edit.position, 0, %textExtent + %offset);                 
   MessageBox_Edit.extent = setWord(MessageBox_Edit.extent, 0, %ctrlExtent - %textExtent - (2 * %offset)); 
   %this.setVisible(true);                                                                                 
   MessageBox_Edit.makeFirstResponder(true);                                                               
}                                                                                                          
function MessageBox::Close(%this)                                                                          
{                                                                                                          
   if(!%this.isVisible())                                                                                  
      return;                                                                                              
   Canvas.popDialog(%this);                                                                                
   %this.setVisible(false);                                                                                
   MessageBox_Edit.setValue("");                                                                           
}                                                                                                          
function MessageBox::ToggleState(%this)                                                                    
{                                                                                                          
   if(%this.isVisible())                                                                                   
      %this.close();                                                                                       
   else                                                                                                    
      %this.open();                                                                                        
}                                                                                                          
function MessageBox_Edit::OnEscape(%this)                                                                  
{                                                                                                          
   MessageBox.close();                                                                                     
}                                                                                                          
function MessageBox_Edit::Eval(%this)                                                                      
{                                                                                                          
   %text = trim(%this.getValue());                                                                         
   if(%text !$= "")                                                                                        
      commandToServer('TypedMessage', %text);                                                              
   MessageBox.close();                                                                                     
}                                                                                                          
function ToggleMessageBox(%make)                                                                           
{                                                                                                          
   if(%make)                                                                                               
      MessageBox.toggleState();                                                                            
}                                                                                                          