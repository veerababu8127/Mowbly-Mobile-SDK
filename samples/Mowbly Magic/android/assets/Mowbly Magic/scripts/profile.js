$m.onReady(function(){
});

function addPhoto(){
	$m.confirm({
		"title":"Add Photo",
		"message": "Choose an option below", 
		"buttons": [{"label": "Capture"},
					{"label": "Choose"},
					{"label": "Cancel"}]
	}, 
	callPhotoOption);
}

function callPhotoOption (index){
	// Code to execute when the confirm dialog dismisses
	if(index === 0) {
		// Capture Pic
		capturePhoto();
	} else if(index == 1) {
		// Choose Pic
		choosePhoto();
	}else{
		//do nothing
		return;
	}
}

function capturePhoto(){
	$m.capturePic(setPhoto);
}

function choosePhoto(){
	$m.choosePic(setPhoto);	
}

function setPhoto(response){
	if(response.code){
		// Success
		var imagePath = response.result.path;
		document.getElementById('pic').src = imagePath;
	} else{
		// Error
		var errMsg = response.error.message;
		$m.logError("Error adding photo - " + errMsg);
	}
}

function closePage(){
	$m.close();
}

function addLocation(){
	$m.getLocation(function(response){
		if(response.code){
			// Success
			var lat = response.result.position.coords.latitude;
			var long = response.result.position.coords.longitude;
			if(lat && long && $m.networkConnected())
				document.getElementById('loc').src = getGMap(lat, long);
			else
				$m.logError("Error adding location");
		} else{
			// Error
			var errMsg = response.error.message;
			$m.logError("Error adding location - " + errMsg);
		}
	});
}

function getGMap(lat, long){
	return 'http://maps.googleapis.com/maps/api/staticmap?center=' + lat + ',' + long + '&zoom=11&size=200x200&sensor=false';
}