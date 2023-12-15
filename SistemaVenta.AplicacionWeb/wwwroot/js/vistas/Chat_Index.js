$(document).ready(function () {
    // Oculta el menú de adjuntos al principio
    $("#attachmentMenu").hide();

    // Manejador de clic en el ícono de paperclip
    $("#attachmentIcon").click(function () {
        // Despliega o esconde el menú de adjuntos
        $("#attachmentMenu").toggle();

        // Puedes agregar efectos adicionales si lo deseas
        // Por ejemplo, para que se despliegue hacia arriba:
        $("#imageOption, #documentOption").slideToggle("fast");
    });

    // Manejador de clic en las opciones del menú
    $(".option").click(function () {
        const optionType = $(this).data('type');
       
        // Cierra el menú de adjuntos
        $("#attachmentMenu").hide();
    });

    // Manejador de clic en el botón de adjuntos
    $("#attachmentBtn").click(function (event) {
        // Detener la propagación del evento para evitar que llegue al contenedor del chat
        event.stopPropagation();

        // Desplegar o esconder el menú de adjuntos
        $("#attachmentMenu").toggle();
    });

    /// Manejador de clic en las opciones del menú
    $(".option").click(function () {
        const optionType = $(this).data('type');

        // Realizar acciones según el tipo seleccionado
        switch (optionType) {
            case 'image':
                // Abrir un cuadro de diálogo para seleccionar una imagen
                $("#fileInput").attr('accept', 'image/*');
                $("#fileInput").click(); // Simular clic en el input de archivo
                break;
            case 'document':
                // Abrir un cuadro de diálogo para seleccionar un documento
                $("#fileInput").attr('accept', 'application/pdf, application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document');
                $("#fileInput").click(); // Simular clic en el input de archivo
                break;
            // Puedes agregar más casos según sea necesario
        }

        // Cierra el menú de adjuntos
        $("#attachmentMenu").hide();
    });

    // Manejador de cambio en el input de archivo
    $("#fileInput").change(function () {
        const fileInput = $(this);
        const fileName = fileInput.val().split('\\').pop(); // Obtener el nombre del archivo seleccionado

        // Crear un objeto para el mensaje con el nombre del archivo
        const message = {
            text: `Archivo adjunto: ${fileName}`,
            file: fileName,
            isUserMessage: true
        };

        // Agregar el mensaje al chat
        appendMessage(message);

        // Limpiar el valor del fileInput para permitir seleccionar el mismo archivo nuevamente
        fileInput.val('');
    });

    // Variable para almacenar el temporizador
    let typingTimer;
    // Manejador de entrada de texto
    $("#messageInput").on('input', function () {
        // Limpiar el temporizador existente
        clearTimeout(typingTimer);

        // Iniciar un nuevo temporizador
        typingTimer = setTimeout(function () {
            // Si no se ha escrito nada en los últimos 10 segundos, mostrar un mensaje de asistencia
            const lastUserMessage = getLastUserMessage();
            if (lastUserMessage && lastUserMessage.text.trim() === '') {
                const assistanceMessage = {
                    text: '¿Puedo ayudarte en algo?',
                    isSystemMessage: true
                };
                appendMessage(assistanceMessage);
            }
        }, 10000); // 10 segundos (10000 milisegundos)
    });

    // Mensaje de bienvenida después de 2 segundos
    setTimeout(function () {
        appendMessage({ text: "¡Hola! Soy Hubby, el robot de soporte de StockHub. Estoy aquí para resolver tus dudas, sugerencias y problemas. Solo tienes que escribirme lo que necesites y haré todo lo posible por ayudarte. 😊!" });
    }, 2000);

    // Respuestas automáticas
    const respuestasAutomaticas = [
        "Gracias por tu mensaje. Nos pondremos en contacto contigo pronto.",
        "Estamos procesando tu solicitud. Gracias por tu paciencia.",
        "Si tienes alguna pregunta, no dudes en preguntar. Estamos aquí para ayudarte.",
        "Hemos recibido tu mensaje. Nuestro equipo revisará la información y te responderá en breve.",
        "¡Hola! ¿En qué podemos ayudarte hoy?",
        "Tu consulta es importante para nosotros. Estamos trabajando en proporcionarte la mejor respuesta posible.",
        "Gracias por contactarnos. Estamos disponibles para responder a tus preguntas.",
        "Estamos aquí para asistirte. ¿En qué tema específico necesitas ayuda?",
        "¡Saludos! Esperamos que tengas un excelente día. ¿Cómo podemos ayudarte?",
        "Tu satisfacción es nuestra prioridad. ¿Hay algo más en lo que podamos colaborar?",
        "Hemos recibido tu mensaje. En breve, nuestro equipo te proporcionará la información que necesitas.",
        "¿Necesitas ayuda con algo en particular? Estamos aquí para asistirte.",
        "Gracias por tu paciencia. Nuestro equipo está trabajando para responder tu consulta.",
        "¡Hola! ¿En qué podemos colaborar contigo hoy?",
        "Tu opinión es importante para nosotros. ¿Hay algo más que podamos hacer por ti?",
        "Gracias por elegir nuestro servicio. ¿En qué podemos ayudarte hoy?",
        "Estamos agradecidos por tu contacto. ¿En qué tema podemos brindarte asistencia?",
        "¡Hola! Estamos aquí para resolver tus dudas. ¿En qué podemos ayudarte hoy?",
        "Hemos recibido tu mensaje. Nuestro equipo se pondrá en contacto contigo lo antes posible.",
        "Tu satisfacción es nuestra meta. ¿En qué podemos colaborar contigo?"
    ];

    $('#sendMessageButton').click(function (event) {
        event.preventDefault();
        sendMessage();
        // Enviar respuesta automática después de 5 segundos
        setTimeout(function () {
            enviarRespuestaAutomatica();
        }, 5000);
    });

    // Función para enviar mensajes al chat
    function sendMessage() {
        const messageInput = $('#messageInput');
        const fileInput = $('#fileInput');

        const messageText = messageInput.val().trim();
        const fileName = fileInput.val();

        if (messageText === '' && fileName === '') {
            return; // No envíes mensajes vacíos
        }

        // Crear un objeto para el mensaje
        const message = {
            text: messageText,
            file: fileName,
            isUserMessage: true
        };

        // Agregar el mensaje al chat
        appendMessage(message);

        // Limpiar los campos
        messageInput.val('');
        fileInput.val('');
    }

    // Función para enviar respuestas automáticas
    function enviarRespuestaAutomatica() {
        const respuestaAleatoria = respuestasAutomaticas[Math.floor(Math.random() * respuestasAutomaticas.length)];
        const respuestaAutomatica = {
            text: respuestaAleatoria,
            isSystemMessage: true // Establecer la propiedad isSystemMessage a true para respuestas automáticas
        };

        // Agregar la respuesta automática al chat
        appendMessage(respuestaAutomatica);
    }

    
    // Función para agregar mensajes al chat
    function appendMessage(message) {
        const chatMessages = $('#chatMessages');
        const messageDiv = $('<div>').addClass('message');

        // Agregar la imagen del robot si el mensaje es del bot
        if (!message.isUserMessage) {
            const robotImage = $('<img>').attr('src', '/img/Hubby-Perfil.jpeg').addClass('robot-image');
            messageDiv.append(robotImage);
        }

        // Agregar la imagen si existe
        if (message.file) {
            const imageElement = $('<img>').attr('src', message.file).css({
                'max-width': '100%', // Ajusta el ancho máximo de la imagen
                'height': 'auto' // Mantiene la proporción original de la imagen
            });
            messageDiv.append(imageElement);
        }

        // Agregar el texto del mensaje
        if (message.text) {
            messageDiv.append(`<p>${message.text}</p>`);
        }

        // Scroll hasta el final para mostrar el último mensaje
        chatMessages.scrollTop(chatMessages.prop('scrollHeight'));

        // Aplicar diferentes estilos según el tipo de mensaje
        if (message.isUserMessage) {
            messageDiv.addClass('user-message');
            messageDiv.css({
                'background-color': '#a8e6cf', // Fondo diferente para mensajes del usuario
                'float': 'right', // Alineación a la derecha
                'clear': 'both' // Evitar que los mensajes estén uno al lado del otro
            });
        } else {
            messageDiv.addClass('system-message');
            messageDiv.css({
                'background-color': '#e1e1e1', // Fondo diferente para mensajes automáticos
                'float': 'left', // Alineación a la izquierda
                'clear': 'both' // Evitar que los mensajes estén uno al lado del otro
            });
        }

        chatMessages.append(messageDiv);
    }



    // Manejador de clic en el documento para cerrar el menú si se hace clic fuera de él
    $(document).click(function () {
        $("#attachmentMenu").hide();
    });

    // Asignar funciones a eventos de botones
    $('#sendMessageButton').on('click', function (event) {
        event.preventDefault();
        sendMessage();
        processUserClick();
    });
    // Manejador de cambio en el input de archivo
    $("#fileInput").change(function (event) {
        const fileInput = $(this);
        const file = event.target.files[0];

        if (file) {
            // Verificar si el archivo es una imagen
            if (file.type.startsWith('image/')) {
                const reader = new FileReader();

                reader.onload = function (e) {
                    const imageUrl = e.target.result;

                    // Crear un objeto para el mensaje con la previsualización de la imagen
                    const message = {
                        text: `Imagen adjunta: ${file.name}`,
                        file: imageUrl,
                        isUserMessage: true
                    };
                    
                    // Agregar el mensaje al chat
                    appendMessage(message);

                    

                    // Limpiar el valor del fileInput para permitir seleccionar el mismo archivo nuevamente
                    fileInput.val('');
                };
                // Crear un objeto para el mensaje con el nombre del archivo
                const message2 = {
                    text: `¡Archivo recibido! Se ha recibido un archivo! Procesaremos la información en breve. Gracias por tu envío.`,
                    isSystemMessage: true
                };
                // Agregar el mensaje al chat
                appendMessage(message2);
                // Leer el contenido de la imagen como una URL de datos
                reader.readAsDataURL(file);
            } else {
                alert('Por favor, selecciona una imagen.');
            }
        }
    });

    // Función para obtener el último mensaje del usuario
    function getLastUserMessage() {
        const chatMessages = $('#chatMessages').find('.user-message');
        if (chatMessages.length > 0) {
            // Obtener el último mensaje del usuario
            const lastUserMessage = chatMessages.last();
            return {
                text: lastUserMessage.find('p').text(),
                isUserMessage: true
            };
        }
        return null;
    }
    /*Texto para acciones*/

    //Usuarios
    function crearUsuario() {
        const message = {
            text: "Para crear un usuario sigue los siguientes pasos:<br><br>" +
                "1. Ir al apartado de administración en la barra lateral.<br>" +
                "2. Abrir la sección de usuarios.<br>" +
                "3. Hacer clic en el botón de nuevo usuario.<br>" +
                "4. Rellenar los datos del usuario.<br>" +
                "5. Confirmar. <br>" +
                "6. Esperar a que el usuario reciba sus credenciales por correo electrónico.",
            isSystemMessage: true
        };
        setTimeout(function () {
            message
        }, 2000);
        //appendMessage(message);
    }

    function editarUsuario() {
        const message = {
            text: "Para editar un usuario sigue los siguientes pasos:<br>" +
                "1. Ir al apartado de administración en la barra lateral.<br>" +
                "2. Abrir la sección de usuarios.<br>" +
                "3. Hacer clic sobre el botón de lápiz.<br>" +
                "4. Realizar los cambios necesarios.<br>" +
                "5. Confirmar los cambios.",
            isSystemMessage: true
        };

        setTimeout(function () {
            message
        }, 2000);
    }

    function eliminarUsuario() {
        const message = {
            text: "Para eliminar un usuario sigue los siguientes pasos:<br>" +
                "1. Ir al apartado de administración en la barra lateral.<br>" +
                "2. Abrir la sección de usuarios.<br>" +
                "3. Hacer clic sobre el botón de basura.<br>" +
                "4. Confirmar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }

    //Categoria
    function crearCategoria() {
        const message = {
            text: "Para crear una categoria sigue los siguientes pasos:<br>" +
                "1. Ir al apartado de inventario en la barra lateral.<br>" +
                "2. Abrir la sección de categorias.<br>" +
                "3. Hacer clic sobre el botón de nueva categoria.<br>" +
                "3. Rellenar los datos de la categoria.<br>" +
                "4. Confirmar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    function editarCategoria() {
        const message = {
            text: "Para editar una categoria sigue los siguientes pasos:<br>" +
                "1. Ir al apartado de inventario en la barra lateral.<br>" +
                "2. Abrir la sección de categorias.<br>" +
                "3. Hacer clic sobre el botón de lapiz al lado de la categoria a editar.<br>" +
                "4. Rellenar los datos de la categoria.<br>" +
                "5. Confirmar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    function eliminarCategoria() {
        const message = {
            text: "Para eliminar una categoria sigue los siguientes pasos:<br>" +
                "1. Ir al apartado de inventario en la barra lateral.<br>" +
                "2. Abrir la sección de categorias.\n" +
                "3. Hacer clic sobre el botón de basura al lado de la categoria a editar.<br>" +
                "4. Confirmar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }

    //Producto
    function crearProducto() {
        const message = {
            text: "Para crear un producto sigue los siguientes pasos:<br>" +
                "1. Ir al apartado de inventario en la barra lateral.<br>" +
                "2. Abrir la sección de productos.<br>" +
                "3. Hacer clic sobre el botón de nueva producto.<br>" +
                "4. Rellenar los datos del producto.<br>" +
                "5. Confirmar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    function editarProducto() {
        const message = {
            text: "Para editar un producto sigue los siguientes pasos:<br><br>" +
                "1. Ir al apartado de inventario en la barra lateral.<br>" +
                "2. Abrir la sección de productos.<br>" +
                "3. Hacer clic sobre el botón de lapiz.<br>" +
                "4. Rellenar los datos del producto.<br>" +
                "5. Confirmar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    function eliminarProducto() {
        const message = {
            text: "Para eliminar un producto sigue los siguientes pasos:<br><br>" +
                "1. Ir al apartado de inventario en la barra lateral.<br>" +
                "2. Abrir la sección de productos.<br>" +
                "3. Hacer clic sobre el botón de basura al lado del producto a borrar.<br>" +
                "4. Confirmar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    //Editar
    function editarNegocio() {
        const message = {
            text: "Para editar la info del negocio sigue los siguientes pasos:<br><br>" +
                "1. Ir al apartado de administración en la barra lateral.<br>" +
                "2. Abrir la sección de negocio.<br>" +
                "3. Hacer los cambios.<br>" +
                "4. Confirmar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    //Crear Venta
    function crearVenta() {
        const message = {
            text: "Para crear una ventasigue los siguientes pasos:<br><br>" +
                "1. Ir al apartado de ventas en la barra lateral.<br>" +
                "2. Abrir la sección de nueva venta.<br>" +
                "3. Rellenar datos de usuario y elegir productos ( y la cantidad que se venderán).<br>" +
                "4. Confirmar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    //Ver historial de ventas
    function verHistorialVenta() {
        const message = {
            text: "Para ver el historial de ventas sigue los siguientes pasos:<br><br>" +
                "1. Ir al apartado de ventas en la barra lateral.<br>" +
                "2. Abrir la sección de historial de ventas.<br>" +
                "3. Seleccionar si buscar por fecha o por numero de venta.<br>" +
                "4. Si es por fecha seleccionar la fecha de inicio y la de fin que se desea buscar, sino introduce el numero de venta.<br>" +
                "5. Dar clic a buscar.<br>" +
                "6. Si deseas ver detalles, dar clic al simbolo de ojo y si deseas imprimir una factura, darle clic a imprimir.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    //Ver reporte
    function verReporteVenta() {
        const message = {
            text: "Para ver el reporte de ventas sigue los siguientes pasos:<br><br>" +
                "1. Ir al apartado de reportes en la barra lateral.<br>" +
                "2. Abrir la seccion de reportes de ventas.<br>" +
                "3. Seleccionar  la fecha de inicio y la de fin que se desea buscar.<br>" +
                "4. dar clic a buscar.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    //Exportar Excel:
    function exportarExcel() {
        const message = {
            text: "Para exportar un excel sigue los siguientes pasos:<br><br>" +
                
                "1.Darle clic al boton de exportar excel en el apartado del que desees importar el excel	.",
            isSystemMessage: true
        };

        appendMessage(message);
    }
    // Esta función se llama cuando se hace clic en algún elemento (por ejemplo, un botón) en tu interfaz de usuario
    function processUserClick() {
        // Obtener el último mensaje del usuario
        const lastUserMessage = getLastUserMessage();

        // Verificar si el mensaje contiene palabras clave y responder en consecuencia
        if (lastUserMessage) {
            const messageText = lastUserMessage.text.toLowerCase();

            if ((messageText.includes('crear') || messageText.includes('creo')) && messageText.includes('usuario')) {
                crearUsuario();
            }  else if (messageText.includes('editar') && messageText.includes('usuario') || messageText.includes('edito') && messageText.includes('usuario')) {
                editarUsuario();
            } else if (messageText.includes('eliminar') && messageText.includes('usuario') || messageText.includes('elimino') && messageText.includes('usuario')) {
                eliminarUsuario();
            } else if (messageText.includes('crear') && messageText.includes('categoria') || messageText.includes('creo') && messageText.includes('categoria')) {
                crearCategoria();
            } else if (messageText.includes('editar') && messageText.includes('categoria') || messageText.includes('edito') && messageText.includes('categoria')) {
                editarCategoria();
            } else if (messageText.includes('eliminar') && messageText.includes('categoria') || messageText.includes('elimino') && messageText.includes('categoria')) {
                eliminarCategoria();
            } else if (messageText.includes('ver') && messageText.includes('reporte') || messageText.includes('veo') && messageText.includes('reporte')) {
                verReporteVenta();
            } else if (messageText.includes('ver') && messageText.includes('historial') || messageText.includes('veo') && messageText.includes('historial')) {
                verHistorialVenta();
            } else if (messageText.includes('editar') && messageText.includes('negocio') || messageText.includes('edito') && messageText.includes('negocio')) {
                editarNegocio();
            } else if (messageText.includes('crear') && messageText.includes('venta') || messageText.includes('creo') && messageText.includes('venta')) {
                crearVenta();
            } else if (messageText.includes('exportar') && messageText.includes('excel') || messageText.includes('exporto') && messageText.includes('excel')) {
                exportarExcel();
            } if (messageText.includes('crear') && messageText.includes('producto') || messageText.includes('creo') && messageText.includes('producto')) {
                crearProducto();
            } else if (messageText.includes('editar') && messageText.includes('producto') || messageText.includes('edito') && messageText.includes('producto')) {
                editarProducto();
            } else if (messageText.includes('eliminar') && messageText.includes('producto') || messageText.includes('elimino') && messageText.includes('producto')) {
                eliminarProducto();
            }

            
        }
    }
    
});
