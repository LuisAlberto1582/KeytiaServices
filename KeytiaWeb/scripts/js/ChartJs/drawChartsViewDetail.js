$(document).ready(function(){

  /* Configuraciones de los graficos (No mover) */
  var optionsLineChart       = { responsive: true, maintainAspectRatio: false, legend: { display: false }, scales: { yAxes: [{ ticks: { beginAtZero:true, fontFamily: "Poppins", fontSize:14  } } ], xAxes: [{ticks:{fontFamily: "Poppins", fontSize: 14, fontWeight: 100}}]}, tooltips: { callbacks: { title: function(tooltipItem,data){ return ""; }, label: function(tooltipItem, data) { return tooltipItem.yLabel; } }, displayColors: false, bodyFontStyle:"Poppins" },};
  var optionsColumnsChart    = { responsive: true, maintainAspectRatio: false, legend: { display: false }, scales: { xAxes: [{ categorySpacing: 60, ticks:{ fontFamily: "Poppins", fontSize:14  } }], yAxes: [{ type: "linear", display: true, position: "left", id: "y-axis-1", ticks:{ fontFamily: "Poppins", fontSize:14  } }] }, tooltips: { callbacks: { title: function(tooltipItem, data) { return ""; }, label: function(tooltipItem, data) { return tooltipItem.yLabel; } }, displayColors: false, bodyFontStyle:"Poppins" } };
  var optionsHorizontalChart = { responsive: true, maintainAspectRatio: false, legend: { display: false },scales:{xAxes:[{ticks:{callback:function(t,o,n){return"$"+formatoMoneda(t)},fontFamily:"Poppins",fontSize:14}}],yAxes:[{stacked:!0,ticks:{fontFamily:"Poppins",fontSize:14}}]},tooltips:{mode:"y",axis:"x",callbacks:{title:function(t,o){return""},label:function(t,o){return"$"+t.xLabel},afterLabel:function(t,o){return""}},displayColors:!1},layout:{padding:{left:15,top:0,bottom:0,right:50}}};
  var optionsAreaChart       = { responsive: true, maintainAspectRatio: false, legend: { display: false }, showLines: true, scales: { yAxes: [{ ticks: { beginAtZero:true, fontFamily: "Poppins", fontSize:14  } } ], xAxes: [{ticks:{fontFamily: "Poppins", fontSize: 14, fontWeight: 100}}]}, tooltips: { callbacks: { title: function(tooltipItem,data){ return ""; }, label: function(tooltipItem, data) { return tooltipItem.yLabel; } }, displayColors: false , bodyFontStyle:"Poppins"} };
  var optionsCircularChart   = { responsive: true, maintainAspectRatio: false, legend: { position: "right", labels: { padding: 25, boxWidth: 10, fontFamily: "Poppins", fontSize : 14, fontWeight : 100 } },tooltips: { displayColors: false } };


  var dataLineDetail = {
      labels: [["Enero" ,"2018"], ["Febrero", "2018"], ["Marzo","2018"], ["Abril","2018"], ["Mayo","2018"], ["Junio","2018"], ["Julio","2018"]],
      datasets: [{ data: [12, 19, 3, 5, 2, 3, 13], backgroundColor: ['rgba(204, 204, 204, 0.4)'], borderColor: [ '#56579C'], borderWidth: 1, lineTension:0, pointBackgroundColor: "#C0C1C0", pointRadius: 6, pointHoverRadius: 6 }]
  };

  var columnsChartDataDetail = {
      labels: [["Enero" ,"2018"], ["Febrero", "2018"], ["Marzo","2018"], ["Abril","2018"], ["Mayo","2018"], ["Junio","2018"], ["Julio","2018"],["Septiembre","2018"]],
      datasets: [{ label: "Test", data: [23,46,34,56,43,65,100,43], backgroundColor: ["rgba(245, 132, 38, 0.8)", "rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)"], hoverBackgroundColor: ["rgba(245, 132, 38, 0.8)", "rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)","rgba(245, 132, 38, 0.8)"] }]
  };

  var horizontalBarDataDetail = {
      labels: [ "Telcel", "Celular Local", "Servicio Medio", "L.D. Mundial", "Local Nacional", "Celular Nacional", "Llamadas 01800", "L.D. Nacional", "Enlace", "Entrada" ],
      datasets: [{ label: "Test", data: [12000,10000,93000,400000, 300000,30000,20000,10200,80000,50000], backgroundColor: ["rgba(105, 108, 172, 0.9)", "rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)"], hoverBackgroundColor: ["rgba(105, 108, 172, 0.9)", "rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)","rgba(105, 108, 172, 0.9)"] }]
  };

  var areaChartDataDetail = {
      labels: [["Enero" ,"2018"], ["Febrero", "2018"], ["Marzo","2018"], ["Abril","2018"], ["Mayo","2018"], ["Junio","2018"], ["Julio","2018"]],
      datasets: [{ data: [12, 19, 3, 5, 2, 3, 13], backgroundColor:['rgba(245, 132, 38, 0.8)'], borderColor:['transparent'], lineTension:0, pointHoverBorderColor:"transparent", pointBackgroundColor:"transparent" }]
  };

  var pieChartDataDetail = {
      labels: ['No identificada','Laboral','Personal', "Color 4" , "Color 5", "Color 6", "Color 7", "Color 8", "Color 9", "Color 10"],
      datasets: [{ data: [10, 10, 10, 10, 10, 10, 10, 10, 10, 10], backgroundColor: ["#FBE599", "#FBD85C","#F6CC48", "#F58426", "#F56F00", "#DE6500", "#DE4600", "#BA2E00", "#7D1F00", "#521400"], hoverBackgroundColor: ["#FBE599", "#FBD85C","#F6CC48", "#F58426", "#F56F00", "#DE6500", "#DE4600", "#BA2E00", "#7D1F00", "#521400"] }],
  };


  var lineChartDetail    = document.getElementById("lineDetail");
  var columnsChartDetail = document.getElementById("columnsDetail");
  var barrasChartDetail  = document.getElementById("barsDetail");
  var areaChartDetail    = document.getElementById("areaDetail");
  var pieChartDetail     = document.getElementById("pieDetail");

  

  var lineDetail = new Chart(lineChartDetail, {
      type: 'line',
      data: dataLineDetail,
      options:optionsLineChart
  });

  var columnsChartJS = new Chart(columnsChartDetail, {
      type: 'bar',
      data: columnsChartDataDetail,
      options: optionsColumnsChart
  });

  var horizontalBarChartJS = new Chart(barrasChartDetail, {
      type: 'horizontalBar',
      data: horizontalBarDataDetail,
      options: optionsHorizontalChart
  });

  var areaChartJS = new Chart(areaChartDetail, {
      type: 'line',
      data: areaChartDataDetail,
      options: optionsAreaChart
  });

  var pieChart = new Chart(pieChartDetail,{
      type: 'pie',
      data: pieChartDataDetail,
      options: optionsCircularChart
  });


  if($(window).width() <= 369){
    pieChart.legend.options.display = false;
    pieChart.update();
  }

  $(window).resize(function(){
    //Quitar la leyenda de los graficos de PIE
    if ($(this).width() <= 369) {
      pieChart.legend.options.display = false;
      pieChart.update();
    }else {
      pieChart.legend.options.display = true;
      pieChart.update();
    }
  });

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
