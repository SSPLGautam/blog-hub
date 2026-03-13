// TinyMCE Editor
tinymce.init({
    selector: 'textarea'
});

// Image Preview Function
function previewImage(event) {

    var file = event.target.files[0];

    if (!file) return;

    var reader = new FileReader();

    reader.onload = function (e) {

        var preview = document.getElementById("imagePreview");

        preview.src = e.target.result;
        preview.style.display = "block";
    };

    reader.readAsDataURL(file);
}