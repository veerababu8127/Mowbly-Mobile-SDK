var journal;

$m.onReady(function(){
	journal = document.getElementById('journal');
	journal.addPhoto = function(photoPath){this.innerHTML = '<div class="entry"><img src="' + photoPath + '"/></div>' + this.innerHTML;};
});

function onAddPhotoButtonTouch(){
	$m.getPic(function(response){
		if(response.code == 1){
			// Success
			journal.addPhoto(response.result.path);
		} else{
			// Error
			$m.logError("Error adding photo - " + response.error.message);
		}
	});	
}
