document.addEventListener('keyup' ,function searchImages() {

    var input = document.getElementById("imgSearch");
    var filter = input.value.toUpperCase();
    var imgItems = document.getElementById("imgGallery");
    var imgLabel = imgItems.querySelectorAll(".img");

    for (var i = 0; i < imgLabel.length; i++) {

        var item = imgLabel[i].getElementsByTagName("p")[0];
        imgLabel[i].style.display = 'none';
        if (item) {
            if (item.innerHTML.toUpperCase().indexOf(filter) > -1) {
                imgLabel[i].style.display = "";
            } else {
                imgLabel[i].style.display = "none";
            }
        }
    }
});