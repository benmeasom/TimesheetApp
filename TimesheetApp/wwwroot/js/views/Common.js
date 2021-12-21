
function ActivateTab(menu)
{
    if (menu == 'Masters') {
        $("#navTimesheets").removeClass('active show');
        $("#navMasters").addClass('active show');

    }

    if (menu == 'Timesheet') {
        $("#navTimesheets").addClass('active show');
        $("#navMasters").removeClass('active show');

    }
}


function RefreshPage() {
    window.location.href = window.location.href;
}


