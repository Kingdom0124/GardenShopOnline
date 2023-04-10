$(function () {
    // Reference the auto-generated proxy for the hub.
    var chat = $.connection.chatHub;
    // Create a function that the hub can call back to display messages.
    chat.client.addNewMessageToPage = function (time, message, userId, toUserId, image) {

        var message_html = message;
        if (image != null) {
            message_html = '<img class="chat-image" src="/assets/images/' + image + '" />' + '<br /><p>' + message + '</p>';
        }

        // Add the message to the page.
        var discussion = $('#discussion');
        if (discussion.data('userid') == userId && discussion.data('currentuser') == toUserId) {
            var avatarImg = $('.chat-avatar img').attr('src');
            discussion.prepend('<li class="chat-left">'
                + '<div class="chat-avatar"><img src="' + avatarImg + '" alt="Retail Admin"></div>'
                + '<div class="chat-text">' + message_html + '</div>'
                + '<div class="chat-hour">' + time + '<span class="fa fa-check-circle ms-1 ml-1"></span></div>'
                + '<div class="chat-avatar"></div>'
                + '</li>');

            // Scroll to bottom
            discussion.scrollTop(discussion.prop('scrollHeight'))
        } else {
            // Add new not seen status
            $('.person .status-' + userId).addClass('busy');
        }
    };
    $.connection.hub.start().done(function () {
        var connectionId = chat.connection.id;
        $(document).on('submit', '#formMessage', function (e) {
            // Prevent default form submit
            e.preventDefault();

            var form = $(this);
            var data = new FormData();
            var files = $('#file').get(0).files;
            var actionUrl = form.attr('action');
            var message = $('#message').val();
            var fromUserId = form.data('fromuserid');
            var toUserId = form.data('touserid');

            if (message || files.length) {

                // Hanle image
                data.append('file', files[0]);
                data.append('message', message);
                data.append('fromUserId', fromUserId);
                data.append('toUserId', toUserId);
                data.append('connectionId', connectionId);

                var message_html = message;

                // Call the Send method on the hub.
                $.ajax({
                    url: actionUrl,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    data: data,
                    success: function (response) {
                        if (response.success) {
                            if (files.length > 0) {
                                message_html = '<img class="chat-image" src="/assets/images/' + response.img + '" />' + '<br /><p>' + response.message + '</p>';
                            }
                            $("#file").val('');
                            $('#output').attr('src', '');
                            // Add chat message
                            var discussion = $('#discussion');
                            discussion.prepend('<li class="chat-right">'
                                + '<div class="chat-hour">' + response.time + '<span class="fa fa-check-circle ms-1 ml-1"></span></div>'
                                + '<div class="chat-text">' + message_html + '</div>'
                                + '<div class="chat-avatar"></div>'
                                + '</li>');

                            // Scroll to bottom
                            discussion.scrollTop(discussion.prop("scrollHeight"))
                        } else {
                            alert('An error has occured, please try again later!')
                        }
                    }
                });
                // Clear text box and reset focus for next comment.
                $('#message').val('').focus();
            }
        });
    });

    $('.person').click(function () {
        var url = $(this).data('chat');
        $.get(url, function (data) {
            // Populate statistics data
            $('#chatDiv').html(data);

            var form = $('#formMessage');
            var actionUrl = form.data('updatestate');
            var fromUserId = form.data('fromuserid');
            var toUserId = form.data('touserid');
            $.ajax({
                url: actionUrl,
                type: 'POST',
                data: { fromUserId, toUserId },
                success: function () {
                    // Refresh admin count of contact
                    $("#contactCount").parent().load('/Home/GetAdminSidebar' + " #contactCount");
                }
            });

            // Remove not seen class
            $('.person .status-' + toUserId).removeClass('busy');
        });
    });

    $(document).on("click", ".chat-image", function () {
        $('#imagepreview').attr('src', $(this).attr('src')); // here asign the image to the modal when the user click the enlarge link
        $('#imageModal').modal('show'); // imagemodal is the id attribute assigned to the bootstrap modal, then i use the show function
    });
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}