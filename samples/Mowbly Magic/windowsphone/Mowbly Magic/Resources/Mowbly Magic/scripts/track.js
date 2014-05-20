var moodMarkers = {"positive":["#B8FFA4", "Lucky"],"neutral":["#F6D448", "Safe"],"negative":["#EA252E", "Hard Hit"]};
var pastMood;

$m.onReady(function(){
	pastMood = $m.getPref("magic");
});

$m.onData(function(obj){
	var data = obj.data;
	var marker = document.getElementById('marker');
	var prevMood = document.getElementById('prevMood');

	var mood = moodMarkers[data.latest.mood];
	marker.style.backgroundColor = mood[0];
	marker.innerHTML = mood[1];
	
	var moodStr = "This time you are " + mood[1].toLowerCase() + ".";
	if(pastMood)
		moodStr += " Previously you were " + pastMood.toLowerCase() + " about my prediction.";
	moodStr += " Press a button, to say how you feel now?";
	
	prevMood.innerHTML = moodStr;
});

function closePage(){
	$m.close();
}

function saveMood(mood){
	$m.putPref("magic", mood);
	$m.savePref();
	$m.alert("I am " + mood + " that you are " + mood, "Mood Meter", function(){$m.close();});
}