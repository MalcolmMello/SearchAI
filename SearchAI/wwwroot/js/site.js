$(document).ready(function () {
    var data = window.location.href.substring(47);
    var textData = document.getElementById('txtSearch');
    textData.value = data.split("%27").join("").replace("%20", "");
})

function btnSearch() {
    var textData = document.getElementById("txtSearch");

    var url = window.location.origin;
    url = url + "/Home/Search?searchData=" + textData.value;
    window.location.href = url;
}