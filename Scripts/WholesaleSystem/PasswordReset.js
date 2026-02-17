function CancelRequest() {
	window.location.href = `/WholesaleSystem/Login.aspx`;
}

function IdenticalCheck() {
	var identical = document.getElementById("MainContent_Identical").checked;
	var Email = document.getElementById("EmailTextbox");

	if (identical) {
		Email.style.display = 'none';
	} else {
		Email.style.display = "table-row";
	}
}

function SubmitRequest() {
	var identical = document.getElementById("MainContent_Identical").checked;
	var Username = document.getElementById("MainContent_UsernameInput").value;
	var Email = document.getElementById("MainContent_EmailInput").value;

	if (identical) {
		Email = Username
	}

	var dataIn = {
		username: Username,
		email: Email
	}

	$.ajax({
		type: 'POST',
		url: 'PasswordReset.aspx/ResetRequest',
		data: JSON.stringify(dataIn),
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			alert(XMLHttpRequest.responseJSON.Message);
		},
		complete: function (response) {
			var r = response.responseJSON.d;
			if (r.success) {
				var parent = document.getElementById('MainContent_PasswordContent');
				document.getElementById("passwordFieldset").style.display = "none";
				parent.insertAdjacentHTML('beforeend', r.message);
			} else {
				alert(r.message);
			}
		}
	});
}

function SubmitReset() {
	// Get Validation Code from url params
	var urlParams = new URLSearchParams(window.location.search);

	var pass1 = document.getElementById("MainContent_InitialPassword").value;
	var pass2 = document.getElementById("MainContent_ConfirmPassword").value;

	if (pass1 != pass2) {
		alert("Passwords do not match!");
		return false;
	}

	var dataIn = {
		valCode: urlParams.get("ValCode"),
		newPass: pass2
	};

	$.ajax({
		type: 'POST',
		url: 'PasswordReset.aspx/ResetSet',
		data: JSON.stringify(dataIn),
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			alert(XMLHttpRequest.responseJSON.Message);
		},
		complete: function (response) {
			var r = response.responseJSON.d;

			if (r == -1) {
				alert("Password does not meet one or all of the requirements!");
			} else if (r == 0) {
				alert("Something went wrong! Please contact support");
			} else {
				alert("Password change successful! Please attempt to log in.");
				window.location.href = `../WholesaleSystem/Login.aspx`;
			}
		}
	});
}

function OnChangePause(input, func, delay) {
	if (delay == null)
		delay = 500;
	input.onkeyup = function (e) {
		clearTimeout(pause_timeout);
		pause_timeout = setTimeout(function () { func(input.value); }, delay);
	};
}

function RegexCheck(pass) {
	// Adjust color to give feedback to user
	document.getElementById("passNum").style.color = /^([^0-9]*)$/.exec(pass) ? "red" : "green";
	document.getElementById("passLen").style.color = /^(.{0,7})$/.exec(pass) ? "red" : "green";
	document.getElementById("passSpecial").style.color = /^([a-zA-Z0-9]*)$/.exec(pass) ? "red" : "green";
	var passUL = document.getElementById("passUL");

	if (/^([^a-z]*)$/.exec(pass) || /^([^A-Z]*)$/.exec(pass))
		passUL.style.color = "red";
	else
		passUL.style.color = "green";

	var match = document.getElementById("passMatch");
	if (document.getElementById("MainContent_InitialPassword").value != document.getElementById("MainContent_ConfirmPassword").value)
		match.style.color = "red";
	else
		match.style.color = "green";
}

function PasswordVisibility(num) {
	var passBox = null;
	var icon = null;
	if (num == 1) {
		passBox = document.getElementById("MainContent_InitialPassword");
		icon = document.getElementById("ShowPassword1");
	}
	else if (num == 2) {
		passBox = document.getElementById("MainContent_ConfirmPassword");
		icon = document.getElementById("ShowPassword2");
	}

	if (passBox.getAttribute("type") == "password") {
		passBox.setAttribute("type", "text");
		icon.src = icon.src.replace("visibility_", "visibility_off_");
	}
	else {
		passBox.setAttribute("type", "password");
		icon.src = icon.src.replace("visibility_off_", "visibility_");
	}
}