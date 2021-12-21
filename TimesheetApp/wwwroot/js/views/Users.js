$(document).ready(function () {
    GetUsers();

    $("#updateUser").on("click", function () {
        var id = $("#EditUserId").val();
        var userRoleId = $("#UserRoleTypeList").val();
        var isDisabled = $("#isDisabled").prop("checked");

        if (id != "" && userRoleId != "") {
            var data = { userId: id, userRoleId: userRoleId, isDisabled: isDisabled };
            $.ajax({
                type: "POST",
                url: updateUserUrl,
                data: data,
                success: function (result) {
                    if (result.status) {
                        $("#editUser").modal("hide");
                        Swal.fire(
                            {
                                title: 'Success!',
                                text: 'Record updated successfully!',
                                icon: 'success',
                                confirmButtonColor: "#009ef7",
                            }
                        )
                        GetUsers();
                    }
                    else {
                        Swal.fire(
                            {
                                title: 'Error!',
                                text: 'Something went wrong!',
                                icon: 'error',
                                confirmButtonColor: "#009ef7",
                            }
                        )
                    }
                },
                error: function (result) {
                }
            });
        }
        else {
            Swal.fire(
                {
                    title: 'Error!',
                    text: 'Please enter/select valid data!',
                    icon: 'error',
                    confirmButtonColor: "#009ef7",
                }
            )
        }
    });

    $("#updateUserPassword").on("click", function () {
        var id = $("#EditPasswordUserId").val();
        var newPassword = $("#newPassword").val();
        var confirmPassword = $("#confirmPassword").val();

        if (id != "" && newPassword != "" && confirmPassword != "") {
            if (newPassword != confirmPassword) {
                Swal.fire(
                    {
                        title: 'Error!',
                        text: 'New password and confirm password should be same.',
                        icon: 'error',
                        confirmButtonColor: "#009ef7",
                    }
                )
            }
            else {
                var data = { userId: id, newPassword: newPassword};
                $.ajax({
                    type: "POST",
                    url: updateUserPasswordUrl,
                    data: data,
                    success: function (result) {
                        if (result.status) {
                            $("#editUserPassword").modal("hide");
                            Swal.fire(
                                {
                                    title: 'Success!',
                                    text: 'Record updated successfully!',
                                    icon: 'success',
                                    confirmButtonColor: "#009ef7",
                                }
                            )
                            $("#EditPasswordUserId").val("");
                            $("#confirmPassword").val("");
                            $("#newPassword").val("");
                            GetUsers();
                        }
                        else {
                            Swal.fire(
                                {
                                    title: 'Error!',
                                    text: result.message,
                                    icon: 'error',
                                    confirmButtonColor: "#009ef7",
                                }
                            )
                        }
                    },
                    error: function (result) {
                    }
                });
            }
        }
        else {
            if (newPassword == "" || confirmPassword == "") {
                Swal.fire(
                    {
                        title: 'Error!',
                        text: 'Please enter data!',
                        icon: 'error',
                        confirmButtonColor: "#009ef7",
                    }
                )
            }
        }
    });

});

function GetUsers() {
    $.ajax({
        type: "GET",
        data: null,
        url: getActivityUrl,
        success: function (result) {
            $("#userTable").dataTable({
                destroy: true,
                data: result.data,
                columns: [
                    { "data": "firstName", "autowidth": true },
                    { "data": "lastName", "autowidth": true },
                    { "data": "email", "autowidth": true },
                    { "data": "roleName", "autowidth": true },
                    {
                        "autowidth": true, "sorting": false,
                        "mRender": function (data, type, row) {
                            return row.isDisabled ? "<span>&#10003;</span>" : "";
                        },
                    },
                    {
                        "autowidth": true, "sorting": false,
                        "mRender": function (data, type, row) {
                            return GetActionColumns(data, type, row);
                        },
                    }
                ]
            })
        }
    });
}

function GetActionColumns(data, type, row) {
    var editActivityLink = "";
    var changePassword = "";
    editActivityLink = '<a href="JavaScript:void(0)" onclick="GetUserByID(' + "'" + row.id + "'" + ')" class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm me-1" data-bs-toggle="modal" data-bs-target="#editUser"> <span class="svg-icon svg-icon-3"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none"> <path opacity="0.3" d="M21.4 8.35303L19.241 10.511L13.485 4.755L15.643 2.59595C16.0248 2.21423 16.5426 1.99988 17.0825 1.99988C17.6224 1.99988 18.1402 2.21423 18.522 2.59595L21.4 5.474C21.7817 5.85581 21.9962 6.37355 21.9962 6.91345C21.9962 7.45335 21.7817 7.97122 21.4 8.35303ZM3.68699 21.932L9.88699 19.865L4.13099 14.109L2.06399 20.309C1.98815 20.5354 1.97703 20.7787 2.03189 21.0111C2.08674 21.2436 2.2054 21.4561 2.37449 21.6248C2.54359 21.7934 2.75641 21.9115 2.989 21.9658C3.22158 22.0201 3.4647 22.0084 3.69099 21.932H3.68699Z" fill="black"></path> <path d="M5.574 21.3L3.692 21.928C3.46591 22.0032 3.22334 22.0141 2.99144 21.9594C2.75954 21.9046 2.54744 21.7864 2.3789 21.6179C2.21036 21.4495 2.09202 21.2375 2.03711 21.0056C1.9822 20.7737 1.99289 20.5312 2.06799 20.3051L2.696 18.422L5.574 21.3ZM4.13499 14.105L9.891 19.861L19.245 10.507L13.489 4.75098L4.13499 14.105Z" fill="black"></path> </svg> </span> </a>';

    changePassword = '<a href="JavaScript:void(0)" onclick="SetId(' + "'" + row.id + "','" + row.firstName + "','" + row.lastName + "','" + row.email + "'" + ')" class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm me-1" data-bs-toggle="modal" data-bs-target="#editUserPassword"> <span class="svg-icon svg-icon-3"> <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none"> <path opacity="0.3" d="M20.5543 4.37824L12.1798 2.02473C12.0626 1.99176 11.9376 1.99176 11.8203 2.02473L3.44572 4.37824C3.18118 4.45258 3 4.6807 3 4.93945V13.569C3 14.6914 3.48509 15.8404 4.4417 16.984C5.17231 17.8575 6.18314 18.7345 7.446 19.5909C9.56752 21.0295 11.6566 21.912 11.7445 21.9488C11.8258 21.9829 11.9129 22 12.0001 22C12.0872 22 12.1744 21.983 12.2557 21.9488C12.3435 21.912 14.4326 21.0295 16.5541 19.5909C17.8169 18.7345 18.8277 17.8575 19.5584 16.984C20.515 15.8404 21 14.6914 21 13.569V4.93945C21 4.6807 20.8189 4.45258 20.5543 4.37824Z" fill="black"></path> <path d="M14.854 11.321C14.7568 11.2282 14.6388 11.1818 14.4998 11.1818H14.3333V10.2272C14.3333 9.61741 14.1041 9.09378 13.6458 8.65628C13.1875 8.21876 12.639 8 12 8C11.361 8 10.8124 8.21876 10.3541 8.65626C9.89574 9.09378 9.66663 9.61739 9.66663 10.2272V11.1818H9.49999C9.36115 11.1818 9.24306 11.2282 9.14583 11.321C9.0486 11.4138 9 11.5265 9 11.6591V14.5227C9 14.6553 9.04862 14.768 9.14583 14.8609C9.24306 14.9536 9.36115 15 9.49999 15H14.5C14.6389 15 14.7569 14.9536 14.8542 14.8609C14.9513 14.768 15 14.6553 15 14.5227V11.6591C15.0001 11.5265 14.9513 11.4138 14.854 11.321ZM13.3333 11.1818H10.6666V10.2272C10.6666 9.87594 10.7969 9.57597 11.0573 9.32743C11.3177 9.07886 11.6319 8.9546 12 8.9546C12.3681 8.9546 12.6823 9.07884 12.9427 9.32743C13.2031 9.57595 13.3333 9.87594 13.3333 10.2272V11.1818Z" fill="black"></path> </svg> </span> </a>';

    return editActivityLink + changePassword;
}


function GetUserByID(id) {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: getUserByIdUrl + "?id=" + id,
        success: function (result) {
            $("#EditUserId").val(result.data.id);
            $("#UserName").val(result.data.firstName + " " + result.data.lastName);
            $("#UserEmail").val(result.data.email);
            $("#UserRoleTypeList").val(result.data.roleId).change();
            $("#isDisabled").prop("checked", result.data.isDisabled);
        },
        error: function (result) {
        }
    });
}


function SetId(id, firstName, lastName, email) {
    $("#ChangePasswordUserName").val(firstName + " " + lastName);
    $("#ChangePasswordUserEmail").val(email);
    $("#EditPasswordUserId").val(id);
}

function DeleteActivityByID(id) {
    swal.fire({
        title: "Are you sure?",
        text: "Are you sure you want to delete this record?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#009ef7",
        confirmButtonText: "Continue",
        cancelButtonText: "Cancel",
        closeOnConfirm: true
    }).then(function (isConfirm) {
        if (isConfirm) {
            if (isConfirm.isConfirmed) {
                var data = { activityId: id };
                $.ajax({
                    type: "POST",
                    url: deleteActivityByIdUrl,
                    data: data,
                    success: function (result) {
                        if (result.status) {
                            Swal.fire(
                                {
                                    title: 'Success!',
                                    text: 'Record deleted successfully!',
                                    icon: 'success',
                                    confirmButtonColor: "#009ef7",
                                }
                            )
                            GetUsers();
                        }
                        else {
                            Swal.fire(
                                {
                                    title: 'Error!',
                                    text: 'Something went wrong!',
                                    icon: 'error',
                                    confirmButtonColor: "#009ef7",
                                }
                            )
                        }
                    },
                    error: function (result) {
                    }
                });
            }
            else {
                return false;
            }
        } else {
            return false;
        }
    });

}