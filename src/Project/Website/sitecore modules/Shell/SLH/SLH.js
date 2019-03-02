

var categories = [];
var tableData;
var currentBucketId;
var currentTableId;

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

var contentFormatter = function (cell, formatterParams) {

    if (!cell) {
        return "";
    }

    var data = cell.getData();
    
    if (!data) {
        return "";
    }

    var title = cell.getColumn().getDefinition().title;

    if (!title) {
        return "";
    }

    var index = data.Fields.findIndex(item => item.FieldName === title)

    return data.Fields[index].FieldValue;
}

function PreviewItem(id) {

	var url = "/sitecore modules/Shell/SLH/Preview.aspx?id=%7B" + id + "%7D&la=en&language=en&vs=1&version=1&database=master&readonly=0&db=master";
	
	$("#preview-item-dialog").dialog("open");
	$("#preview-item-content").html("<iframe src='" + url + "' class='edit-frame'></iframe>");
}

function OpenContentEditor(url, tableId) {

    $("#table-id").val(tableId);
    
    top._close = function () {
        $("#edit-item-dialog").dialog("close");
    };

    $("#edit-item-dialog").dialog("open");
    $("#edit-item-content").html("<iframe src='" + url + "' class='edit-frame'></iframe>");
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

function RefreshTableData(tableId, categoryId, articleType) {
    $.get("/slh_api/buckets/GetBucketableItems?bucketid=" + articleType + "&category=" + categoryId, function (bucketData) {
        $(currentTableId).tabulator("setData", bucketData);
    });
}

function SetupTable(tableId, categoryId, articleType) {
    currentBucketId = articleType;
    currentTableId = tableId;
    $.get("/slh_api/buckets/GetBucketableItems?bucketid=" + articleType + "&category=" + categoryId, function (bucketData) {
        
	    var allowSorting = false;

        if (!bucketData || bucketData.length === 0) {
            alert("No items found.");
            return;
        }

        var fields = bucketData[0].Fields;
        var columnDetails = [
            {
                title: "Item Name",
                field: "ItemName",
            }
        ];

        for (var i = 0; i < fields.length; i++) {
            var column = {
                title: fields[i].FieldName,
                formatter: contentFormatter
                //field: "LogoUrl", headerSort: false, align: "center", formatter: imageFormatter, width: 100
            };

            columnDetails.push(column);
        }

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
                { title: "Actions", field: "", headerSort: false, width: 160, formatter: function (cell, formatterParams) { return GetOpenContentEditor(cell, tableId) + " " + GetPublishControl(cell, tableId); } }
            ]);
        }

        $(tableId).tabulator({
            data: bucketData,
            pagination: "local",
            paginationSize: 10,
            height: "90%",
            layout: "fitColumns",
            placeholder: "No Data Set",
            movableRows: allowSorting, //enable user movable rows
            index: "Id",
            columns: columnDetails,
            /*rowFormatter: function (row) {
                row.getElement().css({ "height": "30px" });
            },*/
        });

    });
}

function GetPublishControl(cell, tableId) {
    return "<a class='open-category actions' href='javascript:PublishItem(\"" + cell.getRow().getData().ItemId + "\", \"" + tableId + "\")'>Publish <img width='16' height='16' src='/sitecore modules/Shell/SLH/publish.png' alt='Publish'/></a>";
}

function GetOpenContentEditor(cell, tableId) {
    return "<a class='open-category actions' href='javascript:OpenContentEditor(\"" + cell.getRow().getData().Url + "\", \"" + tableId + "\")'>Edit <img width='16' height='16' src='/sitecore modules/Shell/SLH/edit.png' alt='Edit'/></a>";
}


$(function () {

    $(".reload-btn").click(function () {
        var tableId = $("#table-id").val();
        RefreshTableData(tableId, "", currentBucketId);
    });

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
            RefreshTableData(tableId, "", currentBucketId);
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
				$("#publish-message").html("Publishing item...");
				$("#refresh-table").val("false");
				
				var id = $("#item-id").val();

                $.post("/slh_api/buckets/PublishItem", { itemId: id }, function (data) {
					if (data == true) {
						$("#refresh-table").val("true");
						var messageDialogOpen = $("#dialog-publish-message").dialog('isOpen');
						if(!messageDialogOpen){
							var tableId = $("#table-id").val();
                            RefreshTableData(tableId, "", currentBucketId);
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
                RefreshTableData(tableId, "", currentBucketId);
				$("#refresh-table").val("false");
			}
		}
	});
});
