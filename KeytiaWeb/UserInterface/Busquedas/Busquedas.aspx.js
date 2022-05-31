var Busqueda = {
    SessionID: "",
    fnServerData: function(sSource, aoData, fnCallback) {
            var request = DSOControls.Grid.GetRequest(aoData);
            request.sSearchGlobal = $(".DSOTextBox[id$=Search_txt]").val();
            var param = { gsRequest: request, SessionID: Busqueda.SessionID };
            var options = {
                type: "POST",
                url: sSource,
                data: JSON.stringify(param),
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                async: true,
                success: function(data, textStatus, jqXHR) {
                    var response = data.d;
                    fnCallback(response);
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    DSOControls.ErrorAjax(jqXHR, textStatus, errorThrown);
                }
            };
            $.ajax(options);
    },
    fnSeachText: function() {
        $(".DSOGrid[id]").each(function() { $(this).dataTable({ bRetrieve: true }).fnDraw(); });
    },
    fnRenderFormat: function(obj, imgClass, imgSrc, sdoPostBack) {
        sdoPostBack = sdoPostBack.replace("{0}", obj.aData[0]);
        sdoPostBack = sdoPostBack.replace("{1}", obj.aData[1]);
        sdoPostBack = sdoPostBack.replace("{2}", obj.aData[2]);
        var ret = "<a href=\"javascript:" + sdoPostBack + ";\"><img class='" + imgClass + "' src='" + imgSrc + "'></a>";
        return ret;
    },
    fnUnload: function() {
        var param = { SessionID: Busqueda.SessionID };
        var options = {
            type: "POST",
            url: KeytiaMaster.appPath + "WebMethods.aspx/Busquedas_Unload",
            data: JSON.stringify(param),
            contentType: "application/json;charset=utf-8",
            async: false,
            error: DSOControls.ErrorAjax
        };
        $.ajax(options);
    }
}

$(document).ready(function() {
    Busqueda.SessionID = $("Input[type=hidden][id$=SessionID]").val();
    $(window).unload(Busqueda.fnUnload);
    $(".DSOTextBox[id$=Search_txt]").keypress(function(e) {
        if (e.which == 13) {
            e.preventDefault();
            Busqueda.fnSeachText();
        }
    });
});
