//// Create a "close" button and append it to each list item
//var myNodelist = document.getElementsByID("LI");
//var i;
//for (i = 0; i < myNodelist.length; i++) {
//    var span = document.createElement("SPAN");
//    var txt = document.createTextNode("\u00D7");
//    span.className = "toDoListClose";
//    span.appendChild(txt);
//    myNodelist[i].appendChild(span);
//}

//// Click on a close button to hide the current list item
//var close = document.getElementsByClassName("toDoListClose");
//var i;
//for (i = 0; i < close.length; i++) {
//    close[i].onclick = function () {
//        var div = this.parentElement;
//        div.style.display = "none";
//    }
//}

//// Add a "checked" symbol when clicking on a list item
//var list = document.querySelector('ul');
//list.addEventListener('click', function (ev) {
//    if (ev.target.tagName === 'LI') {
//        ev.target.classList.toggle('checked');
//    }
//}, false);

//// Create a new list item when clicking on the "Add" button
//function newElement() {
//    var li = document.createElement("li");
//    var inputValue = document.getElementById("myInput").value;
//    var t = document.createTextNode(inputValue);
//    li.appendChild(t);
//    if (inputValue === '') {
//        alert("You must write something!");
//    } else {
//        document.getElementById("myUL").appendChild(li);
//    }
//    document.getElementById("myInput").value = "";

//    var span = document.createElement("SPAN");
//    var txt = document.createTextNode("\u00D7");
//    span.className = "toDoListClose";
//    span.appendChild(txt);
//    li.appendChild(span);

//    for (i = 0; i < close.length; i++) {
//        close[i].onclick = function () {
//            var div = this.parentElement;
//            div.style.display = "none";
//        }
//    }
//}




//new to do list

function get_todos() {
    var todos = new Array;
    var todos_str = localStorage.getItem('todo');
    if (todos_str !== null) {
        todos = JSON.parse(todos_str);
    }
    return todos;
}

function add() {

    var task = document.getElementById('task').value;
    if (task === '')
    {
        alert("ادخل بيانات في حقل المهام اليومية");
    }
    else {
        var todos = get_todos();
        todos.push(task);
        localStorage.setItem('todo', JSON.stringify(todos));
        show();
    }
   
    return false;
}

function remove() {
    var id = this.getAttribute('id');
    var todos = get_todos();
    todos.splice(id, 1);
    localStorage.setItem('todo', JSON.stringify(todos));
    show();

    return false;
}


function show() {



    var todos = get_todos();

    var html = '<ul>';

    for (var i = 0; i < todos.length; i++) {
        html += '<li class="toDoListNormal">' + todos[i] + '<span class="toDoListClose" id="' + i + '">\u00D7</button></li>';
    };
    html += '</ul>';

    document.getElementById('todos').innerHTML = html;

    var buttons = document.getElementsByClassName('toDoListClose');
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].addEventListener('click', remove);

    };

    var toDoListItems = document.getElementsByClassName('toDoListNormal');
    for (var i = 0; i < toDoListItems.length; i++) {
        toDoListItems[i].addEventListener('click', function (e)
        {
            //$('.toDoListNormal').toggleClass("toDoListChecked"); //you can list several class names 
            $(this).toggleClass("toDoListChecked");
            e.preventDefault();
        });
    }
}
    document.getElementById('add').addEventListener('click', add);
    show();


   