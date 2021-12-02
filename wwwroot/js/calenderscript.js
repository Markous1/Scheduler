
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

const month30 = [
    "April", 
    "June", 
    "September", 
    "November"
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

var currDay = date.getMonth();
document.getElementById("monthname").innerHTML = months[currDay];

const days = document.querySelector(".days");

let days1 = "";
for (let i = 1; i <= 31; i++) {
    days1 += `<div>${i}</div>`;
    days.innerHTML = days1;
  }

const days3 = document.querySelector(".daynames");

let namesOfDays = "";
for (let i = 0; i <= dayNames.length-1; i++) {
    namesOfDays += `<div>${dayNames[i]}</div>`;
    days3.innerHTML = namesOfDays;
}

function nextCal() {
    
    if(currDay == 11){
        currDay = 0;
    }else{
        currDay++;
    }
    document.getElementById("monthname").innerHTML = months[currDay];
}

function prevCal() {
    
    if(currDay == 0){
        currDay = 11;
    }else{
        currDay--;
    }
    document.getElementById("monthname").innerHTML = months[currDay];
}