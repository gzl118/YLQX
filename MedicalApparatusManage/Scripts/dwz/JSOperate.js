var oewTable;
var giRedraw = false;
var asInitVals = new Array();
var sEwDCount = 0;
function EWgetData(SortCol, SortKey) {
    eng_tableSort(SortCol, SortKey);
    $("#example tbody").click(function (event) {
        $(oewTable.fnSettings().aoData).each(function () {
            $(this.nTr).removeClass('row_selected');
        });
        $(event.target.parentNode).addClass('row_selected');
    });

    $("#example tbody tr").dblclick(function (event) {
        showOperateLog();
    });

    $("#example thead th").click(function () {

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

    oewTable = $('#example').dataTable({
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

function eng_tableSort(str, sd) {
    //alert(str +"  "+ sd);
    $("#example thead th").each(function () {
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


function fnGetEngSelected(oTableLocal) {
    var aReturn = new Array();
    sEwDCount = 0;
    var aTrs = oTableLocal.fnGetNodes();
    for (var i = 0; i < aTrs.length; i++) {
        if ($(aTrs[i]).hasClass('row_selected')) {
            sEwDCount++;
            aReturn.push(aTrs[i]);
        }
    }
    return aReturn;
}

function OpRemove() {
//    var anSelected = fnGetEngSelected(oewTable);
//    var rowData = anSelected[0];
//    var RegionCode = rowData.cells[0].innerText;
    var diag = new Dialog();
    diag.Title = "删除";
    diag.Width = 300;
    diag.Name = 'ei_diagdel';
    diag.Height = 150;
    diag.ID = "ei_diagdel";
    diag.URL = webmainPath + "OperateLog/DeleteAll/" + true;
    diag.show();
}

function showOperateLog() {
    var anSelected = fnGetEngSelected(oewTable);
    
    if (sEwDCount == 0) { Dialog.alert("请选择要操作的行！"); return false; }
    var rowData = anSelected[0];
    var RegionCode = rowData.cells[0].innerText;
    var diag = new Dialog();
    diag.Title = "查看";
    diag.Width = 800;
    diag.Name = 'pi_diagshow';
    diag.Height = 540;
    diag.ID = "pi_diagshow";
    diag.URL = webmainPath + "OperateLog/Detail/" + RegionCode;
    diag.show();

}
