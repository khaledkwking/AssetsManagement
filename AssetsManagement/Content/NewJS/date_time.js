//$(document).ready(function () {
//    if (navigator.geolocation) {
//        navigator.geolocation.getCurrentPosition(function (position) {
//            $.getJSON('http://ws.geonames.org/countryCode', {
//                lat: position.coords.latitude,
//                lng: position.coords.longitude,
//                type: 'JSON'
//            }, function (result) {
//                alert('Country: ' + result.countryName + '\n' + 'Code: ' + result.countryCode);
//                $('#newURL').attr('href', 'https://www.google.com&jobid=' + result.countryCode);
//            });
//        });
//    }
//});
//$(document).ready(function () {
//    $.getJSON("http://freegeoip.net/json/", function (result) {
//        alert('Country: ' + result.country_name + '\n' + 'Code: ' + result.country_code);
//        $('#newURL').attr('href', 'https://www.google.com&jobid=' + result.country_code);
//    });
//});

function date_time(id) {
    date = new Date;
    year = date.getFullYear();
    month = date.getMonth();
    months = new Array('January', 'February', 'March', 'April', 'May', 'June', 'Jully', 'August', 'September', 'October', 'November', 'December');
    d = date.getDate();
    day = date.getDay();
    days = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
    am = date.getHours();
    h = date.getHours();
    if (h < 10) {
        h = h;
    }
    if (h > 10) {
        h = h-12;
    }
    m = date.getMinutes();
    if (m < 10) {
        m = "0" + m;
    }
    s = date.getSeconds();
    if (s < 10) {
        s = "0" + s;
    }
    if (am < 10) {
        am = "AM";
    }
    if (am > 10) {
        am = "PM";
    }
    result =' ' + h + ':' + m + ':' + s + '' + am + ' . ' + days[day] + ' . ' + months[month] + ' ' + d + ' ' + year ;
    document.getElementById(id).innerHTML = result;
    setTimeout('date_time("' + id + '");', '1000');
    return true;
}