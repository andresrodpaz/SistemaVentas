$(document).ready(function () {

    $('.card-body').LoadingOverlay("show");

    fetch("/Negocio/Obtener")
        .then(response => {
            $('.card-body').LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJSON => {

            console.log(responseJSON);
            if (responseJSON.estado) {
                const d = responseJSON.objeto;

                $('#txtNumeroDocumento').val(d.numeroDocumento);
                $('#txtRazonSocial').val(d.nombre);
                $('#txtCorreo').val(d.correo);
                $('#txtDireccion').val(d.direccion);
                $('#txTelefono').val(d.telefono);
                $('#txtImpuesto').val(d.porcentajeImpuesto);
                $('#txtSimboloMoneda').val(d.simboloMoneda);
                $('#imgLogo').attr('src', d.urlLogo);

                
            } else {
                swal("Error!", responseJson.mensaje, "error");
                console.log("No se encontraron roles.");  // Puedes agregar un mensaje de registro o manejo aquí
            }
        })
        .catch(error => {
            console.error("Error en la solicitud:", error);
            // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
        });
})

$('#btnGuardarCambios').click(function () {
    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter((item) => item.value.trim() == "");

    if (inputs_vacios.length > 0) {
        const mensaje = `Por favor completar el campo: ${inputs_vacios[0].name}`;
        toastr.warning("", mensaje);
        $(`input[name=${inputs_vacios[0].name}]`).focus();
        return; // Agregamos esta línea para salir de la función si hay campos vacíos
    }

    const modelo = {

        numeroDocumento: $('#txtNumeroDocumento').val(),
        nombre: $('#txtRazonSocial').val(),
        correo : $('#txtCorreo').val(),
        direccion: $('#txtDireccion').val(),
        telefono: $('#txTelefono').val(),
        porcentajeImpuesto: $('#txtImpuesto').val(),
        simboloMoneda: $('#txtSimboloMoneda').val()
    }

    const inputLogo = document.getElementById('txtLogo');

    const formData = new FormData();

    //Metodo del controlle rrecibe parametros: logo y modelo
    formData.append("logo", inputLogo.files[0]);
    formData.append("modelo", JSON.stringify(modelo));

    $('.card-body').LoadingOverlay("show");

    fetch("/Negocio/GuardarCambios", {
        method: "POST",
        body: formData
    })
        .then(response => {
            $('.card-body').LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJSON => {

            console.log(responseJSON);
            if (responseJSON.estado) {
                const d = responseJSON.objeto;

                $('#imgLogo').attr('src', d.urlLogo);
               
                swal("Éxito", "Cambios guardados correctamente", "success");

            } else {
                swal("Error!", responseJson.mensaje, "error");
                console.log("No se encontraron roles.");  // Puedes agregar un mensaje de registro o manejo aquí
            }
        })
        .catch(error => {
            console.error("Error en la solicitud:", error);
            // Puedes manejar el error de alguna manera, por ejemplo, mostrando un mensaje al usuario.
        });

})