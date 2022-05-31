/**
Demo script to handle the theme demo
**/
var Demo = function() {

    // Handle Theme Settings
    var handleTheme = function() {

        var panel = jQNewLook('.theme-panel');

        if (jQNewLook('body').hasClass('page-boxed') === false) {
            jQNewLook('.layout-option', panel).val("fluid");
        }

        jQNewLook('.sidebar-option', panel).val("default");
        jQNewLook('.page-header-option', panel).val("fixed");
        jQNewLook('.page-footer-option', panel).val("default");
        if (jQNewLook('.sidebar-pos-option').attr("disabled") === false) {
            jQNewLook('.sidebar-pos-option', panel).val(App.isRTL() ? 'right' : 'left');
        }

        //handle theme layout
        var resetLayout = function() {
            jQNewLook("body").
            removeClass("page-boxed").
            removeClass("page-footer-fixed").
            removeClass("page-sidebar-fixed").
            removeClass("page-header-fixed").
            removeClass("page-sidebar-reversed");

            jQNewLook('.page-header > .page-header-inner').removeClass("container");

            if (jQNewLook('.page-container').parent(".container").size() === 1) {
                jQNewLook('.page-container').insertAfter('body > .clearfix');
            }

            if (jQNewLook('.page-footer > .container').size() === 1) {
                jQNewLook('.page-footer').html(jQNewLook('.page-footer > .container').html());
            } else if (jQNewLook('.page-footer').parent(".container").size() === 1) {
                jQNewLook('.page-footer').insertAfter('.page-container');
                jQNewLook('.scroll-to-top').insertAfter('.page-footer');
            }

             jQNewLook(".top-menu > .navbar-nav > li.dropdown").removeClass("dropdown-dark");

            jQNewLook('body > .container').remove();
        };

        var lastSelectedLayout = '';

        var setLayout = function() {

            var layoutOption = jQNewLook('.layout-option', panel).val();
            var sidebarOption = jQNewLook('.sidebar-option', panel).val();
            var headerOption = jQNewLook('.page-header-option', panel).val();
            var footerOption = jQNewLook('.page-footer-option', panel).val();
            var sidebarPosOption = jQNewLook('.sidebar-pos-option', panel).val();
            var sidebarStyleOption = jQNewLook('.sidebar-style-option', panel).val();
            var sidebarMenuOption = jQNewLook('.sidebar-menu-option', panel).val();
            var headerTopDropdownStyle = jQNewLook('.page-header-top-dropdown-style-option', panel).val();

            if (sidebarOption == "fixed" && headerOption == "default") {
                alert('Default Header with Fixed Sidebar option is not supported. Proceed with Fixed Header with Fixed Sidebar.');
                jQNewLook('.page-header-option', panel).val("fixed");
                jQNewLook('.sidebar-option', panel).val("fixed");
                sidebarOption = 'fixed';
                headerOption = 'fixed';
            }

            resetLayout(); // reset layout to default state

            if (layoutOption === "boxed") {
                jQNewLook("body").addClass("page-boxed");

                // set header
                jQNewLook('.page-header > .page-header-inner').addClass("container");
                var cont = jQNewLook('body > .clearfix').after('<div class="container"></div>');

                // set content
                jQNewLook('.page-container').appendTo('body > .container');

                // set footer
                if (footerOption === 'fixed') {
                    jQNewLook('.page-footer').html('<div class="container">' + jQNewLook('.page-footer').html() + '</div>');
                } else {
                    jQNewLook('.page-footer').appendTo('body > .container');
                }
            }

            if (lastSelectedLayout != layoutOption) {
                //layout changed, run responsive handler:
                App.runResizeHandlers();
            }
            lastSelectedLayout = layoutOption;

            //header
            if (headerOption === 'fixed') {
                jQNewLook("body").addClass("page-header-fixed");
                jQNewLook(".page-header").removeClass("navbar-static-top").addClass("navbar-fixed-top");
            } else {
                jQNewLook("body").removeClass("page-header-fixed");
                jQNewLook(".page-header").removeClass("navbar-fixed-top").addClass("navbar-static-top");
            }

            //sidebar
            if (jQNewLook('body').hasClass('page-full-width') === false) {
                if (sidebarOption === 'fixed') {
                    jQNewLook("body").addClass("page-sidebar-fixed");
                    jQNewLook("page-sidebar-menu").addClass("page-sidebar-menu-fixed");
                    jQNewLook("page-sidebar-menu").removeClass("page-sidebar-menu-default");
                    Layout.initFixedSidebarHoverEffect();
                } else {
                    jQNewLook("body").removeClass("page-sidebar-fixed");
                    jQNewLook("page-sidebar-menu").addClass("page-sidebar-menu-default");
                    jQNewLook("page-sidebar-menu").removeClass("page-sidebar-menu-fixed");
                    jQNewLook('.page-sidebar-menu').unbind('mouseenter').unbind('mouseleave');
                }
            }

            // top dropdown style
            if (headerTopDropdownStyle === 'dark') {
                jQNewLook(".top-menu > .navbar-nav > li.dropdown").addClass("dropdown-dark");
            } else {
                jQNewLook(".top-menu > .navbar-nav > li.dropdown").removeClass("dropdown-dark");
            }

            //footer
            if (footerOption === 'fixed') {
                jQNewLook("body").addClass("page-footer-fixed");
            } else {
                jQNewLook("body").removeClass("page-footer-fixed");
            }

            //sidebar style
            if (sidebarStyleOption === 'light') {
                jQNewLook(".page-sidebar-menu").addClass("page-sidebar-menu-light");
            } else {
                jQNewLook(".page-sidebar-menu").removeClass("page-sidebar-menu-light");
            }

            //sidebar menu 
            if (sidebarMenuOption === 'hover') {
                if (sidebarOption == 'fixed') {
                    jQNewLook('.sidebar-menu-option', panel).val("accordion");
                    alert("Hover Sidebar Menu is not compatible with Fixed Sidebar Mode. Select Default Sidebar Mode Instead.");
                } else {
                    jQNewLook(".page-sidebar-menu").addClass("page-sidebar-menu-hover-submenu");
                }
            } else {
                jQNewLook(".page-sidebar-menu").removeClass("page-sidebar-menu-hover-submenu");
            }

            //sidebar position
            if (App.isRTL()) {
                if (sidebarPosOption === 'left') {
                    jQNewLook("body").addClass("page-sidebar-reversed");
                    jQNewLook('#frontend-link').tooltip('destroy').tooltip({
                        placement: 'right'
                    });
                } else {
                    jQNewLook("body").removeClass("page-sidebar-reversed");
                    jQNewLook('#frontend-link').tooltip('destroy').tooltip({
                        placement: 'left'
                    });
                }
            } else {
                if (sidebarPosOption === 'right') {
                    jQNewLook("body").addClass("page-sidebar-reversed");
                    jQNewLook('#frontend-link').tooltip('destroy').tooltip({
                        placement: 'left'
                    });
                } else {
                    jQNewLook("body").removeClass("page-sidebar-reversed");
                    jQNewLook('#frontend-link').tooltip('destroy').tooltip({
                        placement: 'right'
                    });
                }
            }

            Layout.fixContentHeight(); // fix content height
            Layout.initFixedSidebar(); // reinitialize fixed sidebar
        };

        // handle theme colors
        var setColor = function(color) {
            var color_ = (App.isRTL() ? color + '-rtl' : color);
            jQNewLook('#style_color').attr("href", Layout.getLayoutCssPath() + 'themes/' + color_ + ".min.css");
            if (color == 'light2') {
                jQNewLook('.page-logo img').attr('src', Layout.getLayoutImgPath() + 'logo-invert.png');
            } else {
                jQNewLook('.page-logo img').attr('src', Layout.getLayoutImgPath() + 'logo.png');
            }
        };

        jQNewLook('.toggler', panel).click(function() {
            jQNewLook('.toggler').hide();
            jQNewLook('.toggler-close').show();
            jQNewLook('.theme-panel > .theme-options').show();
        });

        jQNewLook('.toggler-close', panel).click(function() {
            jQNewLook('.toggler').show();
            jQNewLook('.toggler-close').hide();
            jQNewLook('.theme-panel > .theme-options').hide();
        });

        jQNewLook('.theme-colors > ul > li', panel).click(function() {
            var color = jQNewLook(this).attr("data-style");
            setColor(color);
            jQNewLook('ul > li', panel).removeClass("current");
            jQNewLook(this).addClass("current");
        });

        // set default theme options:

        if (jQNewLook("body").hasClass("page-boxed")) {
            jQNewLook('.layout-option', panel).val("boxed");
        }

        if (jQNewLook("body").hasClass("page-sidebar-fixed")) {
            jQNewLook('.sidebar-option', panel).val("fixed");
        }

        if (jQNewLook("body").hasClass("page-header-fixed")) {
            jQNewLook('.page-header-option', panel).val("fixed");
        }

        if (jQNewLook("body").hasClass("page-footer-fixed")) {
            jQNewLook('.page-footer-option', panel).val("fixed");
        }

        if (jQNewLook("body").hasClass("page-sidebar-reversed")) {
            jQNewLook('.sidebar-pos-option', panel).val("right");
        }

        if (jQNewLook(".page-sidebar-menu").hasClass("page-sidebar-menu-light")) {
            jQNewLook('.sidebar-style-option', panel).val("light");
        }

        if (jQNewLook(".page-sidebar-menu").hasClass("page-sidebar-menu-hover-submenu")) {
            jQNewLook('.sidebar-menu-option', panel).val("hover");
        }

        var sidebarOption = jQNewLook('.sidebar-option', panel).val();
        var headerOption = jQNewLook('.page-header-option', panel).val();
        var footerOption = jQNewLook('.page-footer-option', panel).val();
        var sidebarPosOption = jQNewLook('.sidebar-pos-option', panel).val();
        var sidebarStyleOption = jQNewLook('.sidebar-style-option', panel).val();
        var sidebarMenuOption = jQNewLook('.sidebar-menu-option', panel).val();

        jQNewLook('.layout-option, .page-header-option, .page-header-top-dropdown-style-option, .sidebar-option, .page-footer-option, .sidebar-pos-option, .sidebar-style-option, .sidebar-menu-option', panel).change(setLayout);
    };

    // handle theme style
    var setThemeStyle = function(style) {
        var file = (style === 'rounded' ? 'components-rounded' : 'components');
        file = (App.isRTL() ? file + '-rtl' : file);

        jQNewLook('#style_components').attr("href", App.getGlobalCssPath() + file + ".min.css");

        if (typeof Cookies !== "undefined") {
            Cookies.set('layout-style-option', style);
        }
    };

    return {

        //main function to initiate the theme
        init: function() {
            // handles style customer tool
            handleTheme();

            // handle layout style change
            jQNewLook('.theme-panel .layout-style-option').change(function() {
                 setThemeStyle(jQNewLook(this).val());
            });

            // set layout style from cookie
            if (typeof Cookies !== "undefined" && Cookies.get('layout-style-option') === 'rounded') {
                setThemeStyle(Cookies.get('layout-style-option'));
                jQNewLook('.theme-panel .layout-style-option').val(Cookies.get('layout-style-option'));
            }
        }
    };

}();

if (App.isAngularJsApp() === false) {
    jQuery(document).ready(function() {
       Demo.init(); // init metronic core componets
    });
}
