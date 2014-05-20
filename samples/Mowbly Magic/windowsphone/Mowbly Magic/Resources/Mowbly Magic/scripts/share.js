function sendEmail(){
	$m.email([], "Try some Magic!" , "Hi all, \n\n Mowbly looks cool, check it out. \n\n http://www.mowbly.com \n\n cheers,\n");
}

function sendSMS(){
	$m.sms([], "Try some Magic! Mowbly looks cool, check it out. http://www.mowbly.com");
}

function closePage(){
	$m.close();
}