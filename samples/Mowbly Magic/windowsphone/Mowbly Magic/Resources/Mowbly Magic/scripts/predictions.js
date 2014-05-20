var GREEN_BALL = "images/ball-green.png";
var ORANGE_BALL = "images/ball-orange.png";
var RED_BALL = "images/ball-red.png";

var predictions = {"latest":null, "positive":0, "neutral":0, "negative":0, results:[], "satisfaction":null};

var answers = [{"mood":"positive","img":GREEN_BALL,"text":"It is certain"},
{"mood":"positive","img":GREEN_BALL,"text":"It is <br>decidedly so"},
{"mood":"positive","img":GREEN_BALL,"text":"Without a doubt"},
{"mood":"positive","img":GREEN_BALL,"text":"Yes definitely"},
{"mood":"positive","img":GREEN_BALL,"text":"You may <br>rely on it"},
{"mood":"positive","img":GREEN_BALL,"text":"As I see it, <br>yes"},
{"mood":"positive","img":GREEN_BALL,"text":"Most likely"},
{"mood":"positive","img":GREEN_BALL,"text":"Outlook good"},
{"mood":"positive","img":GREEN_BALL,"text":"Yes"},
{"mood":"positive","img":GREEN_BALL,"text":"Signs point to <br>yes"},
{"mood":"neutral","img":ORANGE_BALL,"text":"Reply hazy <br>try again"},
{"mood":"neutral","img":ORANGE_BALL,"text":"Ask again later"},
{"mood":"neutral","img":ORANGE_BALL,"text":"Better not <br>tell you now"},
{"mood":"neutral","img":ORANGE_BALL,"text":"Cannot predict now"},
{"mood":"neutral","img":ORANGE_BALL,"text":"Concentrate <br>and ask again"},
{"mood":"negative","img":RED_BALL,"text":"Don't count on it"},
{"mood":"negative","img":RED_BALL,"text":"My reply is no"},
{"mood":"negative","img":RED_BALL,"text":"My sources <br>say no"},
{"mood":"negative","img":RED_BALL,"text":"Outlook <br>not so good"},
{"mood":"negative","img":RED_BALL,"text":"Very doubtful"}];

function loadPredictionsTrend(){
	$m.readFile("predictions.json", function(response){
		if(response.code == -1) {
			$m.logError("Could not read predictions trend");
			return;
		}
		if(response.code){
			// Success
			predictions = response.result;
		} else{
			// Error
			$m.logError("Could not read predictions trend");
		}
	});
}

function track(prediction){
	var dtStr = format(new Date());
	predictions.results.push({"prediction":prediction, "datetime":dtStr});
	predictions[prediction.mood]++;
	predictions.latest = prediction;
	
	writePredictionsTrend();
}

function writePredictionsTrend(){
	$m.writeFile("predictions.json", JSON.stringify(predictions), function(response){
		if(response.code == -1) {
			$m.logError("Could not write predictions trend");
			return;
		}
		if(response.code){
			$m.logInfo("Predictions trend written to file");
		} else{
			$m.logError("Could not write predictions trend");
			return;
		}
	});
}

function deletePredictionsTrend(){
	$m.deleteFile("predictions.json", function(response){
		if(response.code == -1) {
			$m.logError("Could not write predictions trend");
			return;
		}
		if(response.code) {
			//Success
			predictions = {"latest":null, "positive":0, "neutral":0, "negative":0, results:[], "satisfaction":null};
			$m.logInfo("Predictions trend deleted successfully");
		} else {
			// Error
			$m.logError("Could not write predictions trend");
		}	
	});
}

function format(dt){
	return [[AddZero(dt.getDate()), AddZero(dt.getMonth() + 1), dt.getFullYear()].join("/"), [AddZero(dt.getHours()), AddZero(dt.getMinutes())].join(":"), dt.getHours() >= 12 ? "PM" : "AM"].join(" ");
}

//Pad given value to the left with "0"
function AddZero(num) {
    return (num >= 0 && num < 10) ? "0" + num : num + "";
}
