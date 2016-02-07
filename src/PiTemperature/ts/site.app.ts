/// <reference path="typings/jquery.d.ts" />
$(function () {
    $('#okCancelModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        var action = button.data('action') // Extract info from data-* attributes
        var message = button.data('message') // Extract info from data-* attributes
        // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
        // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
        var modal = $(this)
        modal.find('.modal-title').text('Confirm action')
        modal.find('.modal-body p').text(message)
        modal.find('#okCancelModalButton').off('click').click({ param1: action }, modalClickFunction);
    })
});

function modalClickFunction(event) {
    switch (event.data.param1)
    {
        case 'reboot':
            reboot();
            break;
        case 'shutdown':
            shutdown();
            break;
        default:
            alert('Action ' + event.data.param1 + ' not recognized.');
    }
}

function reboot() {
    $.ajax({
        context: this,
        dataType: "json",
        url: "/api/Sensor/Reboot",
        data: null,
        success: function () {
            alert('System is rebooting...');
        }
    });
}

function shutdown() {
    $.ajax({
        context: this,
        dataType: "json",
        url: "/api/Sensor/Shutdown",
        data: null,
        success: function () {
            alert('System is shutting down...');
        }
    });
}