const modeloBase = {
    idCategoria: 0,
    descripcion: "",
    esActivo: 1,
}

let tablaData;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Categoria/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idCategoria", "visible": false, "searchable": false },
            { "data": "descripcion" },
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
        dom: "Bfrtip", // Coma añadida aquí
        initComplete: function () {
            this.api().buttons().container().appendTo($('#tbdata_wrapper .col-md-6:eq(0)'));
        },
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Categorias',
                exportOptions: {
                    columns: [1, 2]
                }
            }
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });

});
function mostrarModal(modelo = modeloBase) {
    $('#txtId').val(modelo.idCategoria);
    $('#txtDescripcion').val(modelo.descripcion);
    $('#cboEstado').val(modelo.esActivo);

    $('#modalData').modal("show");
}


$("#btnNuevo").click(
    function () {
        mostrarModal()
    }
);

$("#btnGuardar").click(function () {
    
    if ($('#txtDescripcion').val().trim() == ""){
        const mensaje = `Por favor completar el campo: Descripcion`;
        toastr.warning("", mensaje);
        $('#txtDescripcion').focus();
        return;
    }

    const idCategoriaInput = $("#txtId").val();
    const idCategoria = idCategoriaInput ? parseInt(idCategoriaInput.trim()) : 0;
    console.log(idCategoria);

    const modelo = {
        idCategoria: idCategoriaInput,
        descripcion: $('#txtDescripcion').val(),
        esActivo: $("#cboEstado").val(),
    };

    console.log("Modelo:", modelo);

  
        $('#modalData').find("div.modal-content").LoadingOverlay("show");

        if (modelo.idCategoria == 0) {
            //console.log("Entró en el bloque if (modelo.idUsuario == 0)");

            fetch("/Categoria/Crear", {
                method: "POST",
                headers: {"Content-Type":"application/json; charset=utf-8"},
                body: JSON.stringify(modelo),
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
                        swal("Listo!", "Categoria añadida con éxito", "success");
                    } else {
                        swal("Error!", responseJson.mensaje, "error");
                    }
                })
                .catch(error => {
                    console.error("Error en la solicitud:", error);
                    // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
                });
        } else {
            fetch("/Categoria/Editar", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(modelo),
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
                        //Crea un modal exitoso usando librería SweatAlert2
                        swal("Listo!", "Categoria editada con éxito", "success");
                    } else {
                        //Crea un modal de error usando librería SweatAlert2
                        swal("Error!", responseJson.mensaje, "error");
                    }
                })
                .catch(error => {
                    console.error("Error en la solicitud:", error);
                });
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
        text: `Eliminar la categoria ${data.descripcion}`,
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

                fetch(`/Categoria/Eliminar?idCategoria=${data.idCategoria}`, {
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

                            swal("Listo!", "Categoria eliminada con éxito", "success");
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