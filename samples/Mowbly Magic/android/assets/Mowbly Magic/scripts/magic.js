var cball, aball, msg;
var isPlaying = false;
var timer = null;

var DEFAULT_OPACITY = 0.1;
var ANIM_INTERVAL = 4200;

$m.onReady(function(){
	cball = document.getElementById('cball');
	aball = document.getElementById('aball');
	aball.style.opacity = 0.8;
	msg = document.getElementById('msg');
	
	loadPredictionsTrend();
});

function showFortune(){
        if(isPlaying)return;
        isPlaying=true;
        var answer = answers[Math.floor(Math.random() * answers.length)];
        track(answer);
        msg.innerHTML = answer.text;
        aball.src = answer.img;
        fade(true);
        setTimeout(function(){fade(false);}, ANIM_INTERVAL*1.2);
}

function fade(io){
    var fader = function(opacity, endOp) {
        return function(){
            opacity += ((endOp == 1)? 50:-50)/ANIM_INTERVAL;
            if((endOp == 1 && opacity >= endOp) || (endOp == DEFAULT_OPACITY && opacity <= DEFAULT_OPACITY)){
                clearInterval(window.timer); opacity = endOp;
                isPlaying = (!io && isPlaying)?false:true;
            }
            cball.style.opacity = opacity;
            cball.style.filter = "alpha(opacity=" + opacity * 100 + ")";
        };
    };
    var opacity = (io)?1:DEFAULT_OPACITY;
    var endOp = (io)?DEFAULT_OPACITY:1;
    window.timer = setInterval(fader(opacity, endOp), 50);
}

function showTrack(){
	if(predictions.results.length > 0)
		$m.open("Track Mood", "/Mowbly Magic/track.html", predictions);
	else
		$m.toast('Try some predictions, touch the crystal ball');
}

function showTrend(){
	if(predictions.results.length > 0)
		$m.open("Prediction Trend", "/Mowbly Magic/trend.html", predictions);
	else
		$m.toast('Try some predictions, touch the crystal ball');
}

function showInfo(){
	$m.open("About Magic", "/Mowbly Magic/info.html");
}

function showShare(){
	$m.open("Share Magic", "/Mowbly Magic/share.html");
}

function showProfile(){
	$m.open("Profile Magic", "/Mowbly Magic/profile.html");
}

$m.onResult(function(eventObject){
	var result = eventObject.result;
	if(result && result.clearTrend)
		deletePredictionsTrend();
});