﻿
$(document).ready(function () {
    $('.ActiveCheck').change(function () {
        var self = $(this);
        var id = self.attr('id');
        var value = self.prop('checked');

        $.ajax({
            url: "/ToDoes/AjaxEdit",
            data: {
                id: id,
                value: value
            },
            type: 'Post',
            success: function (result) {
                $('#tableDiv').html(result);
            }
        });
    });
});