$(document).ready(function () {

    $('.container-fluid').LoadingOverlay("show");

    fetch("/Home/ObtenerUsuario")
        .then(response => {
            $('.container-fluid').LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJSON => {

            console.log(responseJSON);
            if (responseJSON.estado) {
                const d = responseJSON.objeto;

                $('#imgFoto').attr('src', d.urlFoto);
                $('#txtNombre').val(d.nombre);
                $('#txtCorreo').val(d.correo);
                $('#txTelefono').val(d.telefono);
                $('#txtRol').val(d.nombreRol);
                


            } else {
                swal("Error!", responseJson.mensaje, "error");
                console.log("No se encontraron Usuarios.");  // Puedes agregar un mensaje de registro o manejo aquí
            }
        })
        .catch(error => {
            console.error("Error en la solicitud:", error);
            // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
        });
})

$('#btnGuardarCambios').click(function () {

    const formatoCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if ($('#txtCorreo').val().trim() == "") {
        const mensaje = `Por favor completar el campo: Correo`;
        toastr.warning("", mensaje);
        $('#txtCorreo').focus();
        return;
    } else if (!formatoCorreo.test($('#txtCorreo').val().trim())) {
        const mensaje = `Por favor ingrese un correo electrónico válido`;
        toastr.warning("", mensaje);
        $('#txtCorreo').focus();
        return;
    }

    
    if ($('#txTelefono').val().trim() == "") {
        const mensaje = `Por favor completar el campo: Telefono`;
        toastr.warning("", mensaje);
        $('#txTelefono').focus();
        return;
    }

    swal({
        title: "¿Deseas guardar los cambios?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si, confirmar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $('.showSweetAlert').LoadingOverlay("show");

                let modelo = {
                    correo: $('#txtCorreo').val().trim(),
                    telefono: $('#txTelefono').val().trim()
                };


                fetch("/Home/GuardarPerfil", {
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    body: JSON.stringify(modelo),
                })
                    .then(response => {
                        $('.showSweetAlert').LoadingOverlay('hide');
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        console.log("Respuesta del servidor:", responseJson);

                        if (responseJson.estado) {

                            swal("Listo!", "Los cambios se han guardado con éxito", "success");
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

$("#btnCambiarClave").click(function () {
    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter((item) => item.value.trim() == "");

    if (inputs_vacios.length > 0) {
        const mensaje = `Por favor completar el campo: ${inputs_vacios[0].name}`;
        toastr.warning("", mensaje);
        $(`input[name=${inputs_vacios[0].name}]`).focus();
        return; // Agregamos esta línea para salir de la función si hay campos vacíos
    }

    if ($('#txtClaveNueva').val().trim() == "") {
        const mensaje = `Por favor introduce la nueva contraseña`;
        toastr.warning("", mensaje);
        $('#txtClaveNueva').focus();
        return;
    }
    if ($('#txtConfirmarClave').val().trim() == "") {
        const mensaje = `Por favor introduce la confirmacion de la nueva contraseña`;
        toastr.warning("", mensaje);
        $('#txtConfirmarClave').focus();
        return;
    }

    if ($('#txtClaveNueva').val().trim() != $('#txtConfirmarClave').val().trim()) {
        const mensaje = `Las contraseñas no coinciden`;
        toastr.warning("", mensaje);
        return;
    }
    swal({
        title: "¿Deseas cambiar tu contraseña?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Sí, confirmar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $('.showSweetAlert').LoadingOverlay("show");

                let modelo = {
                    claveActual: $('#txtClaveActual').val().trim(),
                    claveNueva: $('#txtClaveNueva').val().trim()
                }

                fetch("/Home/CambiarClave", {
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    body: JSON.stringify(modelo),
                })
                    .then(response => {
                        $('.showSweetAlert').LoadingOverlay('hide');
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        console.log("Respuesta del servidor:", responseJson);

                        if (responseJson.estado) {
                            swal("Listo!", "La contraseña se ha cambiado con éxito", "success");
                            $("input.input-validar").val("");
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
    );

    
})