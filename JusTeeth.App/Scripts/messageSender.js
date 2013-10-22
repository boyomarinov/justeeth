/// <reference path="Kendo/jquery.min.js" />
/// <reference path="jquery.signalR-2.0.0-rc1.js" />

var rooms = [];

$(document).ready(function () {

    $.connection.hub.start();

    var chat = $.connection.chat;

    $("#join-room").click(function () {
        $(this).hide();
        $("#only-joined-users").show();
        var id = $(this).attr("data-group-id");

        chat.server.joinRoom(id);
    });

    $('#send-message').click(function () {
        var msg = $('#message').val();
        chat.server.sendMessageToRoom(msg, rooms[0]);
    });

    chat.client.addMessage = addMessage;
    chat.client.joinRoom = joinRoom;
});

function addMessage(message) {
    $('#chat-window').append('<div>' + message + '</div>');
}

function joinRoom(room) {
    rooms.push(room);
}