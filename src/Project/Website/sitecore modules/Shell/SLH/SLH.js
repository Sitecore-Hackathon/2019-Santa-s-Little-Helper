

var categories = [];
var tableData;

$.get("/slh_api/buckets/GetBuckets", function (data) {
    categories = data;
});

var imageFormatter = function (cell, formatterParams) {
    var value = this.sanitizeHTML(cell.getValue());
    var img = $("<img src='" + value + "'/>");

    cell.getElement().css({ "background-color": "rgb(97, 96, 96)" });
    img.css({ width: "100%" });

    img.on("load",
        function () {
            cell.getRow().normalizeHeight();
        });

    return img;
}

function PreviewItem(id) {

	var url = "/sitecore modules/Shell/SLH/Preview.aspx?id=%7B" + id + "%7D&la=en&language=en&vs=1&version=1&database=master&readonly=0&db=master";
	
	$("#preview-item-dialog").dialog("open");
	$("#preview-item-content").html("<iframe src='" + url + "' class='edit-frame'></iframe>");
}

function OpenContentEditor(id, tableId) {

    $("#table-id").val(tableId);

    $.get("/slh_api/buckets/editor?id=" + id, function (url) {

        top._close = function () {
            $("#edit-item-dialog").dialog("close");
        };

        $("#edit-item-dialog").dialog("open");
        $("#edit-item-content").html("<iframe src='" + url + "' class='edit-frame'></iframe>");
    });
}

function PublishItem(id, tableId) {

    $("#item-id").val(id);
    $("#table-id").val(tableId);

    $("#dialog-publish-confirm").dialog("open");
}

function OpenTab(tabId) {
    $(tabId).click();
    return false;
}

function SetupTable(tableId, categoryId, articleType) {

    $.get("/slh_api/buckets/GetBucketableItems?bucketid=" + articleType + "&category=" + categoryId, function (bucketData) {
        
	    var allowSorting = false;
        var pagination = "local";

        if (!bucketData || bucketData.length === 0) {
            alert("No items found.");
            return;
        }

        var fields = bucketData[0].Fields;
        var columnDetails = [];

        for (var i = 0; i < fields.length; i++) {
            var column = {
                title: fields[i].FieldName,
                //field: "LogoUrl", headerSort: false, align: "center", formatter: imageFormatter, width: 100
            };

            columnDetails.push(column);
        }
        

        /*var columnDetails = [
            { title: "Source", field: "LogoUrl", headerSort: false, align: "center", formatter: imageFormatter, width: 100 },
            { title: "Title", field: "Title", headerSort: false, width: 330, formatter: "textarea", cellClick: function (e, cell) { window.open(cell.getRow().getData().Url, '_blank'); } },
            { title: "Url", field: "Url", headerSort: false, width: 70, formatter: function (cell, formatterParams) { return (cell.getValue()) ? "<span class='open-category'>Open</span>" : "None" }, cellClick: function (e, cell) { if (cell.getValue()) { window.open(cell.getValue(), "_blank"); } } },
            //{ title: "Categories", field: "Categories", headerSort: false, formatter: function (cell, formatterParams) { cell.getElement().css({ "white-space": "normal" }); return cell.getValue().map(x=>"<a class='open-category' href='javascript:OpenTab(\"#" + articleType + "tabs-" + ((categories[x]) ? categories[x].toLowerCase() : "") + "\")'>" + categories[x] + "</a>").join(", "); } },
            { title: "Created Date", field: "CreatedDate", align: "center", headerSort: false, width: 90 }
        ];*/

        if (articleType === "active") {
		
		    pagination = null;
		
            columnDetails = columnDetails.concat([
                { title: "Publish Restrictions", field: "PublishRestrictions", formatter: "html", headerSort: false, width: 120 },
                { title: "Published", field: "Published", formatter: "tickCross", align: "center", headerSort: false, width: 70 },
                { title: "Errors", field: "Errors", formatter: "tickCross", align: "center", headerSort: false, width: 50 },
                { title: "Actions", field: "", width: 180, headerSort: false, formatter: function (cell, formatterParams) { return GetPreviewControl(cell) + " " + GetOpenContentEditor(cell, tableId) + "<br/>" + GetPublishControl(cell, tableId); } }
            ]);
		
		    if(categoryId == ""){
			    columnDetails.unshift({ rowHandle: true, formatter: "handle", headerSort: false, frozen: true, width: 30, minWidth: 30 });
		    }
        }
        else {
            columnDetails = columnDetails.concat([
                { title: "Actions", field: "", headerSort: false, formatter: function (cell, formatterParams) { return GetOpenContentEditor(cell, tableId) + " " + GetPublishControl(cell, tableId); } }
            ]);
        }

        $(tableId).tabulator({
            data: bucketData,
		    pagination: pagination,
            height: "500px",
            layout: "fitColumns",
            placeholder: "No Data Set",
            movableRows: allowSorting, //enable user movable rows
            index: "Id",
            columns: columnDetails,
            rowFormatter: function (row) {
                row.getElement().css({ "height": "50px" });
            },
            //ajaxResponse: function (url, params, response) {
                // store original table data
            //    tableData = response;
            //    return response;
            //},
        });

        //$(tableId).tabulator("setData", "/slh_api/buckets/GetBucketableItems?bucketid=" + articleType + "&category=" + categoryId);

    });
}

function GetPublishControl(cell, tableId) {
    return "<a class='open-category actions' href='javascript:PublishItem(\"" + cell.getRow().getData().Id + "\", \"" + tableId + "\")'>Publish <img width='16' height='16' src='/temp/iconcache/office/24x24/publish.png' alt='Publish'/></a>";
}

function GetOpenContentEditor(cell, tableId) {
    return "<a class='open-category actions' href='javascript:OpenContentEditor(\"" + cell.getRow().getData().BucketId + "\", \"" + tableId + "\")'>Edit <img width='16' height='16' src='/temp/iconcache/applications/16x16/edit.png' alt='Edit'/></a>";
}

function GetPreviewControl(cell) {
    return "<a class='open-category actions' href='javascript:PreviewItem(\"" + cell.getRow().getData().Id + "\")'>Preview <img width='16' height='16' src='/temp/iconcache/applications/16x16/document_view.png' alt='Preview'/></a>";
}


$(function () {

	$("#tabs").tabs({
		beforeLoad: function (event, ui) {
			ui.jqXHR.fail(function () {
				ui.panel.html(
					"Couldn't load this tab. We'll try to fix this as soon as possible. ");
			});
		}
	});

	$("#edit-item-dialog").dialog({
		autoOpen: false,
		dialogClass: "edit-item",
		height: 600,
		width: 800,
		modal: true,
		buttons: {},
		close: function () {
			$("#edit-item-content").html("");
			top._close = top.close;
			var tableId = $("#table-id").val();
			$(tableId).tabulator("setData");
		}
	});
	
	$("#preview-item-dialog").dialog({
		autoOpen: false,
		height: 600,
		width: 1290,
		modal: true,
		buttons: {},
		close: function () {
			$("#preview-item-content").html("");
		}
	});

	$("#dialog-publish-confirm").dialog({
		resizable: false,
		autoOpen: false,
		height: "auto",
		width: 400,
		modal: true,
		buttons: {
			"Publish": function () {
				$("#publish-message").html("Publishing article...");
				$("#refresh-table").val("false");
				
				var id = $("#item-id").val();

                $.post("/slh_api/buckets/PublishItem", { id: id }, function (data) {
					if (data == true) {
						$("#refresh-table").val("true");
						var messageDialogOpen = $("#dialog-publish-message").dialog('isOpen');
						if(!messageDialogOpen){
							var tableId = $("#table-id").val();
							$(tableId).tabulator("setData");
						}
					}
					else {
						$("#publish-message").html("Error: " + data);
						var messageDialogOpen = $("#dialog-publish-message").dialog('isOpen');
						if(!messageDialogOpen){
							$("#dialog-publish-message").dialog( "option", "title", "Error" );
							$("#dialog-publish-message").dialog("open");
						}
					}
				});
				$(this).dialog("close");
				$("#dialog-publish-message").dialog( "option", "title", "Publish" );
				$("#dialog-publish-message").dialog("open");
			},
			Cancel: function () {
				$(this).dialog("close");
			}
		}
	});
	
	$("#dialog-publish-message").dialog({
		resizable: false,
		autoOpen: false,
		height: "auto",
		width: 400,
		modal: true,
		buttons: {
			Ok: function () {
				$(this).dialog("close");
			}
		},
		close: function() {
			if($("#refresh-table").val() == "true"){
				var tableId = $("#table-id").val();
				$(tableId).tabulator("setData");
				$("#refresh-table").val("false");
			}
		}
	});
});
