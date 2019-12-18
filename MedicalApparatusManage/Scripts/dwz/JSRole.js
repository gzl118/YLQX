var roleTable;
var roRedraw = false;
var roInitVals = new Array();
var oEwDCount = 0;
function ROgetData(SortCol, SortKey) {
    role_tableSort(SortCol, SortKey);
    $("#RoleTable tbody").click(function (event) {
        $(roleTable.fnSettings().aoData).each(function () {
            $(this.nTr).removeClass('row_selected');
        });
        $(event.target.parentNode).addClass('row_selected');
    });

    

    $("#RoleTable thead th").click(function () {

        if ($(this).attr("col") != "") {
            $("#SortCol_OP").val($(this).attr("col"));
            if ($(this).attr("sort") == "DESC" && $("#SortKey_EW").val() == "DESC") {
                $("#SortKey_OP").val("ASC");
                $(this).attr("sort", "ASC");
                $(this).addClass("tablesorting_asc");
                $(this).removeClass("tablesorting");
            } else {
                $("#SortKey_OP").val("DESC");
                $(this).attr("sort", "DESC");
                $(this).addClass("tablesorting_desc");
                $(this).removeClass("tablesorting");
            }
            navTabPageBreak();
        }
    });

    roleTable = $('#RoleTable').dataTable({
        "bProcessing": true,
        "bServerSide": false,
        "sScrollY": screen.height - 470,
        "sScrollX": "100%",
        "sScrollXInner": "100%",
        "bLengthChange": false,

        "bJQueryUI": true,
        "sPaginationType": "full_numbers",
        "bPaginate": false,
        "bFilter": false,
        "bInfo": false,
        //        "aoColumnDefs": [
        //						{ "bSearchable": false, "bVisible": false, "aTargets": [2] }
        //					],
        "aaSorting": [[0, "desc"]]

    });
}

function role_tableSort(str, sd) {
    //alert(str +"  "+ sd);
    $("#RoleTable thead th").each(function () {
        if ($(this).attr("col") != "") {
            $(this).addClass("tablesorting");
            if (str != "" && $(this).attr("col") == str) {
                if (sd == "DESC") {
                    $(this).removeClass("tablesorting");
                    $(this).addClass("tablesorting_desc");
                    $(this).attr("sort", "DESC");
                } else if (sd == "ASC") {
                    $(this).removeClass("tablesorting");
                    $(this).addClass("tablesorting_asc");
                    $(this).attr("sort", "ASC");
                }
            }
        }
    });
}


function roGetEngSelected(oTableLocal) {
    var aReturn = new Array();
    oEwDCount = 0;
    var aTrs = oTableLocal.fnGetNodes();
    for (var i = 0; i < aTrs.length; i++) {
        if ($(aTrs[i]).hasClass('row_selected')) {
            oEwDCount++;
            aReturn.push(aTrs[i]);
        }
    }
    return aReturn;
}


function RoleAdd() {
    var diag = new Dialog();
    diag.Title = "添加";
    diag.Width = 600;
    diag.Name = 'ei_diagadd';
    diag.Height = 250;
    diag.ID = "ei_diagadd";
    diag.URL = webmainPath + "Role/Add";
    diag.show();
}
