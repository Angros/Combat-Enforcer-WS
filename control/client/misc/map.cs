function map::onWake() {
	// set the position of Gui text ctrl
	playerText.setPosition(0,0);
	
	
	}

function toggleMap()
{

    if(!Canvas.isMember(map))
    {
        Canvas.add(map);
    }
    else
      Canvas.remove(map);
}