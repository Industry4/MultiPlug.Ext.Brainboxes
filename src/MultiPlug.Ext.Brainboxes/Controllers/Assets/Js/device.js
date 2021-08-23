
$('#connectbtn').on('click', function (event) {
    event.preventDefault();
    var Group = { Id: ConnectButtonEventId, Pairs: [] };
    $.connection.wS.server.send(Group);
});

$('#restartbtn').on('click', function (event) {
    event.preventDefault();
    var Group = { Id: RestartButtonEventId, Pairs: [] };
    $.connection.wS.server.send(Group);
});

$('.ToggleOutput').change(function () {

    var pair = { "Subject": "value", "Value": "0" };

    if (this.checked) {
        pair = { "Subject": "value", "Value": "1" };
    }

    var Group = { "Id": this.id, "Subjects": [pair] };
    $.connection.wS.server.send(Group);
});

function ApplyHttpToggle() {
    $('.HttpToggleOutput').change(function () {
        $.post("api/multiplug.ext.brainboxes/control/" + DeviceId, { "Id": DeviceId, "Io": this.id.replace("HttpToggleOutput", ""), "State": (this.checked) ? "1" : "0" });
    });
}

function NewEvent(IOid, EventId, Description, RisingEdge, FallingEdge) {
    return '<tr>\
                <td class="span1"><input type="hidden" name="IONumber" value="' + IOid + '"><p>' + IOid + '</p></td>\
                <td class="span3"><input name="EventId" class="input-block-level" type="text" placeholder="Event ID" value="' + EventId + '"></td>\
                <td class="span4"><input name="EventDescription" class="input-block-level" type="text" placeholder="Event Description" value="' + Description + '"></td>\
                <td class="span1"><input name="High" class="input-block-level" type="text" value="' + RisingEdge + '"></td>\
                <td class="span1"><input name="Low" class="input-block-level" type="text" value="' + FallingEdge + '"></td>\
            </tr>'
}

function NewSubscriptions(DeviceGuid, Name, Subscriptions)
{
    var SubscriptionsHtml = "";

    var i;
    for (i = 0; i < Subscriptions.length; i++) {
        SubscriptionsHtml += '<tr>\
    <td><a href="extensions/multiplug.ext.brainboxes/subscription/?device=' + DeviceGuid + '&output=' + Name + '&id=' + Subscriptions[i].Guid + '">' + Subscriptions[i].EventId + '</a></td>\
        <td>\
            <a class="btn btn-red delete-channel" href="extensions/multiplug.ext.brainboxes/subscription/delete/?device=' + DeviceGuid + '&output=' + Name + '&id=' + Subscriptions[i].Guid + '"><i class="icon-trash"></i></a>\
        </td>\
    </tr>'
    }

    return SubscriptionsHtml;
}

function NewOutput(Guid, Name, Subscriptions) {
    return '<div class="accordion-group">\
        <div class="accordion-heading">\
            <div class="row-fluid">\
                <div class="span11">\
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapse-' + Name + '">Output ' + Name + '</a>\
                </div>\
                <div class="span1">\
                    <label class="switch">\
                        <input id="HttpToggleOutput' + Name + '" class="HttpToggleOutput" type="checkbox">\
                        <span class="slider"></span>\
                    </label>\
                </div>\
            </div>\
        </div>\
    </div>\
    <div id="collapse-' + Name + '" class="accordion-body collapse" style="height: 0px;">\
        <div class="accordion-inner">\
            <table class="table table-striped">\
                <thead>\
                    <tr>\
                        <th>Event ID</th>\
                        <th></th>\
                    </tr>\
                </thead>\
                <tbody>\
' + NewSubscriptions(Guid, Name, Subscriptions) + '\
                </tbody>\
            </table>\
            <div class="row-fluid">\
                <div class="span12">\
                    <a class="btn btn-green" href="extensions/multiplug.ext.brainboxes/subscription/?device=' + Guid + '&output=' + Name + '&id=new"><i class="icon-plus-sign"></i></a>\
                </div>\
            </div>\
        </div>\
    </div>\
</div>'
}

$.connection.wS.on("Send", function (id, Group) {

    if (id == LogEventId) {
        var parmfile = $('#textarea-log');
        parmfile.text(parmfile.text() + Group.Pairs[1].Value + "\n");

       // if (parmfile.length)
            parmfile.scrollTop(parmfile[0].scrollHeight);
    }
    else if (id == DeviceStatusEventId) {
        var obj = JSON.parse(Group.Pairs[0].Value);

        $('#devicestatus').text(obj.Status);
        $('#connectbtntext').text(obj.ConnectButtonText);

        var EventsTableHtml = "";

        var i;
        for (i = 0; i < obj.Events.length; i++) {
            EventsTableHtml += NewEvent(obj.Events[i].IOid, obj.Events[i].EventId, obj.Events[i].Description, obj.Events[i].RisingEdge, obj.Events[i].FallingEdge);
        }

        $("#eventsTable tbody tr").remove();
        $('#eventsTable tbody').append(EventsTableHtml);

        var OutputHtml = "";

        for (i = 0; i < obj.Outputs.length; i++) {
            OutputHtml += NewOutput(obj.Guid, obj.Outputs[i].Name, obj.Outputs[i].Subscriptions);
        }

        $("#accordion").empty();
        $("#accordion").append(OutputHtml);

        ApplyHttpToggle();
    }
    else // TODO Add a Array list of all Input Events
    {
        var IOStateColour = $('#IOStateColour-' + Group.Id);
        // TODO IOStateColour -- Get High/Low State values

        if (Group.Pairs[0].Value == "0")
        {
            IOStateColour.removeClass("IOStateGreen");
            IOStateColour.addClass("IOStateRed");
        }
        else if (Group.Pairs[0].Value == "1")
        {
            IOStateColour.removeClass("IOStateRed");
            IOStateColour.addClass("IOStateGreen");
        }
        else
        {
            // TODO Make grey? for unknown
        }        
    }
});