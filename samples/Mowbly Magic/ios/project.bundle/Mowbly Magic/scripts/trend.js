var trendTempl = '<div class="entry [E_BG]"><span class="icon [E_ICON]"></span><span class="datetime">[E_DATETIME]</span><div class="text">[E_TEXT]</div></div>';

$m.onReady(function(){

});

$m.onData(function(obj){
	var predictions = obj.data;
	var trendList = document.getElementById('trendList');
	trendList.innerHTML = "";
	var results = predictions.results;
	for(var i=results.length-1;i>=0;i--){
		var entry = results[i];
		var trendEntry = trendTempl;
		var moodObj = getMood(entry.prediction.mood);
		trendEntry = trendEntry.replace("[E_DATETIME]", entry.datetime);
		trendEntry = trendEntry.replace("[E_TEXT]", entry.prediction.text.replace(/<br>/g,''));
		trendEntry = trendEntry.replace("[E_ICON]", moodObj.icon);
		trendEntry = trendEntry.replace("[E_BG]", moodObj.background);
		trendList.innerHTML += trendEntry;
	}
});

function getMood(mood){
	if(mood === 'positive'){
		return {'icon':'happy','background':'positive'};
	}else if(mood === 'neutral'){
		return {'icon':'fine','background':'neutral'};
	}else{
		return {'icon':'sad','background':'negative'};
	}
}

function clearTrend(){
	$m.confirm({"title":"Confirm",
				"message": "Are you sure you want to clear trends?", 
				"buttons": [{"label": "Yes"},
							{"label": "No"}]
				}, function(index){
		if(index === 0) {
			// Yes
			$m.setResult({"clearTrend":true});
			document.getElementById('trendList').innerHTML = "";
			closePage();
		}
	});
	
}

function closePage(){
	$m.close();
}
