(function($) {
  "use strict"; // Start of use strict

  // Toggle the side navigation
  $("#sidebarToggle, #sidebarToggleTop").on('click', function(e) {
    $("body").toggleClass("sidebar-toggled");
    $(".sidebar").toggleClass("toggled");
    if ($(".sidebar").hasClass("toggled")) {
      $('.sidebar .collapse').collapse('hide');
    };
  });

  // Close any open menu accordions when window is resized below 768px
  $(window).resize(function() {
    if ($(window).width() < 768) {
      $('.sidebar .collapse').collapse('hide');
    };
    
    // Toggle the side navigation when window is resized below 480px
    if ($(window).width() < 480 && !$(".sidebar").hasClass("toggled")) {
      $("body").addClass("sidebar-toggled");
      $(".sidebar").addClass("toggled");
      $('.sidebar .collapse').collapse('hide');
    };
  });

  // Prevent the content wrapper from scrolling when the fixed side navigation hovered over
  $('body.fixed-nav .sidebar').on('mousewheel DOMMouseScroll wheel', function(e) {
    if ($(window).width() > 768) {
      var e0 = e.originalEvent,
        delta = e0.wheelDelta || -e0.detail;
      this.scrollTop += (delta < 0 ? 1 : -1) * 30;
      e.preventDefault();
    }
  });

  // Scroll to top button appear
  $(document).on('scroll', function() {
    var scrollDistance = $(this).scrollTop();
    if (scrollDistance > 100) {
      $('.scroll-to-top').fadeIn();
    } else {
      $('.scroll-to-top').fadeOut();
    }
  });

  // Smooth scrolling using jQuery easing
  $(document).on('click', 'a.scroll-to-top', function(e) {
    var $anchor = $(this);
    $('html, body').stop().animate({
      scrollTop: ($($anchor.attr('href')).offset().top)
    }, 1000, 'easeInOutExpo');
    e.preventDefault();
  });

    // Alternar el modo oscuro
    $("#darkModeToggle").on('click', function () {
        console.log('Boton modo funciona');
        $("body").toggleClass("dark-mode");

        // Lógica para cambiar estilos y el ícono del botón según el modo oscuro
        if ($("body").hasClass("dark-mode")) {
            // Estilos para modo oscuro
            $("body").css("background-color", "#1a1a1a");
            $("body").css("color", "#ffffff");
            $("#darkModeToggle i").removeClass("fa-sun").addClass("fa-moon"); // Cambia el ícono a luna
            // Puedes añadir más estilos aquí según tu necesidad
        } else {
            // Estilos para modo claro (revertir cambios)
            $("body").css("background-color", "#ffffff");
            $("body").css("color", "#000000");
            $("#darkModeToggle i").removeClass("fa-moon").addClass("fa-sun"); // Cambia el ícono a sol
            // Puedes revertir más estilos aquí según tu necesidad
        }
    });


})(jQuery); // End of use strict
// Esperar a que el DOM esté completamente cargado
$(document).ready(function () {
    // Alternar el modo oscuro
    $(document).on('click', "#darkModeToggle", function () {
        console.log('Boton modo funciona');
        $("body").toggleClass("dark-mode");

        // Lógica para cambiar estilos y el ícono del botón según el modo oscuro
        if ($("body").hasClass("dark-mode")) {
            // Estilos para modo oscuro
            $("body").css("background-color", "#1a1a1a");
            $("body").css("color", "#ffffff");
            $("#darkModeToggle i").removeClass("fa-sun").addClass("fa-moon"); // Cambia el ícono a luna
            // Puedes añadir más estilos aquí según tu necesidad
        } else {
            // Estilos para modo claro (revertir cambios)
            $("body").css("background-color", "#ffffff");
            $("body").css("color", "#000000");
            $("#darkModeToggle i").removeClass("fa-moon").addClass("fa-sun"); // Cambia el ícono a sol
            // Puedes revertir más estilos aquí según tu necesidad
        }
    });
});
