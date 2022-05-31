jQNewLook(document).ready(function(){

  // Limitar el Rango de fecha tres años antes

  var yearMin = new Date().getFullYear()-3;
  var yearMax = new Date().getFullYear()+1;
  var dateMinDate = "01/01/" + yearMin.toString();
  var dateMaxDate = "01/01/" + yearMax.toString();

  jQNewLook('input.dateRangeForm').daterangepicker({
    "linkedCalendars": false,
    "startDate": '02/09/2018',
    "endDate": '07/11/2018',
    "autoApply" : true,
    "autoUpdateInput": true,
    "showDropdowns": true,
    "minDate" : dateMinDate,
    "maxDate" : dateMaxDate,
    "opens": "center",
    "locale": {
      "format": "DD/MM/YYYY",
      "separator": " - ",
      "applyLabel": "Apply",
      "cancelLabel": "Cancel",
      "fromLabel": "From",
      "toLabel": "To",
      "customRangeLabel": "Custom",
      "daysOfWeek": ["Do","Lu","Ma","Mi","Ju","Vi","Sa"],
      "monthNames": ["Enero","Febrero","Marzo","Abril","Mayo","Junio","Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"],
      "firstDay": 1
    }
  });

  //Formatear la informacion del input colicada al inicio con la funcion de datepicker
  if (jQNewLook("input.dateRangeForm").val() != undefined) {
      var initDate = jQNewLook("input.dateRangeForm").val();
      var startDate = initDate.split("-")[0];
      var endDateI = initDate.split("-")[1];
      formatTheDate(startDate, endDateI);
  }

  //Cada que se le da clic en la fecha final en el daterangepicker se coloca la fecha en el input
  jQNewLook('input.dateRangeForm').on('apply.daterangepicker', function(ev, picker) {
      formatTheDate(picker.startDate.format("DD/MM/YYYY"), picker.endDate.format("DD/MM/YYYY"));
  });

  //Si cambia la informacion del input se vuelve a formatear, por un error que existe al no aplicar una fecha
  jQNewLook('input.dateRangeForm').on('hide.daterangepicker', function(ev, picker) {
      jQNewLook(".shadow").attr("style","display:none");
      formatTheDate(startDate,endDateI);
  });

  //Detecta cuando se muestra el calendario en la ventana
  jQNewLook('input.dateRangeForm').on('show.daterangepicker',function(ev, picker){

    //Cambia el icono correcto a los inputs que muestra el calendario
    jQNewLook(".daterangepicker").find(".calendar.left").find(".daterangepicker_input").find("svg").remove();
    jQNewLook(".daterangepicker").find(".calendar.left").find(".daterangepicker_input").append('<i class="far fa-calendar-alt"></i>');

    jQNewLook(".daterangepicker").find(".calendar.right").find(".daterangepicker_input").find("svg").remove();
    jQNewLook(".daterangepicker").find(".calendar.right").find(".daterangepicker_input").append('<i class="far fa-calendar-alt"></i>');

    jQNewLook(".daterangepicker").find(".calendar.left").find(".daterangepicker_input").find("input").attr("readonly",true);
    jQNewLook(".daterangepicker").find(".calendar.right").find(".daterangepicker_input").find("input").attr("readonly",true);

    //Muestra la sombra en el area de contenido de la pagina
    jQNewLook(".shadow").attr("style","display:block");
  });

  //Si se hace mas chica o mas grande la pagina se formatea el nombre del mes, cambiando de abrebiado a normal
  jQNewLook(window).resize(function () {
      if (jQNewLook("input.dateRangeForm").val() != undefined) {
          var startDate = jQNewLook('input.dateRangeForm').data('daterangepicker').startDate.format("DD/MM/YYYY");
          var endDate = jQNewLook('input.dateRangeForm').data('daterangepicker').endDate.format("DD/MM/YYYY");
          formatTheDate(startDate, endDate);
      }
  });

  /**
   * Crea el tipo de formato que se presentara el input del Date Range. Septiembre 01, 2018 - Marzo 01, 2019
   *
   * @param {Date} startDate
   * @param {Date} endDate
   */
  function formatTheDate(startDate, endDate) {
    var monthsNames = [ {name: "Enero",abbreviatedName:"Ene", number: 1 }, {name: "Febrero", abbreviatedName:"Feb", number: 2 }, {name: "Marzo", abbreviatedName:"Mar", number: 3 }, {name: "Abril", abbreviatedName:"Abr", number: 4 }, {name: "Mayo", abbreviatedName:"May", number: 5 }, {name: "Junio", abbreviatedName:"Jun", number: 6 }, {name: "Julio", abbreviatedName:"Jul", number: 7 }, {name: "Agosto", abbreviatedName:"Ago", number: 8 }, {name: "Septiembre", abbreviatedName:"Sep", number: 9 }, {name: "Octubre", abbreviatedName:"Oct", number: 10 }, {name: "Noviembre", abbreviatedName:"Nov", number: 11 }, {name: "Diciembre", abbreviatedName:"Dic", number: 12 }];
    var monthStart  = startDate.split("/")[1];
    var monthEnd    = endDate.split("/")[1];

    var caseNameMonth = (jQNewLook(window).width() <= 460) ? "AbbreviatedName" : "CorrectName";
    switch (caseNameMonth) {
      case "CorrectName":
          //Bucamos el mes numeral en el arreglo de monthsNames, para obtener el nombre del mes
          for (var i = 0; i < monthsNames.length; i++) {
            if(monthsNames[i].number == parseInt(monthStart)){
              monthStart = monthsNames[i].name;
              break;
            }
          }

          //Bucamos el mes numeral en el arreglo de monthsNames, para obtener el nombre del mes
          for (var i = 0; i < monthsNames.length; i++) {
            if(monthsNames[i].number == parseInt(monthEnd)){
              monthEnd = monthsNames[i].name;
              break;
            }
          }
      break;

      case "AbbreviatedName":
          //Bucamos el mes numeral en el arreglo de monthsNames, para obtener el nombre del mes
          for (var i = 0; i < monthsNames.length; i++) {
            if(monthsNames[i].number == parseInt(monthStart)){
              monthStart = monthsNames[i].abbreviatedName;
              break;
            }
          }

          //Bucamos el mes numeral en el arreglo de monthsNames, para obtener el nombre del mes
          for (var i = 0; i < monthsNames.length; i++) {
            if(monthsNames[i].number == parseInt(monthEnd)){
              monthEnd = monthsNames[i].abbreviatedName;
              break;
            }
          }
      break;
    }

    //Colocamos la informacion en el input del date range
    jQNewLook("input.dateRangeForm").val(monthStart + " " + startDate.split("/")[0] + ", " + startDate.split("/")[2] + " - " + monthEnd + " " + endDate.split("/")[0] + ", " + endDate.split("/")[2]);
  }
});
