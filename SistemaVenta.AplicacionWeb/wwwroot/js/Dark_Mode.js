$(document).ready(function () {
    // Obtén el enlace del interruptor de modo oscuro
    const darkModeToggle = $('#darkModeToggle');

    // Obtén el ícono del interruptor
    const darkModeToggleIcon = $('#darkModeToggleIcon');

    // Maneja el clic en el interruptor
    darkModeToggle.on('click', function () {
        // Cambia la clase en el elemento body
        $('body').toggleClass('dark-mode');

        // Cambia la apariencia del ícono
        darkModeToggleIcon.toggleClass('fa-rotate-180');
    });
});
