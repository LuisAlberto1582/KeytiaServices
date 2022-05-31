var KeytiaMaster = {};

/* * * * *   Toolbars   * * * * */
KeytiaMaster.Toolbar = function(id, idButton, idToolbar) {
    var me = this;

    this.id = id;
    this.button = $("#" + idButton);
    this.toolbar = $("#" + idToolbar);
    this.isVisible = false;

    this.button.mouseover(function() { me.show() });
    this.button.mouseout(function() { me.hide() });

    this.toolbar.mouseover(function() { me.show() });
    this.toolbar.mouseout(function() { me.hide() });

    if (!this.toolbar.hasClass())
        this.toolbar.addClass("toolbar");

    this.show = function() {
        this.toolbar.show();

        this.toolbar.offset({
            left: this.button.offset().left - this.toolbar.width() + this.button.width(),
            top: this.button.offset().top + this.button.height() + 2
        });

        this.isVisible = true;
    }

    this.hide = function() {
        this.isVisible = false;
        setTimeout(this.id + ".hideDef();", 100);
    }

    this.hideDef = function() {
        if (!this.isVisible)
            this.toolbar.hide();
    }
}

KeytiaMaster.Toolbar2 = function(id, pos) {
    var me = this;

    this.id = id;
    this.pos = pos;
    this.containerId = id + "Container";
    this.tabId = id + "Tab";

    this.tb = {};
    this.tbc = {};
    this.tbt = {};

    this.tbcWidth = 0;

    $(window).load(function() {
        me.tb = $("#" + me.id)
            .addClass("ui-state-default")
            .addClass("ui-widget")
            .addClass("ui-corner-bl")
            .wrap("<div id='" + me.containerId + "' style='position:absolute;' />")
            .css("display", "block");

        me.tbc = $("#" + me.containerId);
        me.tbc.css("top", (-me.tb.height() - 2) + "px")
            .css("left", ($(window).width() - me.tbc.width() - me.pos) + "px")
            .append("<div id='" + me.tabId + "' class='ui-widget ui-state-default ui-corner-bottom ui-icon ui-icon-triangle-1-s' style='float:right;border-top:none'></div>");

        me.tbt = $("#" + me.tabId);

        me.tbcWidth = me.tbc.width();
        //alert(me.tbcWidth);

        me.tbt.click(function() {
            if (KeytiaMaster.Toolbar2ZIndex === undefined)
                KeytiaMaster.Toolbar2ZIndex = 100;

            if (me.tbc.css("top") === "-1px") //Barra está visible
                me.tbc.animate({ top: me.tbt.height() - me.tbc.height(), zIndex: 100 }, 500);
            else //Barra está invisible
            {
                me.tbc.css("z-index", KeytiaMaster.Toolbar2ZIndex++);
                me.tbc.animate({ top: -1 }, 500);
            }
        });

        /* Rollback El toolbar de opciones se vuelve a ocultar automaticamente RZ-20120822 
        El toolbar de opción se muestra cada que carga la página. RZ-20120607
        me.tbc.css("z-index", KeytiaMaster.Toolbar2ZIndex++);
        me.tbc.animate({ top: -1 }, 500); */

        $(window).resize(function() {
            //alert(me.tbc.width());
            me.tbc.width(me.tbcWidth);
            me.tbc.css("left", $(window).width() - me.tbc.width() - me.pos);
        });
    });
}

KeytiaMaster.ToolbarButton = function(idButton, idLabel, fnClick) {
    var me = this;

    this.text = "";

    this.button = $("#" + idButton);
    this.label = $("#" + idLabel);

    this.button.mouseover(function() { me.label.html(me.text); });
    this.button.mouseout(function() { me.label.html("&nbsp;"); });

    if (!this.button.hasClass())
        this.button.addClass("toolbarButton");

    this.label.html("&nbsp;");
    if (!this.label.hasClass())
        this.label.addClass("toolbarLabel");

    if (fnClick !== undefined)
        this.button.click(fnClick);
}

// * * * * *   Cambio de Password   * * * * *
KeytiaMaster.Password = function(wndPassword) {
    this.wndPassword = wndPassword;
    this.change = function(currentPassword, newPassword, newPassword2) {
        var options =
        {
            url: KeytiaMaster.appPath + "WebMethods.aspx/ChangePassword",
            data: "{lsCurrentPassword: '" + $("#" + currentPassword).val() + "'," +
                    "lsNewPassword: '" + $("#" + newPassword).val() + "'," +
                    "lsNewPasswordConf: '" + $("#" + newPassword2).val() + "'," +
                    "wnd: '" + this.wndPassword + "'}",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: this.changeSuccess,
            error: DSOControls.ErrorAjax
        };
        $("#" + currentPassword).val('');
        $("#" + newPassword).val('');
        $("#" + newPassword2).val('');
        $.ajax(options);
    }

    this.changeSuccess = function(msg) {
        var o = eval("(" + msg.d + ")");
        jAlert(o.message);

        if (o.error === 0)
            $("#" + o.wnd).closeWindow(false);
    }

    this.cancel = function(currentPassword, newPassword, newPassword2) {
        $("#" + currentPassword).val('');
        $("#" + newPassword).val('');
        $("#" + newPassword2).val('');

        $("#" + this.wndPassword).closeWindow(false);
    }
}

KeytiaMaster.getCookie = function(name) {
    var lsCookies = document.cookie;
    var laCookies = lsCookies.split(";");
    var i;
    var ret = "";

    for (i = 0; i < laCookies.length; i++) {
        laCookie = laCookies[i].split("=");

        if (laCookie[0] === name) {
            if (laCookie.length > 1)
                ret = laCookie[1];
            break;
        }
    }

    return ret;
}

KeytiaMaster.setCookie = function(name, value, minutes) {
    var date = new Date();

    date.setTime(date.getTime() + (minutes * 60 * 1000));
    document.cookie = name + "=" + value + ";SameSite=Lax; expires=" + date.toGMTString() + "; path=/";
}

KeytiaMaster.showUserName = function(message) {
    var tb = $(".fg-toolbar");

    if (tb.length > 0) {
        if ($(tb[0]).height() === 0)
            $(tb[0]).height(25);

        KeytiaMaster.showUserName2(message);
        setTimeout("KeytiaMaster.showUserName2('');", 2000);
    }
}

KeytiaMaster.showUserName2 = function(message) {
    var tb = $(".fg-toolbar");

    if (tb.length > 0) {
        $(tb[0]).append(
            "<table cellpadding='0' cellspacing='0' border='0' style='float:right;'>" +
            "<tr><td style='height:" + $(tb[0]).height() + "px;" +
				(message === "" ? "" : "padding-left:5px;padding-right:5px;") +
				"font-weight:bold;'>" + message + "</td></tr>" +
            "</table>");
    }
}