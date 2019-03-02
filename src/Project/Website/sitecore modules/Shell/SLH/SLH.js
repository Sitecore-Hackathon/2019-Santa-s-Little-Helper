
$(document).ready(function () {
    $("#article-table").tabulator({
        columns: [
            { title: "", field: "cheese", sorter: "boolean", align: "center", formatter: "tickCross" },
            { title: "Title", field: "name", sorter: "string", width: 200, editor: true },
            { title: "Source", field: "age", sorter: "number", align: "right", formatter: "progress" },
            { title: "Categories", field: "gender", sorter: "string", cellClick: function (e, cell) { console.log("cell click") }, },
            { title: "Date", field: "height", formatter: "star", align: "center", width: 100 },
            { title: "Published Date", field: "col", sorter: "string" },
            { title: "Actions", field: "dob", sorter: "date", align: "center" }
        ],
    });
});
