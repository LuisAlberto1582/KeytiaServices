$(document).ready(function(){

  var pieChartDashboard = [];
  /*----------------------------------------------------------------------------- Configuraciones de los graficos (No mover) -----------------------------------------------------------------------------*/
  var optionsLineChart       = { legend: { display: false }, scales: { yAxes: [{ ticks: { beginAtZero:true, fontFamily: "Poppins", fontSize:10  } } ], xAxes: [{ticks:{fontFamily: "Poppins", fontSize: 10, fontWeight: 100}}]}, tooltips: { callbacks: { title: function(tooltipItem,data){ return ""; }, label: function(tooltipItem, data) { return tooltipItem.yLabel; } }, displayColors: false, bodyFontStyle:"Poppins" },};
  var optionsColumnsChart    = { legend: { display: false }, scales: { xAxes: [{ categorySpacing: 60, ticks:{ fontFamily: "Poppins", fontSize:10  } }], yAxes: [{ type: "linear", display: true, position: "left", id: "y-axis-1", ticks:{ fontFamily: "Poppins", fontSize:10  } }] }, tooltips: { callbacks: { title: function(tooltipItem, data) { return ""; }, label: function(tooltipItem, data) { return tooltipItem.yLabel; } }, displayColors: false, bodyFontStyle:"Poppins" } };
  var optionsHorizontalChart = {legend:{display:false},scales:{xAxes:[{ticks:{callback:function(t,o,n){return"$"+formatoMoneda(t)},fontFamily:"Poppins",fontSize:10}}],yAxes:[{stacked:!0,ticks:{fontFamily:"Poppins",fontSize:10}}]},tooltips:{mode:"y",axis:"x",callbacks:{title:function(t,o){return""},label:function(t,o){return"$"+t.xLabel},afterLabel:function(t,o){return""}},displayColors:!1},layout:{padding:{left:15,top:0,bottom:0,right:50}}};
  var optionsAreaChart       = { legend: { display: false }, showLines: true, scales: { yAxes: [{ ticks: { beginAtZero:true, fontFamily: "Poppins", fontSize:10  } } ], xAxes: [{ticks:{fontFamily: "Poppins", fontSize: 10, fontWeight: 100}}]}, tooltips: { callbacks: { title: function(tooltipItem,data){ return ""; }, label: function(tooltipItem, data) { return tooltipItem.yLabel; } }, displayColors: false , bodyFontStyle:"Poppins"} };
  var optionsPieChart        = { legend: { position: "right", labels: { padding : 20, boxWidth: 10, fontFamily: "Poppins", fontSize : 12, fontWeight : 100 } },tooltips: { displayColors: false } };
  /*----------------------------------------------------------------------------- Configuraciones para los graficos -----------------------------------------------------------------------------*/


  /*----------------------------------------------------------------------------- DataSet para cada tipo de grafica -----------------------------------------------------------------------------*/
  var dataLineChart = {
      labels: [["Enero" ,"2018"], ["Febrero", "2018"], ["Marzo","2018"], ["Abril","2018"], ["Mayo","2018"], ["Junio","2018"], ["Julio","2018"]],
      datasets: [{ data: [12, 19, 3, 5, 2, 3, 13], backgroundColor: ['rgba(204, 204, 204, 0.4)'], borderColor: [ '#56579C'], borderWidth: 1, lineTension:0, pointBackgroundColor: "#C0C1C0", pointRadius: 6, pointHoverRadius: 6}]
  };

  var dataColumnsChart = {
      labels: [["Enero" ,"2018"], ["Febrero", "2018"], ["Marzo","2018"], ["Abril","2018"], ["Mayo","2018"], ["Junio","2018"], ["Julio","2018"],["Septiembre","2018"]],
      datasets: [{ label: "Test", data: [23,46,34,56,43,65,100,43], backgroundColor: ["rgba(245, 132, 38, 0.8)", "rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)"], hoverBackgroundColor: ["rgba(245, 132, 38, 0.8)", "rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)"] }]
  };

  var dataHorizontalBarChart = {
      labels: [ "Telcel", "Celular Local", "Servicio Medio", "L.D. Mundial", "Local Nacional", "Celular Nacional", "Llamadas 01800", "L.D. Nacional", "Enlace", "Entrada"],
      datasets: [{ label: "Test", data: [12000,10000,93000,400000, 300000,30000,20000,10200,80000,50000], backgroundColor: ["rgba(105, 108, 172, 0.9)", "rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)"], hoverBackgroundColor: ["rgba(105, 108, 172, 0.9)", "rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)"] }]
  };

  var dataAreaChart = {
      labels: [["Enero" ,"2018"], ["Febrero", "2018"], ["Marzo","2018"], ["Abril","2018"], ["Mayo","2018"], ["Junio","2018"], ["Julio","2018"]],
      datasets: [{ data: [12, 19, 3, 5, 2, 3, 13], backgroundColor:['rgba(245, 132, 38, 0.8)'], borderColor:['transparent'], lineTension:0, pointHoverBorderColor:"transparent", pointBackgroundColor:"transparent",}]
  };

  var dataPieChart = {
      datasets: [{ data: [10, 10, 10, 10, 10, 10, 10, 10, 10, 10], backgroundColor: ["#FBE599", "#FBD85C","#F6CC48", "#F58426", "#F56F00", "#DE6500", "#DE4600", "#BA2E00", "#7D1F00", "#521400"], hoverBackgroundColor: ["#FBE599", "#FBD85C","#F6CC48", "#F58426", "#F56F00", "#DE6500", "#DE4600", "#BA2E00", "#7D1F00", "#521400"] }],
      labels: ['No identificada','Laboral','Personal', "Color 4" , "Color 5", "Color 6", "Color 7", "Color 8", "Color 9", "Color 10"]
  };
  /*----------------------------------------------------------------------------- / DataSet para cada tipo de grafica -----------------------------------------------------------------------------*/


   /*-------------------------------------------------- Colocar los graficos en el panel de Consumo Historico --------------------------------------------------*/
  var lineChartHC    = document.getElementById("lineHC");
  var columnsChartHC = document.getElementById("columnsHC");
  var barrasChartHC  = document.getElementById("barsHC");
  var areaChartHC    = document.getElementById("areaHC");
  var pieChartHC     = document.getElementById("pieHC");

  var lineJsHC = new Chart(lineChartHC, {
      type: 'line',
      data: dataLineChart,
      options:optionsLineChart
  });

  var columnsChartJsHC = new Chart(columnsChartHC, {
      type: 'bar',
      data: dataColumnsChart,
      options: optionsColumnsChart
  });

  var horizontalBarChartJsHC = new Chart(barrasChartHC, {
      type: 'horizontalBar',
      data: dataHorizontalBarChart,
      options: optionsHorizontalChart
  });

  var areaChartJsHC = new Chart(areaChartHC, {
      type: 'line',
      data: dataAreaChart,
      options: optionsAreaChart
  });

  var pieChartJsHC = new Chart(pieChartHC,{
      type: 'pie',
      data: dataPieChart,
      options: optionsPieChart
  });

  pieChartDashboard.push(pieChartJsHC);
  /*------------------------------------------------- /Colocar los graficos en el panel de Consumo Historico --------------------------------------------------*/



  /*------------------------------------------------- /Colocar los graficos en el panel de Consumo por tipo de destino --------------------------------------------------*/

  var lineChartCTD    = document.getElementById("lineCTD");
  var columnsChartCTD = document.getElementById("columnsCTD");
  var barrasChartCTD  = document.getElementById("barsCTD");
  var areaChartCTD    = document.getElementById("areaCTD");
  var pieChartCTD     = document.getElementById("pieCTD");


  var lineJsCTD = new Chart(lineChartCTD, {
    type: 'line',
    data: dataLineChart,
    options:optionsLineChart
  });

  var columnsChartJsCTD = new Chart(columnsChartCTD, {
    type: 'bar',
    data: dataColumnsChart,
    options: optionsColumnsChart
  });

  var horizontalBarChartJsCTD = new Chart(barrasChartCTD, {
    type: 'horizontalBar',
    data: dataHorizontalBarChart,
    options: optionsHorizontalChart
  });

  var areaChartJsCTD = new Chart(areaChartCTD, {
    type: 'line',
    data: dataAreaChart,
    options: optionsAreaChart
  });

  var pieChartJsCTD = new Chart(pieChartCTD,{
    type: 'pie',
    data: dataPieChart,
    options: optionsPieChart
  });

  pieChartDashboard.push(pieChartJsCTD);
  /*------------------------------------------------- /Colocar los graficos en el panel de Consumo por tipo de destino --------------------------------------------------*/



  /*------------------------------------------------- Colocar los graficos en el panel de Consumo por sitio --------------------------------------------------*/
  var lineChartCPS    = document.getElementById("lineCPS");
  var columnsChartCPS = document.getElementById("columnsCPS");
  var barrasChartCPS  = document.getElementById("barsCPS");
  var areaChartCPS    = document.getElementById("areaCPS");
  var pieChartCPS     = document.getElementById("pieCPS");

  var lineJsCPS = new Chart(lineChartCPS,{
    type: 'line',
    data: dataLineChart,
    options:optionsLineChart
  });

  var columnsChartJsCPS = new Chart(columnsChartCPS, {
    type: 'bar',
    data: dataColumnsChart,
    options: optionsColumnsChart
  });

  var horizontalBarChartJsCPS = new Chart(barrasChartCPS, {
    type: 'horizontalBar',
    data: dataHorizontalBarChart,
    options: optionsHorizontalChart
  });

  var areaChartJsCPS = new Chart(areaChartCPS, {
    type: 'line',
    data: dataAreaChart,
    options: optionsAreaChart
  });

  var pieChartJsCPS = new Chart(pieChartCPS,{
    type: 'pie',
    data: dataPieChart,
    options: optionsPieChart
  });

  pieChartDashboard.push(pieChartJsCPS);
  /*------------------------------------------------- /Colocar los graficos en el panel de Consumo por sotio --------------------------------------------------*/



  /*------------------------------------------------- Colocar los graficos en el panel de Consumo centro de costo jerarquico --------------------------------------------------*/
  var lineChartHCC    = document.getElementById("lineHCC");
  var columnsChartHCC = document.getElementById("columnsHCC");
  var barrasChartHCC  = document.getElementById("barsHCC");
  var areaChartHCC    = document.getElementById("areaHCC");
  var pieChartHCC     = document.getElementById("pieHCC");

  var lineJsHCC = new Chart(lineChartHCC, {
    type: 'line',
    data: dataLineChart,
    options:optionsLineChart
  });

  var columnsChartJsHCC = new Chart(columnsChartHCC, {
    type: 'bar',
    data: dataColumnsChart,
    options: optionsColumnsChart
  });

  var horizontalBarChartJsHCC = new Chart(barrasChartHCC, {
    type: 'horizontalBar',
    data: dataHorizontalBarChart,
    options: optionsHorizontalChart
  });

  var areaChartJsHCC = new Chart(areaChartHCC, {
    type: 'line',
    data: dataAreaChart,
    options: optionsAreaChart
  });

  var pieChartJsHCC = new Chart(pieChartHCC,{
    type: 'pie',
    data: dataPieChart,
    options: optionsPieChart
  });

  pieChartDashboard.push(pieChartJsHCC);
  /*------------------------------------------------- /Colocar los graficos en el panel de Consumo centro de costo jerarquico --------------------------------------------------*/



  /*------------------------------------------------- Colocar los graficos en el panel de Consumo tipo de llamada --------------------------------------------------*/
  var lineChartCTP    = document.getElementById("lineCTP");
  var columnsChartCTP = document.getElementById("columnsCTP");
  var barrasChartCTP  = document.getElementById("barsCTP");
  var areaChartCTP    = document.getElementById("areaCTP");
  var pieChartCTP     = document.getElementById("pieCTP");

  var lineJsCTP = new Chart(lineChartCTP, {
    type: 'line',
    data: dataLineChart,
    options:optionsLineChart
  });

  var columnsChartJsCTP = new Chart(columnsChartCTP, {
    type: 'bar',
    data: dataColumnsChart,
    options: optionsColumnsChart
  });

  var horizontalBarChartJsCTP = new Chart(barrasChartCTP, {
    type: 'horizontalBar',
    data: dataHorizontalBarChart,
    options: optionsHorizontalChart
  });

  var areaChartJsCTP = new Chart(areaChartCTP, {
    type: 'line',
    data: dataAreaChart,
    options: optionsAreaChart
  });

  var pieChartJsCTP = new Chart(pieChartCTP,{
    type: 'pie',
    data: dataPieChart,
    options: optionsPieChart  });

  pieChartDashboard.push(pieChartJsCTP);
  /*------------------------------------------------- /Colocar los graficos en el panel de Consumo tipo de llamada --------------------------------------------------*/



  /*------------------------------------------------- Colocar los graficos en el panel de Empleado más caro --------------------------------------------------*/
  var lineChartMEE    = document.getElementById("lineMEE");
  var columnsChartMEE = document.getElementById("columnsMEE");
  var barrasChartMEE  = document.getElementById("barsMEE");
  var areaChartMEE    = document.getElementById("areaMEE");
  var pieChartMEE     = document.getElementById("pieMEE");


  var lineJsMEE = new Chart(lineChartMEE, {
    type: 'line',
    data: dataLineChart,
    options:optionsLineChart
  });

  var columnsChartJsMEE = new Chart(columnsChartMEE, {
    type: 'bar',
    data: dataColumnsChart,
    options: optionsColumnsChart
  });

  var horizontalBarChartJsMEE = new Chart(barrasChartMEE, {
    type: 'horizontalBar',
    data: dataHorizontalBarChart,
    options: optionsHorizontalChart
  });

  var areaChartJsMEE = new Chart(areaChartMEE, {
    type: 'line',
    data: dataAreaChart,
    options: optionsAreaChart
  });

  var pieChartJsMEE = new Chart(pieChartMEE,{
    type: 'pie',
    data: dataPieChart,
    options: optionsPieChart
  });

  pieChartDashboard.push(pieChartJsMEE);
  /*------------------------------------------------- /Colocar los graficos en el panel de Empleado más caro --------------------------------------------------*/

  /* ----------------------------------- Oculta el Legend de la grafica de PIE cuando el ancho de la ventana sea menos a 369 pixeles y pueda mostrarse bien la grafica ----------------------------------- */
  if($(window).width() <= 369){
    for (var i = 0; i < pieChartDashboard.length; i++) {
      pieChartDashboard[i].legend.options.display = false;
      pieChartDashboard[i].update();
    }
  }

  $(window).resize(function(){
    //Quitar la leyenda de los graficos de PIE
    if ($(this).width() <= 369) {
      for (var i = 0; i < pieChartDashboard.length; i++) {
        pieChartDashboard[i].legend.options.display = false;
        pieChartDashboard[i].update();
      }
    }else {
      for (var i = 0; i < pieChartDashboard.length; i++) {
        pieChartDashboard[i].legend.options.display = true;
        pieChartDashboard[i].update();
      }
    }
  });
  /* ----------------------------------- Oculta el Legend de la grafica de PIE cuando el ancho de la ventana sea menos a 369 pixeles y pueda mostrarse bien la grafica ----------------------------------- */

    /**
     * Coloca la "," en los digitos colocado en el eje X de la grafica de barras, para separar los miles
     */
    function formatoMoneda(number) {
        var number1 = number.toString(), result = '', estado = true;
        if (parseInt(number1) < 0) {
            estado = false;
            number1 = parseInt(number1) * -1;
            number1 = number1.toString();
        }
        if (number1.indexOf(',') == -1) {
            while (number1.length > 3) {
                result = ',' + '' + number1.substr(number1.length - 3) + '' + result;
                number1 = number1.substring(0, number1.length - 3);
            }
            result = number1 + result;
            if (estado == false) {
                result = '-' + result;
            }
        }
        else {
            var pos = number1.indexOf(',');
            var numberInt = number1.substring(0, pos);
            var numberDec = number1.substring(pos, number1.length);
            while (numberInt.length > 3) {
                result = ',' + '' + numberInt.substr(numberInt.length - 3) + '' + result;
                numberInt = numberInt.substring(0, numberInt.length - 3);
            }
            result = numberInt + result + numberDec;
            if (estado == false) {
                result = '-' + result;
            }
        }
        return result;
    }

});
