$(document).ready(function () {
    
    /*https://flatpickr.js.org/examples/*/
    var today = new Date(),
        dow = today.getDay(),
        toAdd = dow === 0 ? 0 : 7 - dow,
        thisSunday = new Date(),
        lastMonday = new Date();

    thisSunday.setDate(thisSunday.getDate() + toAdd);
    lastMonday.setDate(thisSunday.getDate() - 7);
    
    GetTimesheetDateList(GetFormatedDate(weekEndingDate), BindTimesheetPicker);

    if (isUserSelectPageRequired == "True") {
        $("#reviewUserTimesheetModel").modal("show");
    }
    if (status == "True") {
        swal.fire({
            title: "Saved!",
            text: "Timesheet Data Saved Successfully!",
            icon: "success",
            button: "Ok",
            confirmButtonColor: "#009ef7"
        });
    }
    if (noItemSavedStatus == "True") {
        swal.fire({
            title: "No Item Saved!",
            text: "Please add atleast one Timesheet Item!",
            icon: "error",
            button: "Ok",
            confirmButtonColor: "#009ef7"
        });
    }
    if (isTimeLimitCrossed == "True") {
        swal.fire({
            title: "Time limit exceeded!",
            text: "Timesheet hours can't be more than 24 Hours!",
            icon: "error",
            button: "Ok",
            confirmButtonColor: "#009ef7"
        });
    }

    LoadTimesheetDetails(obj, list);

    var a = document.querySelector("#kt_modal_new_card")
    var today = new Date(),
        dow = today.getDay(),
        toAdd = dow === 0 ? 0 : 7 - dow,
        thisSundayForReview = new Date();

    thisSundayForReview.setDate(thisSundayForReview.getDate() + toAdd);
    $("#ReviewWeekEndingDate").flatpickr({
        "disable": [
            function (date) {
                // return true to disable
                return (date.getDay() === 1 || date.getDay() === 2 || date.getDay() === 3 || date.getDay() === 4 || date.getDay() === 5 || date.getDay() === 6);

            }
        ],
        defaultDate: thisSundayForReview,
        dateFormat: "d-m-Y",
        onChange: function (rawdate, altdate, FPOBJ) {
            FPOBJ.close();
        },

    });

    $(document).on("click", "#add", function () {
        var form = $('#timesheetDetails')[0];
        var formData = new FormData(form);
        if ($("#TimesheetDate").val() != "" && $("#WeekEndingDate").val() != "") {

            $.ajax({
                type: "POST",
                url: addItemUrl,
                data: formData,
                processData: false,
                contentType: false,
                cache: false,
                async: false,
                success: function (result) {
                    $("#timesheetItemsList").html(result);
                    if (pwObj.isTimeSheetExists) {
                        swal.fire({
                            title: "Timesheet already exists!",
                            text: "You have " + pwObj.existingTotalHours + " hours " + pwObj.existingTotalMinutes + " minutes already entered for this date. Please choose another date.",
                            icon: "error",
                            button: "Ok",
                            confirmButtonColor: "#009ef7"
                        });
                    }
                    else if (pwObj.totalHours >= 24) {
                        if (!(pwObj.totalHours == 24 && pwObj.totalMinutes == 0)) {
                            swal.fire({
                                title: "Time limit exceeded!",
                                text: "Timesheet hours can't be more than 24 Hours!",
                                icon: "error",
                                button: "Ok",
                                confirmButtonColor: "#009ef7"
                            });
                        }
                    }

                    LoadTimesheetDetails(pwObj, pwList);
                },
                error: function (result) {
                }
            });
        } else {
            if ($("#TimesheetDate").val() == "") {
                swal.fire({
                    title: "Error!",
                    text: "Please select timesheet date.",
                    icon: "error",
                    button: "Ok",
                    confirmButtonColor: "#009ef7"
                });
            }
            else {
                swal.fire({
                    title: "Error!",
                    text: "Please select week ending date.",
                    icon: "error",
                    button: "Ok",
                    confirmButtonColor: "#009ef7"
                });
            }

            return false;
        }
    });


    $(document).on("click", "#delete", function () {
        var form = $('#timesheetDetails')[0];
        var formData = new FormData(form);
        if ($("#TimesheetDate").val() != "" && $("#WeekEndingDate").val() != "") {

            $.ajax({
                type: "POST",
                url: removeItemUrl + "?index=" + $(this).data('id'),
                data: formData,
                processData: false,
                contentType: false,
                cache: false,
                async: false,
                success: function (result) {
                    $("#timesheetItemsList").html(result);

                    LoadTimesheetDetails(pwObj, pwList);
                },
                error: function (result) {
                }
            });
        } else {
            if ($("#TimesheetDate").val() == "") {
                swal.fire({
                    title: "Error!",
                    text: "Please select timesheet date.",
                    icon: "error",
                    button: "Ok",
                    confirmButtonColor: "#009ef7"
                });
            }
            else {
                swal.fire({
                    title: "Error!",
                    text: "Please select week ending date.",
                    icon: "error",
                    button: "Ok",
                    confirmButtonColor: "#009ef7"
                });
            }

            return false;
        }
    });

    $(document).on("change", "#WeekEndingDate", function (e) {

        if (e.currentTarget.value != '') {
            GetTimesheetDateList(e.currentTarget.value, BindTimesheetDate);
        }
    });

    $(document).on("change", "#TimesheetDate", function (e) {
        for (var i = 0; i < existingTimesheetDate.length; i++) {
            DisableTimesheetDates(existingTimesheetDate[i]);
        }
    });

    $('#kt_modal_new_card').on('show.bs.modal', function () {
        var today = new Date(),
            dow = today.getDay(),
            toAdd = dow === 0 ? 0 : 7 - dow,
            thisSundayForReview = new Date();

        thisSundayForReview.setDate(thisSundayForReview.getDate() + toAdd);
        var date = $("#WeekEndingDate").val() == '' ? '' : $("#WeekEndingDate").val();
        $("#ReviewWeekEndingDate").flatpickr({
            "disable": [
                function (date) {
                    // return true to disable
                    return (date.getDay() === 1 || date.getDay() === 2 || date.getDay() === 3 || date.getDay() === 4 || date.getDay() === 5 || date.getDay() === 6);

                }
            ],
            defaultDate: date,
            dateFormat: "d-m-Y",
            onChange: function (rawdate, altdate, FPOBJ) {
                FPOBJ.close();
            },

        });

        if ($("#WeekEndingDate").val() != '') {
            $("#ReviewWeekEndingDate").change();
        }
        else {
            $("#ReviewWeekEndingDate").val('').change();
            $(".reviewTimesheetDetails").val("");
            $(".reviewTimesheetDetailsBtn").attr("data-id", -1);
        }
    })

    $('#kt_modal_new_card').on('hide.bs.modal', function (e) {

    })

    $('#timesheetDetails').submit(function (e) {
        if ($("#TimesheetDate").val() != "" && $("#WeekEndingDate").val() != "") {
            e.preventDefault();
            var $form = $(this);
            $.ajax({
                type: "GET",
                contentType: "application/json; charset=utf-8",
                context: $form, // context will be "this" in your handlers
                url: getTimesheetHoursByDateUrl + "?date=" + $("#TimesheetDate").val(),
                success: function (result) {

                    if (result.isDataExists) {
                        var totalMinutes = 0;
                        var totalHours = 0;
                        totalHours = parseFloat(parseInt(result.totalMinutes / 60) + result.totalHours).toFixed(0)
                        totalMinutes = parseFloat(result.totalMinutes % 60).toFixed(0)
                        swal.fire({
                            title: "Timesheet already exists!",
                            text: "You have " + totalHours + " hours " + totalMinutes + " minutes already entered for this date. Please choose another date.",
                            icon: "error",
                            button: "Ok",
                            confirmButtonColor: "#009ef7"
                        });
                    }
                    else {
                        $('#timesheetDetails').unbind('submit').submit();
                    }
                },
                error: function (result) {
                }
            });
        }
        else {
            if ($("#TimesheetDate").val() == "") {
                swal.fire({
                    title: "Error!",
                    text: "Please select timesheet date.",
                    icon: "error",
                    button: "Ok",
                    confirmButtonColor: "#009ef7"
                });
            }
            else {
                swal.fire({
                    title: "Error!",
                    text: "Please select week ending date.",
                    icon: "error",
                    button: "Ok",
                    confirmButtonColor: "#009ef7"
                });
            }

            return false;
        }
    });

    $("#ReviewWeekEndingDate").on("change", function () {
        SearchUserTimesheet()
    });

    $("#TimesheetUser").on("change", function () {
        SearchUserTimesheet()
    });
});

function SearchUserTimesheet() {

    if ($("#ReviewWeekEndingDate").val() != '' && $("#TimesheetUser").val() != '') {
        $.ajax({
            type: "GET",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            url: getTimesheetByUser + "?endDate=" + $("#ReviewWeekEndingDate").val() + "&userId=" + $("#TimesheetUser").val(),
            success: function (result) {
                $(".reviewTimesheetDetails").val("");
                $(".reviewTimesheetDetailsBtn").attr("data-id", -1);
                for (var i = 0; i < result.length; i++) {
                    if (result[i].dayName.toLowerCase() == "monday") {
                        $("#monday_date").val(moment(result[i].date).format('DD-MM-YYYY'));
                        $("#monday_totalHours").val(result[i].totalHours);
                        $("#monday_totalMinutes").val(result[i].totalMinutes);
                        $("#monday_btn").attr("data-id", result[i].timeSheetId);
                        $("#monday_edit_btn").attr("data-id", result[i].timeSheetId);
                    }
                    if (result[i].dayName.toLowerCase() == "tuesday") {
                        $("#tuesday_date").val(moment(result[i].date).format('DD-MM-YYYY'));
                        $("#tuesday_totalHours").val(result[i].totalHours);
                        $("#tuesday_totalMinutes").val(result[i].totalMinutes);
                        $("#tuesday_btn").attr("data-id", result[i].timeSheetId);
                        $("#tuesday_edit_btn").attr("data-id", result[i].timeSheetId);
                    }
                    if (result[i].dayName.toLowerCase() == "wednesday") {
                        $("#wednesday_date").val(moment(result[i].date).format('DD-MM-YYYY'));
                        $("#wednesday_totalHours").val(result[i].totalHours);
                        $("#wednesday_totalMinutes").val(result[i].totalMinutes);
                        $("#wednesday_btn").attr("data-id", result[i].timeSheetId);
                        $("#wednesday_edit_btn").attr("data-id", result[i].timeSheetId);
                    }
                    if (result[i].dayName.toLowerCase() == "thursday") {
                        $("#thursday_date").val(moment(result[i].date).format('DD-MM-YYYY'));
                        $("#thursday_totalHours").val(result[i].totalHours);
                        $("#thursday_totalMinutes").val(result[i].totalMinutes);
                        $("#thursday_btn").attr("data-id", result[i].timeSheetId);
                        $("#thursday_edit_btn").attr("data-id", result[i].timeSheetId);
                    }
                    if (result[i].dayName.toLowerCase() == "friday") {
                        $("#friday_date").val(moment(result[i].date).format('DD-MM-YYYY'));
                        $("#friday_totalHours").val(result[i].totalHours);
                        $("#friday_totalMinutes").val(result[i].totalMinutes);
                        $("#friday_btn").attr("data-id", result[i].timeSheetId);
                        $("#friday_edit_btn").attr("data-id", result[i].timeSheetId);
                    }
                    if (result[i].dayName.toLowerCase() == "saturday") {
                        $("#saturday_date").val(moment(result[i].date).format('DD-MM-YYYY'));
                        $("#saturday_totalHours").val(result[i].totalHours);
                        $("#saturday_totalMinutes").val(result[i].totalMinutes);
                        $("#saturday_btn").attr("data-id", result[i].timeSheetId);
                        $("#saturday_edit_btn").attr("data-id", result[i].timeSheetId);
                    }
                    if (result[i].dayName.toLowerCase() == "sunday") {
                        $("#sunday_date").val(moment(result[i].date).format('DD-MM-YYYY'));
                        $("#sunday_totalHours").val(result[i].totalHours);
                        $("#sunday_totalMinutes").val(result[i].totalMinutes);
                        $("#sunday_btn").attr("data-id", result[i].timeSheetId);
                        $("#sunday_edit_btn").attr("data-id", result[i].timeSheetId);
                    }
                    //Do something
                }

                var timesheetId = -1;
                var reviewWeekEndingDate = $("#ReviewWeekEndingDate").val();
                GetTimesheetData(timesheetId, reviewWeekEndingDate);

            },
            error: function (result) {
            }
        });
    }
    else {
        
    }
}

function GetTimesheetForEdit(e) {
    //debugger
    //e.dataset.id
    if (e.dataset.id > 0) {
        window.location.href = "?timesheetId=" + e.dataset.id + "&userId=" + $("#TimesheetUser").val();
    }
    else {

    }
    
}

function updateTimesheetItem() {

    var formItem = $('#edittimesheetItemForm')[0];
    var formData = new FormData(formItem);

    $.ajax({
        type: "POST",
        url: saveTimesheetItemUrl,
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        async: false,
        success: function (result) {
            if (result.status) {
                $('#editTimesheetItem').modal('hide');
                $("#timesheetItemsList").html(result.html);

                LoadTimesheetDetails(pwObj, pwList);
            }
            else {
                $('#editTimesheetItem').modal('show');
                $("#editTimesheetItemSection").html(result.html);
                if (result.message != undefined) {
                    if (result.message == "TimesheetHours") {
                        swal.fire({
                            title: "Time limit exceeded!",
                            text: "Timesheet hours can't be more than 24 Hours!",
                            icon: "error",
                            button: "Ok",
                            confirmButtonColor: "#009ef7"
                        });
                    }
                }
                LoadTimesheetITemDetails();
            }
            console.log(result);
        },
        error: function (result) {
        }
    });
}

function BindTimesheetDate(result) {
    var weekEndingDate = $("#WeekEndingDate").val();
    var sunday = GetDate(weekEndingDate);
    var monday = new Date(sunday);
    monday = new Date(monday.setDate(monday.getDate() - 6));

    var start = new Date(monday),
        end = new Date(sunday),
        between = [];

    while (start <= end) {
        between.push(GetFormatedDate(start));
        start.setDate(start.getDate() + 1);
    }

    for (var i = 0; i < result.length; i++) {
        between.splice($.inArray(GetFormatedDate(result[i].timesheetDate), between), 1);
    }

    $("#TimesheetDate").flatpickr({
        dateFormat: "d-m-Y",
        enable: between,
        defaultDate: between.length > 0 ? between[0] : "",
        onReady(_, __, fp) {
            fp.calendarContainer.classList.add("timesheetDatepickerClass");
        }
    });

    existingTimesheetDate = [];
    for (var i = 0; i < result.length; i++) {
        existingTimesheetDate.push(result[i].timesheetDate);
        DisableTimesheetDates(result[i].timesheetDate);
    }
    if (between.length == 0) {
        $("#TimesheetDate").val("");
    }

}

function GetTimesheetDateList(endDate, successCallback, errorCallback) {

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: getTimesheetDatesByEndDateUrl + "?endDate=" + endDate,
        success: function (result) {
            if (successCallback != undefined) {
                successCallback(result);
            }
        },
        error: function (result) {
        }
    });
}

function BindTimesheetPicker(result) {
    var today = new Date(),
        dow = today.getDay(),
        toAdd = dow === 0 ? 0 : 7 - dow,
        thisSunday = new Date(),
        lastMonday = new Date();

    thisSunday.setDate(thisSunday.getDate() + toAdd);
    lastMonday.setDate(thisSunday.getDate() - 6);

    var start = lastMonday,
        end = thisSunday,
        between = [];

    while (start <= end) {
        between.push(GetFormatedDate(start));
        start.setDate(start.getDate() + 1);
    }

    for (var i = 0; i < result.length; i++) {
        between.splice($.inArray(GetFormatedDate(result[i].timesheetDate), between), 1);
    }

    $("#TimesheetDate").flatpickr({
        defaultDate: timesheetDate,
        dateFormat: "d-m-Y",
        enable: [timesheetDate]
    });

    $("#WeekEndingDate").flatpickr({
        "disable": [
            function (date) {
                // return true to disable
                return (date.getDay() === 1 || date.getDay() === 2 || date.getDay() === 3 || date.getDay() === 4 || date.getDay() === 5 || date.getDay() === 6);

            }
        ],
        defaultDate: weekEndingDate,
        dateFormat: "d-m-Y",
    });
    if (minDate == "True") {
        $("#TimesheetDate").val("");
        $("#WeekEndingDate").val("");
        $("#add").attr("disabled", true);
        $(".saveTimeSheetData").attr("disabled", true);
    }
}

function LoadTimesheetDetails(timesheetObject, timesheetListObject) {
    $("#TotalHours").val(timesheetObject.totalHours)
    $("#TotalMinutes").val(timesheetObject.totalMinutes)
    for (var i = 0; i < timesheetListObject.length; i++) {
        if (timesheetListObject[i].activityTypeId > 0) {
            var actList = activityList.filter(function (itm) {
                return itm.activityTypeId == timesheetListObject[i].activityTypeId
            });
            var itr = i;
            $("#TimesheetItems_" + itr + "__ActivityId").html('');
            var options = '';
            options = '<option value="">-- Select --</option>';
            for (var j = 0; j < actList.length; j++) {
                options += '<option value="' + actList[j].activityId + '">' + actList[j].activityName + '</option>';
            }

            $("#TimesheetItems_" + itr + "__ActivityId").append(options);
            $("#TimesheetItems_" + itr + "__ActivityId").val(timesheetListObject[itr].activityId).change();

        }
        $("#TimesheetItems_" + i + "__ActivityTypeId").attr('data-control', 'select2');
        $("#TimesheetItems_" + i + "__ActivityTypeId").select2();
        $("#TimesheetItems_" + i + "__ActivityId").attr('data-control', 'select2');
        $("#TimesheetItems_" + i + "__ActivityId").select2();

        $("#TimesheetItems_" + i + "__Hours").attr('data-control', 'select2');
        $("#TimesheetItems_" + i + "__Hours").select2();

        $("#TimesheetItems_" + i + "__Minutes").attr('data-control', 'select2');
        $("#TimesheetItems_" + i + "__Minutes").select2();
    }
}

function GetTimesheetItems(e) {
    var timesheetId = e.getAttribute("data-id");
    var reviewWeekEndingDate = $("#ReviewWeekEndingDate").val();
    GetTimesheetData(timesheetId, reviewWeekEndingDate);
}

function GetTimesheetItemForEdit(e) {
    var form = $('#timesheetDetails')[0];
    var formData = new FormData(form);

    $.ajax({
        type: "POST",
        url: getTimesheetItemForEdit + "?index=" + e,
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        async: false,
        success: function (result) {
            $("#editTimesheetItemSection").html("");
            $("#editTimesheetItemSection").html(result);

            LoadTimesheetITemDetails();

        },
        error: function (result) {
        }
    });
}

function LoadTimesheetITemDetails() {

    $("#ActivityTypeId").attr('data-control', 'select2');
    $("#ActivityTypeId").select2();

    $("#ActivityId").attr('data-control', 'select2');
    $("#ActivityId").select2();

    $("#Hours").attr('data-control', 'select2');
    $("#Hours").select2();

    $("#Minutes").attr('data-control', 'select2');
    $("#Minutes").select2();

    $('#ActivityTypeId').select2({
        dropdownParent: $('#editTimesheetItem')
    });

    $('#ActivityId').select2({
        dropdownParent: $('#editTimesheetItem')
    });

    $('#Hours').select2({
        dropdownParent: $('#editTimesheetItem')
    });

    $('#Minutes').select2({
        dropdownParent: $('#editTimesheetItem')
    });

}

function GetTimesheetData(timesheetId, reviewWeekEndingDate) {
    $.ajax({
        type: "GET",
        url: getTimesheetDetailsByTimesheetId + "?timesheetId=" + timesheetId + "&endDate=" + reviewWeekEndingDate + "&userId=" + $("#TimesheetUser").val(),
        /*data: formData,*/
        processData: false,
        contentType: false,
        cache: false,
        async: false,
        success: function (result) {
            $("#reviewTimesheetItemsTable").html(result);
        },
        error: function (result) {
        }
    });
}

function updateHoursMinutes(data) {
    var totalMinutes = 0;
    var totalHours = 0;
    $('select.itemMinutes').each(function (idx, el) {
        if (el.value != '') {
            totalMinutes += parseInt(el.value);
        }
    })
    $('select.itemHours').each(function (idx, el) {
        if (el.value != '') {
            totalHours += parseInt(el.value);
        }
    })
    $("#TotalHours").val(parseFloat(parseInt(totalMinutes / 60) + totalHours).toFixed(0))
    $("#TotalMinutes").val(parseFloat(totalMinutes % 60).toFixed(0))
}

function getActivityType(data) {
    var actList = activityList.filter(function (obj) {
        return obj.activityTypeId == data.value
    });
    var itr = data.id.split("_")[1];
    $("#TimesheetItems_" + itr + "__ActivityId").html('');
    var options = '';

    options = '<option value="">-- Select --</option>';
    for (var i = 0; i < actList.length; i++) {
        options += '<option value="' + actList[i].activityId + '">' + actList[i].activityName + '</option>';
    }

    $("#TimesheetItems_" + itr + "__ActivityId").append(options);
    $("#TimesheetItems_" + itr + "__ActivityId").attr('data-control', 'select2');
    $("#TimesheetItems_" + itr + "__ActivityId").select2();

}

function getActivityTypeForEdit(data) {
    var actList = activityList.filter(function (obj) {
        return obj.activityTypeId == data.value
    });

    $("#ActivityId").html('');

    var options = '';
    options = '<option value="">-- Select --</option>';
    for (var i = 0; i < actList.length; i++) {
        options += '<option value="' + actList[i].activityId + '">' + actList[i].activityName + '</option>';
    }

    $("#ActivityId").append(options);
    $("#ActivityId").attr('data-control', 'select2');
    $("#ActivityId").select2();
}

function GetFormatedDate(value) {
    return moment(value).format('DD-MM-YYYY');
}

function GetDate(value) {
    if (value != '') {
        return new Date(value.split("-")[2] + "-" + value.split("-")[1] + "-" + value.split("-")[0]);
    }
    return '';
}

function DisableTimesheetDates(value) {
    var areaLable = GetAreaLabelDate(value);
    $(".timesheetDatepickerClass span.flatpickr-day.flatpickr-disabled[aria-label*='" + areaLable + "']").css("background-color", "grey").css("color", "white");
    $(".timesheetDatepickerClass span.flatpickr-day.flatpickr-disabled[aria-label*='" + areaLable + "']").addClass("disabledDate");
}

function GetAreaLabelDate(value) {
    if (value != '') {
        return moment(value).format('MMMM D, YYYY');
    }
    return '';
}