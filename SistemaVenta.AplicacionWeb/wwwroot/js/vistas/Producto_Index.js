const modeloBase = {
    idProducto: 0,
    codigoBarra: "",
    marca: "",
    descripcion: "",
    idCategoria: 0,
    stock: 0,
    idRol: 0,
    urlImagen: "",
    precio:0,
    esActivo: 1,
    
}

let tablaData;

/*
    ('#tbdata').DataTable --> #tbdata es el id de la tabla en el index y lo convertimos al tipo DataTable
    responsive: true --> es responsive
*/

$(document).ready(function () {

    fetch("/Categoria/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJSON => {
            if (responseJSON.data.length > 0) {
                responseJSON.data.forEach((item) => {
                    $("#cboCategoria").append(
                        $("<option>").val(item.idCategoria).text(item.descripcion)
                    );
                });
            } else {
                console.log("No se encontraron categorias.");  // Puedes agregar un mensaje de registro o manejo aquí
            }
        })
        .catch(error => {
            console.error("Error en la solicitud:", error);
            // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
        });


    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Producto/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idProducto", "visible": false, "searchable": false },
            {
                "data": "urlImagen", render: function (data) {
                    console.log("URL de la imagen:", data);
                    //return '<img style="height:60px"" src=${data} class="rounded mx-auto d-block"/>' retorna simbolo de imagen missing pero da error de consola por las comillas
                    return `<img style="height:60px" src="${data}" class="rounded mx-auto d-block"/>`;

                }
            },
            { "data": "codigoBarra" },
            { "data": "marca" },
            { "data": "descripcion" },
            { "data": "nombreCategoria" },
            { "data": "stock" },
            { "data": "precio" },
            {
                "data": "esActivo", render: function (data) {
                    if (data == 1) {
                        return '<span class="badge badge-info">Activo</span>';
                    } else {
                        return '<span class="badge badge-danger">No Activo</span>';
                    }
                }

            },


            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Productos',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
});

function mostrarModal(modelo = modeloBase) {
    $('#txtId').val(modelo.idProducto);
    $('#txtCodigoBarra').val(modelo.codigoBarra);
    $('#txtMarca').val(modelo.marca);
    $('#txtDescripcion').val(modelo.descripcion);
    $('#cboCategoria').val(modelo.idCategoria === 0 ? $('#cboCategoria option:first').val() : modelo.idCategoria);
    $('#txtStock').val(modelo.stock);
    $('#txtPrecio').val(modelo.precio);
    $('#cboEstado').val(modelo.esActivo);
    $('#txtImagen').val("");
    $('#imgProducto').attr("src", modelo.urlImagen);

    $('#modalData').modal("show");
}

$("#btnNuevo").click(
    function () {
        mostrarModal()
    }
)

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter((item) => item.value.trim() == "");

    if (inputs_vacios.length > 0) {
        const mensaje = `Por favor completar el campo: ${inputs_vacios[0].name}`;
        toastr.warning("", mensaje);
        $(`input[name=${inputs_vacios[0].name}]`).focus();
        return; // Agregamos esta línea para salir de la función si hay campos vacíos
    }

    const idProductoInput = $("#txtId").val();
    const idProducto = idProductoInput ? parseInt(idProductoInput.trim()) : 0;
    console.log(idProducto);


    const modelo = {
        idProducto: idProducto,
        codigoBarra: $('#txtCodigoBarra').val(),
        marca: $('#txtMarca').val(),
        descripcion: $('#txtDescripcion').val(),
        idCategoria: $('#cboCategoria').val(),
        stock: $('#txtStock').val(),
        precio: $('#txtPrecio').val(),
        esActivo: $('#cboEstado').val(),
    };

    console.log("Modelo:", modelo);

    const inputFoto = document.getElementById("txtImagen");

    if (inputFoto.files.length > 0) {
        const file = inputFoto.files[0];
        console.log("Archivo seleccionado:", file);

        const formData = new FormData();
        formData.append("imagen", file);
        formData.append("modelo", JSON.stringify(modelo));

        console.log("FormData:", formData);

        $('#modalData').find("div.modal-content").LoadingOverlay("show");

        if (modelo.idProducto == 0) {
            //console.log("Entró en el bloque if (modelo.idUsuario == 0)");

            fetch("/Producto/Crear", {
                method: "POST",
                body: formData,
            })
                .then(response => {
                    $('#modalData').find("div.modal-content").LoadingOverlay('hide');
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    console.log("Respuesta del servidor:", responseJson);

                    if (responseJson.estado) {
                        tablaData.row.add(responseJson.objeto).draw(false);
                        $('#modalData').modal('hide');
                        swal("Listo!", "Producto añadido con éxito", "success");
                    } else {
                        swal("Error!", responseJson.mensaje, "error");
                    }
                })
                .catch(error => {
                    console.error("Error en la solicitud:", error);
                    // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
                });
        } else {
            fetch("/Producto/Editar", {
                method: "PUT",
                body: formData,
            })
                .then(response => {
                    $('#modalData').find("div.modal-content").LoadingOverlay('hide');
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    console.log("Respuesta del servidor:", responseJson);

                    if (responseJson.estado) {
                        tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                        filaSeleccionada = null;
                        $('#modalData').modal('hide');
                        swal("Listo!", "Producto editado con éxito", "success");
                    } else {
                        swal("Error!", responseJson.mensaje, "error");
                    }
                })
                .catch(error => {
                    console.error("Error en la solicitud:", error);
                    // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
                });
        }

    } else {
        console.log("No se ha seleccionado ningún archivo.");
    }

});

let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    console.log("Fila seleccionada:", filaSeleccionada);

    const data = tablaData.row(filaSeleccionada).data();
    console.log("Datos de la fila:", data);

    mostrarModal(data);
});


$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    console.log("Fila seleccionada:", fila);

    const data = tablaData.row(fila).data();
    console.log("Datos de la fila:", data);

    swal({
        title: "Estas seguro?",
        text: `Eliminar el producto ${data.descripcion}`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $('.showSweetAlert').LoadingOverlay("show");

                fetch(`/Producto/Eliminar?idProducto=${data.idProducto}`, {
                    method: "DELETE",
                })
                    .then(response => {
                        $('.showSweetAlert').LoadingOverlay('hide');
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        console.log("Respuesta del servidor:", responseJson);

                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw(false);

                            swal("Listo!", "Producto eliminado con éxito", "success");
                        } else {
                            swal("Error!", responseJson.mensaje, "error");
                        }
                    })
                    .catch(error => {
                        console.error("Error en la solicitud:", error);
                        // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
                    });


            }
        }
    )
});

function previewImage(input) {
    const imgProducto = $('#imgProducto');

    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            imgProducto.attr('src', e.target.result);
        };

        reader.readAsDataURL(input.files[0]);
    } else {
        imgProducto.attr('src', ''); // Limpiar la imagen si no se selecciona ningún archivo
    }
}

// Llamar a la función cuando cambie el input de la imagen
$("#txtImagen").change(function () {
    previewImage(this);
});
