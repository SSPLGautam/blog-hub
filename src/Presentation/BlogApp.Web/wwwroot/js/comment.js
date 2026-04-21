$(document).ready(function () {

    var postId = parseInt($("#commentPostId").val());

    // ADD COMMENT
    $("#commentForm").on("submit", function (event) {

        event.preventDefault();

        var content = $("#Content").val();
        var postId = $("#commentPostId").val();

        console.log("PostId:", postId);
        console.log("Content:", content);

        if (!postId || isNaN(parseInt(postId))) {
            alert("PostId is invalid");
            return;
        }
        postId = parseInt(postId);

        if (!content.trim()) {
            alert("Comment cannot be empty");
            return;
        }
        $.ajax({
            url: '/Post/AddComment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                Content: content,
                PostId: postId
            }),
            success: function (response) {
                $("#commentSection").html(response);
                $("#commentForm")[0].reset();
            },
            error: function (xhr) {
                console.log("ERROR:", xhr.responseText);
                alert(xhr.responseText);
            }
        });

    });
    // LIKE FUNCTION
    $("#likeBtn").click(function () {
        var postId = $("#PostId").val();
        $.post("/Post/ToggleLike",
            { postId: postId },

            function (response) {

                $("#likeCount").text(response.count + " Likes");

                if (response.liked) {

                    $("#likeBtn")
                        .removeClass("btn-outline-danger")
                        .addClass("btn-danger")
                        .html("<i class='fa-regular fa-heart'></i> Unlike");

                } else {

                    $("#likeBtn")
                        .removeClass("btn-danger")
                        .addClass("btn-outline-danger")
                        .html("<i class='fa-solid fa-heart'></i> Like");
                }
            });
    });
    // PUBLISH FUNCTION
    $("#publishBtn").click(function () {
            var postId = $(this).data("id");
            $.ajax({
                url: '/Post/TogglePublish',   
                type: 'PUT',                  
                data: { id: postId },         
                success: function () {
                    location.reload();
                },
                error: function () {
                    alert("Something went wrong");
                }
            });

        });

    // DELETE COMMENT
    $(document).on("click", ".deleteComment", function () {

        var commentId = $(this).data("id");
        var postId = $(this).data("post-id");

        if (!confirm("Are you sure you want to delete this comment?")) {
            return;
        }
        $.ajax({
            url: '/Post/DeleteComment',
            type: 'POST',
            data: {
                id: commentId,
                postId: postId
            },
            success: function (response) {

                $("#commentSection").html(response); 
                $("#commentForm")[0].reset();

            },
            error: function () {

                alert("Something went wrong while deleting the comment.");
            }
        })
    })
});




       