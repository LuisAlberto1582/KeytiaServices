jQNewLook(document).ready(function () {

    jQNewLook(".dashboard-stat").find(".desc").addClass("col-lg-12 col-md-12 col-sm-12 col-xs-12");

    /*------------------------------------------------------------------------------ Centrar el texto del Footer --------------------------------------------*/

    //Mover el footer a que este alineado al centro
    jQNewLook("#btnTogglerBar").click(function () {
        if (jQNewLook(window).width() >= 992) {
            var sideBarExpanded = (jQNewLook("body").hasClass("page-sidebar-closed") ? "MenuAbierto" : "MenuCerrado");
            switch (sideBarExpanded) {
                case "MenuAbierto":
                    jQNewLook('.FooterKeytiaTitle').attr('style', 'margin-left: -300px !important;');
                    break;

                case "MenuCerrado":
                    jQNewLook('.FooterKeytiaTitle').attr('style', 'margin-left: -140px !important;');
                    break;
            }
        }
    });

    //Mover el footer a que este alineado al centro
    jQNewLook(window).resize(function () {
        var caseSize = (jQNewLook(this).width() <= 992) ? "DispositivosPequenos" : "DispositivosGrandes";
        switch (caseSize) {
            case "DispositivosPequenos":
                jQNewLook('.FooterKeytiaTitle').attr('style', 'margin-left: 0px !important;');

                //Regulariza la direccion de la fecha colocada en el menu para dispositivos moviles
                jQNewLook(".nav-link").find(".arrowRight").addClass("left");
                break;

            case "DispositivosGrandes":
                //Regulariza la direccion de la fecha colocada en el menu para dispositivos moviles
                jQNewLook(".nav-link").find(".arrowRight").removeClass("left");
                jQNewLook(".nav-link").find(".arrowRight").removeClass("down")

                //Centra el texto del Footer de la pagina si la ventana se cambia de tamaño
                var sideBarExpanded = (jQNewLook("body").hasClass("page-sidebar-closed") ? "MenuCerrado" : "MenuAbierto");
                switch (sideBarExpanded) {
                    case "MenuAbierto":
                        jQNewLook('.FooterKeytiaTitle').attr('style', 'margin-left: -300px !important;');
                        break;
                    case "MenuCerrado":
                        jQNewLook('.FooterKeytiaTitle').attr('style', 'margin-left: -140px !important;');
                        break;
                }
                break;
        }
    });

    var caseSize = (jQNewLook(this).width() <= 992) ? "DispositivosPequenos" : "DispositivosGrandes";
    switch (caseSize) {
        case "DispositivosPequenos":
            jQNewLook('.FooterKeytiaTitle').attr('style', 'margin-left: 0px !important;');

            //Regulariza la direccion de la fecha colocada en el menu para dispositivos moviles
            jQNewLook(".nav-link").find(".arrowRight").addClass("left");
            break;

        case "DispositivosGrandes":
            //Regulariza la direccion de la fecha colocada en el menu para dispositivos moviles
            jQNewLook(".nav-link").find(".arrowRight").removeClass("left");
            jQNewLook(".nav-link").find(".arrowRight").removeClass("down")

            //Centra el texto del Footer de la pagina si la ventana se cambia de tamaño
            var sideBarExpanded = (jQNewLook("body").hasClass("page-sidebar-closed") ? "MenuCerrado" : "MenuAbierto");
            switch (sideBarExpanded) {
                case "MenuAbierto":
                    jQNewLook('.FooterKeytiaTitle').attr('style', 'margin-left: -300px !important;');
                    break;
                case "MenuCerrado":
                    jQNewLook('.FooterKeytiaTitle').attr('style', 'margin-left: -140px !important;');
                    break;
            }
            break;
    }


    /*------------------------------------------------------------------------------ Centrar el texto del Footer ----------------------------------------------------------*/

    /*------------------------------------------------------------------------------ Portlet ------------------------------------------------------------------------------*/

    //Detectar el expandido y el colapsado de los Portlet para poder cambiar la imagen de "-" y "+"
    jQNewLook(".actions").find("button").click(function () {
        var dataTarget = jQNewLook(this).attr("data-target");
        var portletExpanded = (jQNewLook(dataTarget).hasClass("in")) ? "PortletCerrado" : "PortletExpandido";
        switch (portletExpanded) {
            case "PortletExpandido":
                jQNewLook(this).find(".svg-inline--fa").removeClass("fa-plus-square").addClass("fa-minus-square");
                break;

            case "PortletCerrado":
                jQNewLook(this).find(".svg-inline--fa").removeClass("fa-minus-square").addClass("fa-plus-square");
                break;
        }
    });

    /*------------------------------------------------------------------------------ /Portlet ------------------------------------------------------------------------------*/

    //Actualizar el Dropdown de Bootstrap
    jQNewLook("#dropdown12").find("li").find("a").click(function () {
        jQNewLook("#dropdown12").find(".btn:first-child").html(jQNewLook(this).text() + ' <i class="fas fa-angle-down"></i>');
    });

    /*---------------------------------------------------------------------------- Centra los modales en la PAGINA ---------------------------------------------------------------------------*/

    jQNewLook(function () {
        function reposition() {
            var modal = jQNewLook(this),
                dialog = modal.find('.modal-dialog');
            modal.css('display', 'block');
            dialog.css("margin-top", Math.max(0, (jQNewLook(window).height() - dialog.height()) / 2));
        }

        jQNewLook('.modal').on('show.bs.modal', reposition);

        jQNewLook(window).on('resize', function () {
            jQNewLook('.modal:visible').each(reposition);
        });
    });

    /*---------------------------------------------------------------------------- Centra los modales en la PAGINA ---------------------------------------------------------------------------*/





    /*NZ: Se agrega la siguiente seccion para colapsar el Menu en base a la configuracion del Cliente */
    $(function () {
        var bandera = $("[id$='_hdOpcCollapse']").val();

        if (bandera == '1') {
            $("body").find(".sidebar-toggler.btnTogglerBar").click();
        }
    });

});
