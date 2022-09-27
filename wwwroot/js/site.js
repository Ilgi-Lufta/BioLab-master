// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function myFunction() {
    var element = document.getElementById("myDIV");
    if(element.classList.contains("none")){
        element.classList.remove("none");

    }else{

        element.classList.add("none");
    }
  }