

function date() {
    var m = "AM";
    var gd = new Date();
    var secs = gd.getSeconds();
    var minutes = gd.getMinutes();
    var hours = gd.getHours();
    var day = gd.getDay();
    var month = gd.getMonth();
    var date = gd.getDate();
    var year = gd.getYear();

    if (year < 1000) {
        year += 1900;
    }
    if (hours == 0) {
        hours = 12;
    }
    if (hours > 12) {
        hours = hours - 12;
        m = "PM";
    }
    if (secs < 10) {
        secs = "0" + secs;
    }
    if (minutes < 10) {
        minutes = "0" + minutes;
    }

    var dayarray = new Array("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");
    var montharray = new Array("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December");

    var fulldate = dayarray[day] + ", " + montharray[month] + " " + date + ", " + year + " at " + hours + ":" + minutes + ":" + secs + " " + m;
    //$("#date").html(fulldate);
    $('.cMonth').html(montharray[month]);
    $('.cDate').html(date);
    $('.cYear').html(year);
    $('.cDay').html(dayarray[day]);
    $('.cTime').html(hours + ":" + minutes + ":" + secs + " " + m);

    setTimeout("date()", 1000);
}
date();