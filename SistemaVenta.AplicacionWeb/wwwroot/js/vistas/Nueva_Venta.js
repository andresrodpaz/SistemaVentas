let valorImpuesto = 0; 

$(document).ready(function () {

    fetch("/Venta/ListaTipoDocumentoVenta")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJSON => {
            if (responseJSON.length > 0) {
                responseJSON.forEach((item) => {
                    $("#cboTipoDocumentoVenta").append(
                        $("<option>").val(item.idTipoDocumentoVenta).text(item.descripcion)
                    );
                });
            } else {
                console.log("No se encontraron tipos de documentos.");
            }
        })
        .catch(error => {
            console.error("Error en la solicitud:", error);
            // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
        });

    fetch("/Negocio/Obtener")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJSON => {
            if (responseJSON.estado === true) {
                const d = responseJSON.objeto;
                console.log(d);

                //Ajustamos el texto segun la moneda que use el negocio
                $('#inputGroupSubTotal').text(`Sub Total - ${d.simboloMoneda}`);
                //Ajustamos el texto segun la porcentaje de impuesto que use el negocio
                $('#inputGroupIGV').text(`IGV(${d.porcentajeImpuesto}%)- ${d.simboloMoneda}`);
                //Ajustamos el texto segun la moneda que use el negocio
                $('#inputGroupTotal').text(`Total - ${d.simboloMoneda}`);

                valorImpuesto = parseFloat(d.porcentajeImpuesto);
            } else {
                console.log("No se encontraron datos del negocio o el estado no es true.");
            }
        })
        .catch(error => {
            console.error("Error en la solicitud:", error);
            // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
        });

    $('#cboBuscarProducto').select2({
        ajax: {
            url: "/Venta/ObtenerProductos",
            dataType: 'json',
            contentType:"application/json; charset=utf-8",
            delay: 250,
            data: function (params) {
                return {
                    busqueda: params.term
                };
            },
            processResults: function (data) {
                // parse the results into the format expected by Select2
                // since we are using custom formatting functions we do not need to
                // alter the remote JSON data, except to indicate that infinite
                // scrolling can be used

                return {
                    results: data.map((item) => (
                        {
                            id: item.idProducto,
                            text: item.descripcion,

                            marca: item.marca,
                            categoria: item.nombreCategoria,
                            urlImagen: item.urlImagen,
                            precio: parseFloat(item.precio)
                        }
                    ))
                    
                };
            },
        },
        language:"es",
        placeholder: 'Buscar producto...',
        minimumInputLength: 1,
        templateResult: formatoResultado
    });
});

function formatoResultado(data) {
    console.log("Datos recibidos para la busqueda: ", data);
    if (data.loading) {
        return data.text;
    }
    var contenedor = $(
        `
        <table width="100%">
            <tr>
                <td style="width:60px">
                    <img style="height:60px;width:60px;margin-right:10px" src="${data.urlImagen}" alt="Imagen del Producto"/>
                </td>
                <td>
                    <p style="font-weight:bolder;margin:2px">${data.marca}</p>
                    <p style="margin:2px">${data.text}</p>
                </td>
            </tr>
        </table>
        `
    )
    return contenedor;

}

$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})
let ProductosParaVenta = [];


$('#cboBuscarProducto').on("select2:select", function (e) {
    const data = e.params.data;

    console.log(data);

    let producto_encontrado = ProductosParaVenta.filter(p => p.idProducto == data.id);
    if (producto_encontrado.length > 0) {
        $('#cboBuscarProducto').val("").trigger("change");
        toastr.warning("", "El producto ya fue agregado");
        return false;
    }
    swal({
        title: data.marca,
        text: data.text,
        imageUrl: data.urlImagen,
        type:"input",
        showCancelButton: true,
        closeOnConfirm: false,
        inputPlaceholder:"Ingrese cantidad"
    },
        function (valor) {
            if (valor === false) {
                return false;
            }
            //Si no es un numero
            if (isNaN(parseInt(valor))) {
                toastr.warning("", "Debes Ingresar un valor numerico!");
                return false;
            }
            //Si es vacío
            if (valor==="") {
                toastr.warning("", "Debes Ingresar la cantidad!");
                return false;
            }
            let producto = {
                idProducto: data.id,
                marcaProducto: data.marca,
                descripcionProducto: data.text,
                categoriaProducto: data.categoria,
                cantidad: parseInt(valor),
                precio: data.precio.toString(),
                total: (parseFloat(valor) * data.precio).toString()

            }
            
            ProductosParaVenta.push(producto);
            mostrarProductos_Precios();
            $('#cboBuscarProducto').val("").trigger("change");
            swal.close();
        }
        
    )
})

function mostrarProductos_Precios() {
    let total = 0;
    let impuesto = 0;
    let subtotal = 0;
    let porcentaje = valorImpuesto / 100;

    $('#tbProducto tbody').html("");
    ProductosParaVenta.forEach((item) => {

        total = total + parseFloat(item.total);

        $('#tbProducto tbody').append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idProducto", item.idProducto)
                ),
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )
    })

    subtotal = total / (1 + porcentaje);
    impuesto = total - subtotal;

    $('#txtSubTotal').val(subtotal.toFixed(2));
    $('#txtIGV').val(impuesto.toFixed(2));
    $('#txtTotal').val(total.toFixed(2));
}
$(document).on("click", "button.btn-eliminar", function () {
    const _idproducto = $(this).data("idProducto");
    ProductosParaVenta = ProductosParaVenta.filter(producto => producto.idProducto != _idproducto);

    mostrarProductos_Precios();
});

$("#btnTerminarVenta").click(function () {
    if (ProductosParaVenta.length < 1) {
        toastr.warning("", "Debes añadir productos");
        return;
    }

    const tipoDocumento = $("#cboTipoDocumento").val();
    const numeroDocumento = $("#txtDocumentoCliente").val().trim();
    console.log("Tipo de Documento Seleccionado: ", tipoDocumento);

    // Validación del formato del número de documento según el tipo seleccionado
    if (tipoDocumento === "DNI" && !/^\d{8}[A-Z]$/.test(numeroDocumento)) {
        const mensaje = "Por favor ingresa un DNI válido (8 dígitos numéricos + letra).";
        toastr.warning("", mensaje);
        $("#txtDocumentoCliente").focus();
        return;
    } else if (tipoDocumento === "NIE" && !/^[XYZ]\d{7}[A-Z]$/.test(numeroDocumento)) {
        const mensaje = "Por favor ingresa un NIE válido (letra + 7 dígitos numéricos + letra).";
        toastr.warning("", mensaje);
        $("#txtDocumentoCliente").focus();
        return;
    } else if (tipoDocumento === "Pasaporte" && !/^.{1,20}$/.test(numeroDocumento)) {
        const mensaje = "Por favor ingresa un Pasaporte válido (hasta 20 caracteres).";
        toastr.warning("", mensaje);
        $("#txtDocumentoCliente").focus();
        return;
    }


    const vmDetalleVenta = ProductosParaVenta;

    const venta = {
        idTipoDocumentoVenta: $("#cboTipoDocumentoVenta").val(),
        documentoCliente: $("#txtDocumentoCliente").val(),
        nombreCliente: $("#txtNombreCliente").val(),
        subTotal: $("#txtSubTotal").val(),
        impuestoTotal: $("#txtIGV").val(),
        total: $("#txtTotal").val(),
        detalleVenta: vmDetalleVenta
    }

    $('#btnTerminarVenta').LoadingOverlay("show");

    fetch("/Venta/RegistrarVenta", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(venta)
    })
        .then(response => {
            $('#btnTerminarVenta').LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJSON => {
            if (responseJSON.estado === true) {
                console.log(responseJSON);
                ProductosParaVenta = [];
                mostrarProductos_Precios();

                $("#txtNombreCliente").val("");
                $("#txtDocumentoCliente").val("");
                $("cboTipoDocumentoVenta").val($("#cboTipoDocumentoVenta option:first").val());

                swal("Registrado!", `Numero de Venta: ${responseJSON.objeto.numeroVenta.toString()}`, "success");

            } else {
                swal("Lo Sentimos!", "No se ha podido registrar la venta", "error");
                console.log("No se encontraron tipos de documentos.");
            }
        })
        .catch(error => {
            console.error("Error en la solicitud:", error);
            // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
        });
})