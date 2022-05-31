//DSONavegador-------------------------------------------------------------------------
var DSONavegador = {
    Init: function() {
        var direction = $(".DSONavegadorH").attr("direction");
        $(".DSONavegadorH > li").smoothMenu({ delay: 1000, duration: 500, zIndex: 1, direction: direction });
        $(".DSONavegadorH").show("slow");
    }
};
$(document).ready(function() {
    DSONavegador.Init();
});