//Main Menu
function openMenu() {
    document.getElementById("PushMenu").style.width = "250px";
    document.getElementById("main").style.marginRight = "250px";
    document.getElementById("infrmationCenter").style.width = "0";
    document.getElementById("main").style.marginLeft = "0";

}

function closeMenu() {
    document.getElementById("PushMenu").style.width = "0";
    document.getElementById("main").style.marginRight = "0";
}

// Information Center
function openInformationCenter() {
    document.getElementById("infrmationCenter").style.width = "250px";
    document.getElementById("main").style.marginRight = "250px";
    document.getElementById("PushMenu").style.width = "0";
    document.getElementById("main").style.marginLeft = "0";
}

function closeInformationCenter() {
    document.getElementById("infrmationCenter").style.width = "0";
    document.getElementById("main").style.marginRight = "0";
}

//Notification Center
function openNotificationCenter() {
document.getElementById("notificationCenter").style.width = "250px";
document.getElementById("main").style.marginLeft = "250px";
document.getElementById("PushMenu").style.width = "0";
document.getElementById("main").style.marginRight = "0";
document.getElementById("infrmationCenter").style.width = "0";
document.getElementById("main").style.marginRight = "0";
}

function closeNotificationCenter() {
    document.getElementById("notificationCenter").style.width = "0";
    document.getElementById("main").style.marginLeft = "0";
}

