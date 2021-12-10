
const date = new Date();

const months = [
    "January",
    "Febuary",
    "March",
    "April",
    "May",
    "June",
    "July",
    "Augaust",
    "September",
    "October",
    "November",
    "December"
];

const dayNames = [
    "Sun",
    "Mon",
    "Tue",
    "Wed",
    "Thu",
    "Fri",
    "Sat"
];

var currMonth = date.getMonth();
var currDay = date.getDate();
var curryear = date.getFullYear();
var firstDay = new Date(curryear, currMonth, 1).getDay();
var namesOfDays = "";
var numberOfDays = 0;

const days = document.querySelector(".days");
const daysNames = document.querySelector(".daynames");


document.getElementById("monthname").innerHTML = months[currMonth];
document.getElementById("year").innerHTML = curryear;

renderCalander(firstDay);

for (let i = 0; i <= dayNames.length - 1; i++) {
    namesOfDays += `<div>${dayNames[i]}</div>`;
    daysNames.innerHTML = namesOfDays;
}

function renderCalander(firstDays) {

    var newDates = "";
    while (days.firstChild) {
        days.removeChild(days.firstChild);
    }

    dayInMoths(currMonth, curryear);

    for (let i = 1; i <= firstDays; i++) {
        newDates += `<div></div>`;
    }

    for (let i = 1; i <= numberOfDays; i++) {
        newDates += `<div id = "${currMonth}/${i}/${curryear}" class = "numberDays"onclick="create()">${i}</div>`;
    }
    days.innerHTML = newDates;
}

function dayInMoths(month, year) {

    if (month == 1) {
        if (year % 4 == 0) {
            numberOfDays = 29
        } else {
            numberOfDays = 28;
        }
    }
    else if (month == 3 || month == 5 || month == 7 || month == 8) {
        numberOfDays = 30;
    }
    else {
        numberOfDays = 31;
    }
}

function nextCal() {

    if (currMonth == 11) {
        currMonth = 0;
        curryear++;
    } else {
        currMonth++;
    }
    firstDay = new Date(curryear, currMonth, 1).getDay();
    renderCalander(firstDay, currMonth);
    document.getElementById("monthname").innerHTML = months[currMonth];
    document.getElementById("year").innerHTML = curryear;
}

function prevCal() {

    if (currMonth == 0) {
        currMonth = 11;
        curryear--;
    } else {
        currMonth--;
    }
    firstDay = new Date(curryear, currMonth, 1).getDay();
    renderCalander(firstDay, currMonth);
    document.getElementById("monthname").innerHTML = months[currMonth];
    document.getElementById("year").innerHTML = curryear;
}

function create() {
    window.location.href = '/Events/Create';
}
