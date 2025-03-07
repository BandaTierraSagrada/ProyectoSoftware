jQuery(document).ready(function ($) {
    const tab = $('.tabs h3 a');

    tab.on('click', function (event) {
        event.preventDefault();
        tab.removeClass('active');
        $(this).addClass('active');

        let tab_content = $(this).attr('href');
        $('div[id$="tab-content"]').removeClass('active');
        $(tab_content).addClass('active');

        // Determinar si es pestaña de registro o login
        if (tab_content === "#register-tab-content") {
            // Habilitar campos de registro y deshabilitar los de login
            $('.register-field').prop('disabled', false).attr("name", function () { return $(this).data("name"); });
            $('.login-field').prop('disabled', true).removeAttr("name");
        } else if (tab_content === "#login-tab-content") {
            // Habilitar campos de login y deshabilitar los de registro
            $('.login-field').prop('disabled', false).attr("name", function () { return $(this).data("name"); });
            $('.register-field').prop('disabled', true).removeAttr("name");
        }
    });

    // Para asegurar que se seleccionen los campos correctos al inicio
    $('.tabs h3 a.active').trigger('click');
});
