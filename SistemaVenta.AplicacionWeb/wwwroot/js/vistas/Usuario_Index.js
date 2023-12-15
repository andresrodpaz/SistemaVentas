const modeloBase = {
    idUsuario: 0,
    nombre: "",
    correo: "",
    telefono: "",
    idRol: 0,
    esActivo: 1,
    urlFoto:""
}

let tablaData;

/*
    ('#tbdata').DataTable --> #tbdata es el id de la tabla en el index y lo convertimos al tipo DataTable
    responsive: true --> es responsive
*/

$(document).ready(function () {

    fetch("/Usuario/ListaRoles")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJSON => {
            if (responseJSON.length > 0) {  
                responseJSON.forEach((item) => {
                    $("#cboRol").append(
                        $("<option>").val(item.idRol).text(item.descripcion)
                    );
                });
            } else {
                console.log("No se encontraron roles.");  // Puedes agregar un mensaje de registro o manejo aquí
            }
        })
        .catch(error => {
            console.error("Error en la solicitud:", error);
            // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
        });



    tablaData = $('#tbdata').DataTable({
        responsive: true,
         "ajax": {
             "url": '/Usuario/ListaUsuarios',
             "type": "GET",
             "datatype": "json"
         },
        "columns": [
            { "data": "idUsuario", "visible": false, "searchable": false },
            {
                "data": "urlFoto", render: function (data) {
                    console.log("URL de la imagen:", data);
                    //return '<img style="height:60px"" src=${data} class="rounded mx-auto d-block"/>' retorna simbolo de imagen missing pero da error de consola por las comillas
                    return `<img style="height:60px" src="${data}" class="rounded mx-auto d-block"/>`;

                }
            },
            { "data": "nombre" },
            { "data": "correo" },
            { "data": "telefono" },
            { "data": "nombreRol" },
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
                filename: 'Reporte Usuarios',
                exportOptions: {
                    columns: [2,3,4,5,6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
});

function mostrarModal(modelo = modeloBase) {
    $('#txtId').val(modelo.idUsuario);
    $('#txtNombre').val(modelo.nombre);
    $('#txtCorreo').val(modelo.correo);
    $('#txtTelefono').val(modelo.telefono);
    $('#cboRol').val(modelo.idRol === 0 ? $('#cboRol option:first').val() : modelo.idRol);
    $('#cboEstado').val(modelo.esActivo);
    $('#txtFoto').val("");
    $('#imgUsuario').attr("src", modelo.urlFoto);

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

    const idUsuarioInput = $("#txtId").val();
    const idUsuario = idUsuarioInput ? parseInt(idUsuarioInput.trim()) : 0;
    console.log(idUsuario);

    const tipoDocumento = $("#txtTipoDocumento").val();
    const numeroDocumento = $("#txtNumeroDocumento").val().trim();
    console.log("Tipo de Documento Seleccionado: ", tipoDocumento);
    // Validación del formato del número de documento según el tipo seleccionado
    if (tipoDocumento === "DNI" && !/^\d{8}[A-Z]$/.test(numeroDocumento)) {
        const mensaje = "Por favor ingresa un DNI válido (8 dígitos numéricos + letra).";
        toastr.warning("", mensaje);
        $("#txtNumeroDocumento").focus();
        return;
    } else if (tipoDocumento === "NIE" && !/^[XYZ]\d{7}[A-Z]$/.test(numeroDocumento)) {
        const mensaje = "Por favor ingresa un NIE válido (letra + 7 dígitos numéricos + letra).";
        toastr.warning("", mensaje);
        $("#txtNumeroDocumento").focus();
        return;
    } else if (tipoDocumento === "Pasaporte" && !/^.{1,20}$/.test(numeroDocumento)) {
        const mensaje = "Por favor ingresa un Pasaporte válido (hasta 20 caracteres).";
        toastr.warning("", mensaje);
        $("#txtNumeroDocumento").focus();
        return;
    }

    const modelo = {
        idUsuario: idUsuarioInput,
        nombre: $("#txtNombre").val(),
        correo: $("#txtCorreo").val(),
        telefono: $("#txtTelefono").val(),
        idRol: $("#cboRol").val(),
        esActivo: $("#cboEstado").val(),
    };

    console.log("Modelo:", modelo);

    const inputFoto = document.getElementById("txtFoto");

    if (inputFoto.files.length > 0) {
        const file = inputFoto.files[0];
        console.log("Archivo seleccionado:", file);

        const formData = new FormData();
        formData.append("foto", file);
        formData.append("modelo", JSON.stringify(modelo));

        console.log("FormData:", formData);

        $('#modalData').find("div.modal-content").LoadingOverlay("show");

        if (modelo.idUsuario == 0) {
            //console.log("Entró en el bloque if (modelo.idUsuario == 0)");
            //Peticion para Crear
            fetch("/Usuario/Crear", {
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
                        swal("Listo!", "Usuario añadido con éxito", "success");
                    } else {
                        swal("Error!", responseJson.mensaje, "error");
                    }
                })
                .catch(error => {
                    console.error("Error en la solicitud:", error);
                    // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
                });
        } else {
            //Peticion para editar
            fetch("/Usuario/Editar", {
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
                        swal("Listo!", "Usuario editado con éxito", "success");
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

//Funcion click para abrir modal de editar con fila seleccionada
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

//Funcion para seleccionar fila y eliminarla
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
    //Abre swal de vonfirmacion
    swal({
        title: "Estas seguro?",
        text: `Eliminar al usuario ${data.nombre}`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel:true
    },
        function (respuesta) {
            if (respuesta) {
                $('.showSweetAlert').LoadingOverlay("show");

                fetch(`/Usuario/Eliminar?idUsuario=${data.idUsuario}`, {
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
                            
                            swal("Listo!", "Usuario eliminado con éxito", "success");
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
})
    ;
//función para previsualizar la imagen
function previsualizarImagen(input) {
    const reader = new FileReader();

    reader.onload = function (e) {
        $('#imgUsuario').attr('src', e.target.result);
    };

    reader.readAsDataURL(input.files[0]);
}

// Llamada a la función previsualizarImagen
$("#txtFoto").change(function () {
    previsualizarImagen(this);
});