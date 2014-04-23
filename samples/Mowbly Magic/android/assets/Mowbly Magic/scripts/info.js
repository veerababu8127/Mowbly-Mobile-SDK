var bgImgs = [];

$m.onReady(function(){
	if($m.networkConnected()){
		loadBgImages();
		fetchMagic();
	}else
		$m.alert("Unable to show some cool network features of Mowbly!", "Network Unavailable");
});

function loadBgImages(){
	var data = {
		"format":"json",
		"action":"query",
		"generator":"search",
		"gsrnamespace":"6",
		"gsrsearch":"nebula",
		"gsrlimit":"20",
		"gsroffset":"20",
		"prop":"imageinfo",
		"iiprop":"url|size|mime",
		"iiurlwidth":"1024"
	};
	
	
	$m.post("https://commons.wikimedia.org/w/api.php", data, function(response){
		if(response.code == 200){
			// Success
			var result = JSON.parse(response.result.data);
			if(result.query){
				for(var pageId in result.query.pages){
					var page = result.query.pages[pageId];
					if(page.imageinfo[0].width > 1024 && page.imageinfo[0].width < 2048)
						bgImgs.push(page.imageinfo[0].thumburl);
				}
				changeBgImg();
			}else{
				// Error
				$m.logError("Error calling Wikimedia POST API - unexpected response");
			}
		} else{
			// Error
			var errMsg = response.error.message;
			$m.logError("Error calling Wikimedia POST API - " + errMsg);
		}
	});
}

var bgCnt = 0;
function changeBgImg(){
	document.body.style.backgroundImage = "url('" + bgImgs[bgCnt] + "')";
	document.body.style.backgroundSize = "cover";
	bgCnt++;
	if(bgCnt >= bgImgs.length)
		bgCnt = 0;
	setTimeout(changeBgImg, 12000);
}

function fetchMagic(){
	$m.get("http://glosbe.com/gapi/translate?from=eng&dest=eng&format=json&phrase=magic", function(response){
		if(response.code === 200){
			// Success
			if(response.result && response.result.data){
				var result = JSON.parse(response.result.data);
				var meanings = result.tuc[0].meanings;
				addMeanings(meanings);
			}else{
				// Error
				$m.logError("Error calling Dicitionary GET API - unexpected response");
			}
		} else{
			// Error
			var errMsg = response.error.message;
			$m.logError("Error calling Dictionary GET API - " + errMsg);
		}
	});
}

function addMeanings(meanings){
	var infoPnl = document.getElementById('infoPanel');
	var templ = '<div class="entry"><span class="text">[MEANING]</span></div>';
	var htmlStr = '<div class="title">What is magic?</div>'; 
	for(var m=0;m<meanings.length;m++){
		htmlStr += templ.replace('[MEANING]', ((m+1) + '. ' + meanings[m].text));
	}
	infoPnl.innerHTML = htmlStr;
}

function closePage(){
	$m.close();
}