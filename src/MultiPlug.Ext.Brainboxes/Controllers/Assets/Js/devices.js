$("#btn-newdevice").click(function () {
    $('#devicesTable tr:last').after(NewDevice());

    $(".btn-deletedevicetemp").click(function (event) {
        event.preventDefault();
        $(this).closest("tr").remove();
    });

});

function ApplyDeleteButton() {

    $(".btn-deletedevice").click(function (event) {
        event.preventDefault();

        var theRow = $(this).closest("tr");

        $.post($(this).attr('href'), function (data) {

        })
        .done(function () {
            theRow.remove();
        });
    });
}

function NewDevice() {
    return '<tr>\
                <td class="span3"><input type="text" name="IPAddress" value=""></td>\
                <td class="span3"><input type="text" name="Name" value=""></td>\
                <td class="span3"></td>\
                <td class="span2"></td>\
                <td class="span1"><a class="btn btn-red btn-deletedevicetemp" href="#"><i class="icon-trash"></i></a></td>\
            </tr>'
}

function NewDeviceLoading(Guid, IPAddress, Name, Location, Status) {
    return '<tr>\
                <td class="span3"><a href="extensions/multiplug.ext.brainboxes/device/?id=' + Guid + '">' + IPAddress + '</a></td>\
                <td class="span3"><a href="extensions/multiplug.ext.brainboxes/device/?id=' + Guid + '">' + Name + '</a></td>\
                <td class="span3"><a href="extensions/multiplug.ext.brainboxes/device/?id=' + Guid + '">' + Location + '</a></td>\
                <td class="span2"><a href="extensions/multiplug.ext.brainboxes/device/?id=' + Guid + '">' + Status + '</a></td>\
                <td class="span1"><a class="btn btn-red btn-deletedevice" href="extensions/multiplug.ext.brainboxes/device/delete/?id=' + Guid + '"><i class="icon-trash"></i></a></td>\
            </tr>'
}

$('#discoverybtn').click(function (e) {
    e.preventDefault();
    $.connection.wS.server.send({ Id: DiscoverDeviceEventId, Pairs: [] });
})

$.connection.wS.on("Send", function (id, Group) {

    if (id == LogEventId) {
        var parmfile = $('#textarea-log');
        parmfile.text(parmfile.text() + Group.Pairs[0].Value + "\n");

        if (parmfile.length)
            parmfile.scrollTop(parmfile[0].scrollHeight - parmfile.height());
    }
    else if (id == DevicesListEventId) {

        var DeviceList = JSON.parse(Group.Pairs[0].Value);

        var DeviceTableHtml = "";

        var i;
        for (i = 0; i < DeviceList.length; i++) {
            DeviceTableHtml += NewDeviceLoading(DeviceList[i].Guid, DeviceList[i].IPAddress, DeviceList[i].Name, DeviceList[i].Location, DeviceList[i].Status);
        }

        $("#devicesTable tbody tr").remove();
        $('#devicesTable tbody').append(DeviceTableHtml);

        ApplyDeleteButton();
    }
});

ApplyDeleteButton();
