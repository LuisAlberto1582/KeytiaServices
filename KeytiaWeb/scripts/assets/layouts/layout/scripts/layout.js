/**
Core script to handle the entire theme and core functions
**/
var Layout = function () {

    var layoutImgPath = 'layouts/layout/img/';

    var layoutCssPath = 'layouts/layout/css/';

    var resBreakpointMd = App.getResponsiveBreakpoint('md');

    var ajaxContentSuccessCallbacks = [];
    var ajaxContentErrorCallbacks = [];

    //* BEGIN:CORE HANDLERS *//
    // this function handles responsive layout on screen size resize or mobile device rotate.

    // Set proper height for sidebar and content. The content and sidebar height must be synced always.
    var handleSidebarAndContentHeight = function () {
        var content = jQNewLook('.page-content');
        var sidebar = jQNewLook('.page-sidebar');
        var body = jQNewLook('body');
        var height;

        if (body.hasClass("page-footer-fixed") === true && body.hasClass("page-sidebar-fixed") === false) {
            var available_height = App.getViewPort().height - jQNewLook('.page-footer').outerHeight() - jQNewLook('.page-header').outerHeight();
            var sidebar_height = sidebar.outerHeight();
            if (sidebar_height > available_height) {
                available_height = sidebar_height + jQNewLook('.page-footer').outerHeight();
            }
            if (content.height() < available_height) {
                content.attr('style', 'min-height:' + available_height + 'px');
            }
        } else {
            if (body.hasClass('page-sidebar-fixed')) {
                height = _calculateFixedSidebarViewportHeight();
                if (body.hasClass('page-footer-fixed') === false) {
                    height = height - jQNewLook('.page-footer').outerHeight();
                }
            } else {
                var headerHeight = jQNewLook('.page-header').outerHeight();
                var footerHeight = jQNewLook('.page-footer').outerHeight();

                if (App.getViewPort().width < resBreakpointMd) {
                    height = App.getViewPort().height - headerHeight - footerHeight;
                } else {
                    height = sidebar.height() + 20;
                }

                if ((height + headerHeight + footerHeight) <= App.getViewPort().height) {
                    height = App.getViewPort().height - headerHeight - footerHeight;
                }
            }
            content.attr('style', 'min-height:' + height + 'px');
        }
    };

    // Handle sidebar menu links
    var handleSidebarMenuActiveLink = function(mode, el) {
        var url = location.hash.toLowerCase();
        
        var menu = jQNewLook('.page-sidebar-menu');

        if (mode === 'click' || mode === 'set') {
            el = jQNewLook(el);
        } else if (mode === 'match') {
            menu.find("li > a").each(function() {
                var path = jQNewLook(this).attr("href").toLowerCase();
                // url match condition
                if (path.length > 1 && url.substr(1, path.length - 1) == path.substr(1)) {
                    el = jQNewLook(this);
                    return;
                }
            });
        }

        if (!el || el.size() == 0) {
            return;
        }

        if (el.attr('href').toLowerCase() === 'javascript:;' || el.attr('href').toLowerCase() === '#') {
            return;
        }

        var slideSpeed = parseInt(menu.data("slide-speed"));
        var keepExpand = menu.data("keep-expanded");

        // begin: handle active state
        if (menu.hasClass('page-sidebar-menu-hover-submenu') === false) {
            menu.find('li.nav-item.open').each(function() {
                var match = false;
                jQNewLook(this).find('li').each(function(){
                    if (jQNewLook(this).find(' > a').attr('href') === el.attr('href')) {
                        match = true;
                        return;
                    }
                });

                if (match === true) {
                    return;
                }

                jQNewLook(this).removeClass('open');
                jQNewLook(this).find('> a > .arrow.open').removeClass('open');
                jQNewLook(this).find('> .sub-menu').slideUp();
            });
        } else {
             menu.find('li.open').removeClass('open');
        }

        menu.find('li.active').removeClass('active');
        menu.find('li > a > .selected').remove();
        // end: handle active state

        el.parents('li').each(function () {
            jQNewLook(this).addClass('active');
            jQNewLook(this).find('> a > span.arrow').addClass('open');

            if (jQNewLook(this).parent('ul.page-sidebar-menu').size() === 1) {
                jQNewLook(this).find('> a').append('<span class="selected"></span>');
            }

            if (jQNewLook(this).children('ul.sub-menu').size() === 1) {
                jQNewLook(this).addClass('open');
            }
        });

        if (mode === 'click') {
            if (App.getViewPort().width < resBreakpointMd && jQNewLook('.page-sidebar').hasClass("in")) { // close the menu on mobile view while laoding a page
                jQNewLook('.page-header .responsive-toggler').click();
            }
        }
    };

    // Handle sidebar menu
    var handleSidebarMenu = function () {
        // offcanvas mobile menu
        jQNewLook('.page-sidebar-mobile-offcanvas .responsive-toggler').click(function() {
            jQNewLook('body').toggleClass('page-sidebar-mobile-offcanvas-open');
            e.preventDefault();
            e.stopPropagation();
        });

        if (jQNewLook('body').hasClass('page-sidebar-mobile-offcanvas')) {
            jQNewLook(document).on('click', function(e) {
                if (jQNewLook('body').hasClass('page-sidebar-mobile-offcanvas-open')) {
                    if (jQNewLook(e.target).closest('.page-sidebar-mobile-offcanvas .responsive-toggler').length === 0 &&
                        jQNewLook(e.target).closest('.page-sidebar-wrapper').length === 0) {
                        jQNewLook('body').removeClass('page-sidebar-mobile-offcanvas-open');
                        e.preventDefault();
                        e.stopPropagation();
                    }
                }
            });
        }

        // handle sidebar link click
        jQNewLook('.page-sidebar-menu').on('click', 'li > a.nav-toggle, li > a > span.nav-toggle', function (e) {
            var that = jQNewLook(this).closest('.nav-item').children('.nav-link');

            if (App.getViewPort().width >= resBreakpointMd && !jQNewLook('.page-sidebar-menu').attr("data-initialized") && jQNewLook('body').hasClass('page-sidebar-closed') &&  that.parent('li').parent('.page-sidebar-menu').size() === 1) {
                return;
            }

            var hasSubMenu = that.next().hasClass('sub-menu');

            if (App.getViewPort().width >= resBreakpointMd && that.parents('.page-sidebar-menu-hover-submenu').size() === 1) { // exit of hover sidebar menu
                return;
            }

            if (hasSubMenu === false) {
                if (App.getViewPort().width < resBreakpointMd && jQNewLook('.page-sidebar').hasClass("in")) { // close the menu on mobile view while laoding a page
                    jQNewLook('.page-header .responsive-toggler').click();
                }
                return;
            }

            var parent =that.parent().parent();
            var the = that;
            var menu = jQNewLook('.page-sidebar-menu');
            var sub = that.next();

            var autoScroll = menu.data("auto-scroll");
            var slideSpeed = parseInt(menu.data("slide-speed"));
            var keepExpand = menu.data("keep-expanded");

            if (!keepExpand) {
                parent.children('li.open').children('a').children('.arrow').removeClass('open');
                parent.children('li.open').children('.sub-menu:not(.always-open)').slideUp(slideSpeed);
                parent.children('li.open').removeClass('open');
            }

            var slideOffeset = -200;

            if (sub.is(":visible")) {
                jQNewLook('.arrow', the).removeClass("open");
                the.parent().removeClass("open");
                sub.slideUp(slideSpeed, function () {
                    if (autoScroll === true && jQNewLook('body').hasClass('page-sidebar-closed') === false) {
                        if (jQNewLook('body').hasClass('page-sidebar-fixed')) {
                            menu.slimScroll({
                                'scrollTo': (the.position()).top
                            });
                        } else {
                            App.scrollTo(the, slideOffeset);
                        }
                    }
                    handleSidebarAndContentHeight();
                });
            } else if (hasSubMenu) {
                jQNewLook('.arrow', the).addClass("open");
                the.parent().addClass("open");
                sub.slideDown(slideSpeed, function () {
                    if (autoScroll === true && jQNewLook('body').hasClass('page-sidebar-closed') === false) {
                        if (jQNewLook('body').hasClass('page-sidebar-fixed')) {
                            menu.slimScroll({
                                'scrollTo': (the.position()).top
                            });
                        } else {
                            App.scrollTo(the, slideOffeset);
                        }
                    }
                    handleSidebarAndContentHeight();
                });
            }

            e.preventDefault();
        });

        // handle menu close for angularjs version
        if (App.isAngularJsApp()) {
            jQNewLook(".page-sidebar-menu li > a").on("click", function(e) {
                if (App.getViewPort().width < resBreakpointMd && jQNewLook(this).next().hasClass('sub-menu') === false) {
                    jQNewLook('.page-header .responsive-toggler').click();
                }
            });
        }

        // handle ajax links within sidebar menu
        jQNewLook('.page-sidebar').on('click', ' li > a.ajaxify', function (e) {
            e.preventDefault();
            App.scrollTop();
            var url = jQNewLook(this).attr("href");
            var menuContainer = jQNewLook('.page-sidebar ul');

            menuContainer.children('li.active').removeClass('active');
            menuContainer.children('arrow.open').removeClass('open');

            jQNewLook(this).parents('li').each(function () {
                jQNewLook(this).addClass('active');
                jQNewLook(this).children('a > span.arrow').addClass('open');
            });
            jQNewLook(this).parents('li').addClass('active');

            if (App.getViewPort().width < resBreakpointMd && jQNewLook('.page-sidebar').hasClass("in")) { // close the menu on mobile view while laoding a page
                jQNewLook('.page-header .responsive-toggler').click();
            }

            Layout.loadAjaxContent(url, jQNewLook(this));
        });

        // handle ajax link within main content
        jQNewLook('.page-content').on('click', '.ajaxify', function (e) {
            e.preventDefault();
            App.scrollTop();

            var url = jQNewLook(this).attr("href");

            if (App.getViewPort().width < resBreakpointMd && jQNewLook('.page-sidebar').hasClass("in")) { // close the menu on mobile view while laoding a page
                jQNewLook('.page-header .responsive-toggler').click();
            }

            Layout.loadAjaxContent(url);
        });

        // handle scrolling to top on responsive menu toggler click when header is fixed for mobile view
        jQNewLook(document).on('click', '.page-header-fixed-mobile .page-header .responsive-toggler', function(){
            App.scrollTop();
        });

        // handle sidebar hover effect
        handleFixedSidebarHoverEffect();

        // handle the search bar close
        jQNewLook('.page-sidebar').on('click', '.sidebar-search .remove', function (e) {
            e.preventDefault();
            jQNewLook('.sidebar-search').removeClass("open");
        });

        // handle the search query submit on enter press
        jQNewLook('.page-sidebar .sidebar-search').on('keypress', 'input.form-control', function (e) {
            if (e.which == 13) {
                jQNewLook('.sidebar-search').submit();
                return false; //<---- Add this line
            }
        });

        // handle the search submit(for sidebar search and responsive mode of the header search)
        jQNewLook('.sidebar-search .submit').on('click', function (e) {
            e.preventDefault();
            if (jQNewLook('body').hasClass("page-sidebar-closed")) {
                if (jQNewLook('.sidebar-search').hasClass('open') === false) {
                    if (jQNewLook('.page-sidebar-fixed').size() === 1) {
                        jQNewLook('.page-sidebar .sidebar-toggler').click(); //trigger sidebar toggle button
                    }
                    jQNewLook('.sidebar-search').addClass("open");
                } else {
                    jQNewLook('.sidebar-search').submit();
                }
            } else {
                jQNewLook('.sidebar-search').submit();
            }
        });

        // handle close on body click
        if (jQNewLook('.sidebar-search').size() !== 0) {
            jQNewLook('.sidebar-search .input-group').on('click', function(e){
                e.stopPropagation();
            });

            jQNewLook('body').on('click', function() {
                if (jQNewLook('.sidebar-search').hasClass('open')) {
                    jQNewLook('.sidebar-search').removeClass("open");
                }
            });
        }
    };

    // Helper function to calculate sidebar height for fixed sidebar layout.
    var _calculateFixedSidebarViewportHeight = function () {
        var sidebarHeight = App.getViewPort().height - jQNewLook('.page-header').outerHeight(true);
        if (jQNewLook('body').hasClass("page-footer-fixed")) {
            sidebarHeight = sidebarHeight - jQNewLook('.page-footer').outerHeight();
        }

        return sidebarHeight;
    };

    // Handles fixed sidebar
    var handleFixedSidebar = function () {
        var menu = jQNewLook('.page-sidebar-menu');

        handleSidebarAndContentHeight();

        if (jQNewLook('.page-sidebar-fixed').size() === 0) {
            return;
        }

        if (App.getViewPort().width >= resBreakpointMd && !jQNewLook('body').hasClass('page-sidebar-menu-not-fixed')) {
             menu.attr("data-height", _calculateFixedSidebarViewportHeight());
            App.destroySlimScroll(menu);
            App.initSlimScroll(menu);
            handleSidebarAndContentHeight();
        }
    };

    // Handles sidebar toggler to close/hide the sidebar.
    var handleFixedSidebarHoverEffect = function () {
        var body = jQNewLook('body');
        if (body.hasClass('page-sidebar-fixed')) {
            jQNewLook('.page-sidebar').on('mouseenter', function () {
                if (body.hasClass('page-sidebar-closed')) {
                    jQNewLook(this).find('.page-sidebar-menu').removeClass('page-sidebar-menu-closed');
                }
            }).on('mouseleave', function () {
                if (body.hasClass('page-sidebar-closed')) {
                    jQNewLook(this).find('.page-sidebar-menu').addClass('page-sidebar-menu-closed');
                }
            });
        }
    };

    // Hanles sidebar toggler
    var handleSidebarToggler = function () {
        var body = jQNewLook('body');
        if (jQNewLook.cookie && jQNewLook.cookie('sidebar_closed') === '1' && App.getViewPort().width >= resBreakpointMd) {
            jQNewLook('body').addClass('page-sidebar-closed');
            jQNewLook('.page-sidebar-menu').addClass('page-sidebar-menu-closed');
        }

        // handle sidebar show/hide
        jQNewLook('body').on('click', '.sidebar-toggler', function (e) {
            var sidebar = jQNewLook('.page-sidebar');
            var sidebarMenu = jQNewLook('.page-sidebar-menu');
            jQNewLook(".sidebar-search", sidebar).removeClass("open");

            if (body.hasClass("page-sidebar-closed")) {
                body.removeClass("page-sidebar-closed");
                sidebarMenu.removeClass("page-sidebar-menu-closed");
                if (jQNewLook.cookie) {
                    jQNewLook.cookie('sidebar_closed', '0');
                }
            } else {
                body.addClass("page-sidebar-closed");
                sidebarMenu.addClass("page-sidebar-menu-closed");
                if (body.hasClass("page-sidebar-fixed")) {
                    sidebarMenu.trigger("mouseleave");
                }
                if (jQNewLook.cookie) {
                    jQNewLook.cookie('sidebar_closed', '1');
                }
            }

            jQNewLook(window).trigger('resize');
        });
    };

    // Handles the horizontal menu
    var handleHorizontalMenu = function () {
        //handle tab click
        jQNewLook('.page-header').on('click', '.hor-menu a[data-toggle="tab"]', function (e) {
            e.preventDefault();
            var nav = jQNewLook(".hor-menu .nav");
            var active_link = nav.find('li.current');
            jQNewLook('li.active', active_link).removeClass("active");
            jQNewLook('.selected', active_link).remove();
            var new_link = jQNewLook(this).parents('li').last();
            new_link.addClass("current");
            new_link.find("a:first").append('<span class="selected"></span>');
        });

        // handle search box expand/collapse
        jQNewLook('.page-header').on('click', '.search-form', function (e) {
            jQNewLook(this).addClass("open");
            jQNewLook(this).find('.form-control').focus();

            jQNewLook('.page-header .search-form .form-control').on('blur', function (e) {
                jQNewLook(this).closest('.search-form').removeClass("open");
                jQNewLook(this).unbind("blur");
            });
        });

        // handle hor menu search form on enter press
        jQNewLook('.page-header').on('keypress', '.hor-menu .search-form .form-control', function (e) {
            if (e.which == 13) {
                jQNewLook(this).closest('.search-form').submit();
                return false;
            }
        });

        // handle header search button click
        jQNewLook('.page-header').on('mousedown', '.search-form.open .submit', function (e) {
            e.preventDefault();
            e.stopPropagation();
            jQNewLook(this).closest('.search-form').submit();
        });


        jQNewLook(document).on('click', '.mega-menu-dropdown .dropdown-menu', function (e) {
            e.stopPropagation();
        });
    };

    // Handles Bootstrap Tabs.
    var handleTabs = function () {
        // fix content height on tab click
        jQNewLook('body').on('shown.bs.tab', 'a[data-toggle="tab"]', function () {
            handleSidebarAndContentHeight();
        });
    };

    // Handles the go to top button at the footer
    var handleGoTop = function () {
        var offset = 300;
        var duration = 500;

        if (navigator.userAgent.match(/iPhone|iPad|iPod/i)) {  // ios supported
            jQNewLook(window).bind("touchend touchcancel touchleave", function(e){
               if (jQNewLook(this).scrollTop() > offset) {
                    jQNewLook('.scroll-to-top').fadeIn(duration);
                } else {
                    jQNewLook('.scroll-to-top').fadeOut(duration);
                }
            });
        } else {  // general
            jQNewLook(window).scroll(function() {
                if (jQNewLook(this).scrollTop() > offset) {
                    jQNewLook('.scroll-to-top').fadeIn(duration);
                } else {
                    jQNewLook('.scroll-to-top').fadeOut(duration);
                }
            });
        }

        jQNewLook('.scroll-to-top').click(function(e) {
            e.preventDefault();
            jQNewLook('html, body').animate({scrollTop: 0}, duration);
            return false;
        });
    };

    // Hanlde 100% height elements(block, portlet, etc)
    var handle100HeightContent = function () {

        jQNewLook('.full-height-content').each(function(){
            var target = jQNewLook(this);
            var height;

            height = App.getViewPort().height -
                jQNewLook('.page-header').outerHeight(true) -
                jQNewLook('.page-footer').outerHeight(true) -
                jQNewLook('.page-title').outerHeight(true) -
                jQNewLook('.page-bar').outerHeight(true);

            if (target.hasClass('portlet')) {
                var portletBody = target.find('.portlet-body');

                App.destroySlimScroll(portletBody.find('.full-height-content-body')); // destroy slimscroll

                height = height -
                    target.find('.portlet-title').outerHeight(true) -
                    parseInt(target.find('.portlet-body').css('padding-top')) -
                    parseInt(target.find('.portlet-body').css('padding-bottom')) - 5;

                if (App.getViewPort().width >= resBreakpointMd && target.hasClass("full-height-content-scrollable")) {
                    height = height - 35;
                    portletBody.find('.full-height-content-body').css('height', height);
                    App.initSlimScroll(portletBody.find('.full-height-content-body'));
                } else {
                    portletBody.css('min-height', height);
                }
            } else {
               App.destroySlimScroll(target.find('.full-height-content-body')); // destroy slimscroll

                if (App.getViewPort().width >= resBreakpointMd && target.hasClass("full-height-content-scrollable")) {
                    height = height - 35;
                    target.find('.full-height-content-body').css('height', height);
                    App.initSlimScroll(target.find('.full-height-content-body'));
                } else {
                    target.css('min-height', height);
                }
            }
        });
    };
    //* END:CORE HANDLERS *//

    return {
        // Main init methods to initialize the layout
        //IMPORTANT!!!: Do not modify the core handlers call order.

        initHeader: function() {
            handleHorizontalMenu(); // handles horizontal menu
        },

        setSidebarMenuActiveLink: function(mode, el) {
            handleSidebarMenuActiveLink(mode, el);
        },

        initSidebar: function() {
            //layout handlers
            handleFixedSidebar(); // handles fixed sidebar menu
            handleSidebarMenu(); // handles main menu
            handleSidebarToggler(); // handles sidebar hide/show

            if (App.isAngularJsApp()) {
                handleSidebarMenuActiveLink('match'); // init sidebar active links
            }

            App.addResizeHandler(handleFixedSidebar); // reinitialize fixed sidebar on window resize
        },

        initContent: function() {
            handle100HeightContent(); // handles 100% height elements(block, portlet, etc)
            handleTabs(); // handle bootstrah tabs

            App.addResizeHandler(handleSidebarAndContentHeight); // recalculate sidebar & content height on window resize
            App.addResizeHandler(handle100HeightContent); // reinitialize content height on window resize
        },

        initFooter: function() {
            handleGoTop(); //handles scroll to top functionality in the footer
        },

        init: function () {
            this.initHeader();
            this.initSidebar();
            this.initContent();
            this.initFooter();
        },

        loadAjaxContent: function(url, sidebarMenuLink) {
            var pageContent = jQNewLook('.page-content .page-content-body');

            App.startPageLoading({animate: true});

            jQNewLook.ajax({
                type: "GET",
                cache: false,
                url: url,
                dataType: "html",
                success: function (res) {
                    App.stopPageLoading();

                    for (var i = 0; i < ajaxContentSuccessCallbacks.length; i++) {
                        ajaxContentSuccessCallbacks[i].call(res);
                    }

                    if (sidebarMenuLink.size() > 0 && sidebarMenuLink.parents('li.open').size() === 0) {
                        jQNewLook('.page-sidebar-menu > li.open > a').click();
                    }

                    pageContent.html(res);
                    Layout.fixContentHeight(); // fix content height
                    App.initAjax(); // initialize core stuff
                },
                error: function (res, ajaxOptions, thrownError) {
                    App.stopPageLoading();
                    pageContent.html('<h4>Could not load the requested content.</h4>');

                    for (var i = 0; i < ajaxContentErrorCallbacks.length; i++) {
                        ajaxContentSuccessCallbacks[i].call(res);
                    }
                }
            });
        },

        addAjaxContentSuccessCallback: function(callback) {
            ajaxContentSuccessCallbacks.push(callback);
        },

        addAjaxContentErrorCallback: function(callback) {
            ajaxContentErrorCallbacks.push(callback);
        },

        //public function to fix the sidebar and content height accordingly
        fixContentHeight: function () {
            handleSidebarAndContentHeight();
        },

        initFixedSidebarHoverEffect: function() {
            handleFixedSidebarHoverEffect();
        },

        initFixedSidebar: function() {
            handleFixedSidebar();
        },

        getLayoutImgPath: function () {
            return App.getAssetsPath() + layoutImgPath;
        },

        getLayoutCssPath: function () {
            return App.getAssetsPath() + layoutCssPath;
        }
    };

}();

if (App.isAngularJsApp() === false) {
    jQuery(document).ready(function() {
       Layout.init(); // init metronic core componets
    });
}
