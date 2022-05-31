jQNewLook(document).ready(function(){

  //Para mas informacion
  //https://silviomoreto.github.io/bootstrap-select/options/

  jQNewLook('.selectpicker').selectpicker({
    style: 'btn-defult',
    dropupAuto: true,
    liveSearch: true, //Muestra un input donde podemos hacer busqueda en la lista desplegable
    language: 'ES'
  });

  //obtener la informacion seleccionada
  var valorSeleccionado = jQNewLook('#drowpdown1').selectpicker('val');

  /*
  Se ejecuta cuando se va a mostrar la Lista
  ('#drowpdown1').on('show.bs.select', function (e) {
    // do something...
  });

  Se ejecuta cuando se oculta la Lista
  ('#drowpdown1').on('hide.bs.select', function (e) {
    // do something...
  });
  */
});
