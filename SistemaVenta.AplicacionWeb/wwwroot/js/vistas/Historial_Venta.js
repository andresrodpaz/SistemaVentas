const vistaBusqueda = {
    busquedaFecha: () => {
        $('#txtFechaInicio').val("")
        $('#txtFechaFin').val("")
        $('#txtNumeroVenta').val("")

        $('.busqueda-fecha').show();
        $('.busqueda-venta').hide();
    }, busquedaVenta: () => {
        $('#txtFechaInicio').val("")
        $('#txtFechaFin').val("")
        $('#txtNumeroVenta').val("")

        $('.busqueda-fecha').hide();
        $('.busqueda-venta').show();
        
    }
}

$(document).ready(function () {
    vistaBusqueda["busquedaFecha"]()

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $('#txtFechaInicio').datepicker({ dateFormat: "dd/mm/yy" });
    $('#txtFechaFin').datepicker({ dateFormat: "dd/mm/yy" });
})

$("#cboBuscarPor").change(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        vistaBusqueda["busquedaFecha"]();
    } else {
        vistaBusqueda["busquedaVenta"]();
    }
})

$("#btnBuscar").click(function () {

    if ($("#cboBuscarPor").val() == "fecha") {
        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debes ingresar una fecha de inicio y de fin");
            return;
        }
    } else {
        if ($("#txtNumeroVenta").val().trim() == "") {
            toastr.warning("", "Debes ingresar un numero de venta");
            return;
        }
    }

    let numeroVenta = $('#txtNumeroVenta').val();
    let fechaInicio = $('#txtFechaInicio').val();
    let fechaFin = $('#txtFechaFin').val();

    $('.card-body').find("div.row").LoadingOverlay("show");

    fetch(`/Venta/Historial?numeroVenta=${numeroVenta}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`)
        .then(response => {
            $('.card-body').find("div.row").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJSON => {

            console.log(responseJSON);
          
            $("#tbventa tbody").html("");

            if (responseJSON.length > 0) {
                responseJSON.forEach((venta) => {
                    $("#tbventa tbody").append(
                        $("<tr>").append(
                            $("<td>").text(venta.fechaRegistro),
                            $("<td>").text(venta.numeroVenta),
                            $("<td>").text(venta.tipoDocumentoVenta),
                            $("<td>").text(venta.documentoCliente),
                            $("<td>").text(venta.nombreCliente),
                            $("<td>").text(venta.total),
                            $("<td>").append(
                                $("<button>").addClass("btn btn-info btn-sm").append(
                                    $("<i>").addClass("fas fa-eye")
                                ).data("venta",venta)
                            )
                    ))
                })
            }
        })
        .catch(error => {
            console.error("Error en la solicitud:", error);
            // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
        });
})

$("#tbventa tbody").on("click", ".btn-info", function () {
    let d = $(this).data("venta");

    $("#txtFechaRegistro").val(d.fechaRegistro);
    $("#txtNumVenta").val(d.numeroVenta);
    $("#txtUsuarioRegistro").val(d.usuario);
    $("#txtTipoDocumento").val(d.tipoDocumentoVenta);
    $("#txtDocumentoCliente").val(d.documentoCliente);
    $("#txtNombreCliente").val(d.nombreCliente);
    $("#txtSubTotal").val(d.subTotal);
    $("#txtIGV").val(d.impuestoTotal);
    $("#txtTotal").val(d.total);

    $("#tbProductos tbody").html("");

    d.detalleVenta.forEach((item) => {
        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total),
            ))
    })
    $("#linkImprimir").attr("href", `/Venta/MostrarPDFVenta?numeroVenta=${d.numeroVenta}`);
    $("#modalData").modal("show");
})

