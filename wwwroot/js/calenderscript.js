
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

document.getElementById("monthname").innerHTML = months[currMonth];
document.getElementById("year").innerHTML = curryear;

const days = document.querySelector(".days");

let days1 = "";

for (let i = 1; i <= firstDay; i++) {
    days1 += `<div></div>`;
}

for (let i = 1; i <= 31; i++) {
    days1 += `<div>${i}</div>`;
}

days.innerHTML = days1;

const days3 = document.querySelector(".daynames");

let namesOfDays = "";

for (let i = 0; i <= dayNames.length - 1; i++) {
    namesOfDays += `<div>${dayNames[i]}</div>`;
    days3.innerHTML = namesOfDays;
}

var numberOfDays = 0;
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
        newDates += `<div>${i}</div>`;
    }
    days.innerHTML = newDates;
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