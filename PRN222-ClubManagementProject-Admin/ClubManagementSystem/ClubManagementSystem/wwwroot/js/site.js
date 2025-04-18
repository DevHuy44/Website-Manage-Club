// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/serverHub")
    .build();

connection.start().then(function () {
    //connection.invoke("GetServerTime").catch(function (err) { console.error(err.toString()); });
}).catch(function (err) { console.log(err) });

connection.on("notifyNews", function (noti) {
    console.log("notifyNews");
    LoadNotifications(noti);
    LoadNotificationsIcon();
})

connection.on("NotifyPost", function (comment, reaction) {
    console.log("notify Post");
    LoadComment(comment);
    LoadReaction(reaction);
})

connection.on("notifyDeletePost", function (comment, reaction) {
    console.log("notify Post");
    DeleteComment(comment);
    DeleteReaction(reaction);
})

function LoadNotificationsIcon(){
    document.getElementById("notiDot").style.display = "block";
};

function LoadNotifications(noti) {
    var newNoti = `<div id="gridRow-${noti.notificationId}" class="row py-3 border-bottom notification-item unread-notification"">
                        <div class="col-1 d-flex align-items-center justify-content-center">
                            <input type="checkbox" class="form-check-input" value="${noti.isRead}">
                        </div>
                        <div class="col-3 text-center d-flex align-items-center"><span>${noti.createdAt}</span></div>
                        <div class="col-3 d-flex align-items-center"><span>${noti.location}</span></div>
                        <div class="col-5 d-flex align-items-center"><span>${noti.message}</span></div>
                    </div>`;
    var div = document.getElementById("gridBody");
    if (div != null) {
        div.insertAdjacentHTML('afterbegin', newNoti);
    }
}

function LoadComment(comment) {
    console.log(comment);

    // Create a new div element for the comment
    var element = document.createElement("div");
    element.id = `comment-${comment.commentId}`;
    element.classList.add("comment-area-box", "media", "mt-4", "d-flex");

    // Set innerHTML with the comment's content
    element.innerHTML = `
        <img src="${comment.user.profilePictureBase64}"
            class="img-fluid"
            style="width: 50px; height: 50px; border-radius: 50%; margin-right: 10px;"
            alt="Profile image">

        <div class="d-flex flex-column w-100">
            <div class="d-flex justify-content-between align-items-center">
                <div>
                    <h4 class="mb-0">${comment.user.username}</h4>
                    <span class="date-comm font-sm text-capitalize text-color">
                       <i class="ti-time mr-2"></i> ${new Date(comment.createdAt).toLocaleString('en-GB').replace(',', '')}
                    </span>
                </div>
            </div>
            <div id="comment-text-${comment.commentId}" class="comment-content mt-2">
                <p>${comment.commentText}</p>
            </div>
        </div>`;

    var commentContainer = document.getElementById(`CommentOnPost-${comment.postId}`);
    if (commentContainer) {
        var oldComment = document.getElementById(`comment-${comment.commentId}`);
        if (oldComment) {
            oldComment.remove(); // Remove old comment
        }
        commentContainer.insertAdjacentElement('afterbegin', element); // Add the new comment
    }
}

function LoadReaction(reaction) {
    console.log(reaction)
}

function DeleteComment(comment) {
    var commentContainer = document.getElementById(`CommentOnPost-${comment.postId}`);
    if (commentContainer) {
        var oldComment = document.getElementById(`comment-${comment.commentId}`);
        if (oldComment) {
            oldComment.remove(); // Remove old comment
        }
    }
}

function DeleteReaction(reaction) {

}