let tablaData;

$(document).ready(function () {

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $('#txtFechaInicio').datepicker({ dateFormat: "dd/mm/yy" });
    $('#txtFechaFin').datepicker({ dateFormat: "dd/mm/yy" });

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Reporte/ReporteVenta?fechaInicio=01/01/1999&fechaFin=01/01/1999',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "fechaRegistro" },
            { "data": "numeroVenta" },
            { "data": "tipoDocumento" },
            
            { "data": "documentoCliente" },
            { "data": "nombreCliente" },
            { "data": "subTotalVenta" },
            { "data": "impuestoTotalVenta" },
            { "data": "totalVenta" },
            { "data": "producto" },
            { "data": "cantidad" },
            { "data": "precio" },
            { "data": "total" },
            
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        initComplete: function () {
            this.api().buttons().container().appendTo($('#tbdata_wrapper .col-md-6:eq(0)'));
        },
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Ventas',
            }
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });

})

$('#btnBuscar').click(function () {
    if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
        toastr.warning("", "Debes ingresar una fecha de inicio y de fin");
        return;
    }

    let fechaInicio = $("#txtFechaInicio").val();
    let fechaFin = $("#txtFechaFin").val();

    let nueva_url = `/Reporte/ReporteVenta?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`;

    tablaData.ajax.url(nueva_url).load();
})