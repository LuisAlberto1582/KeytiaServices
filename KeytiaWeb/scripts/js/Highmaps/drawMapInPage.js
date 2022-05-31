$(document).ready(function () {

  var data = [
    {color:"#FBE599",name: "No Registradas",  data: [ ['mx-bc', 32],['mx-bs', 32],['mx-so', 32] ], states:{ hover:{ brightness :0 }}},
    {color: "#FBD85C",name: "No Registradas", data: [ ['mx-cm', 62],['mx-qr', 20],['mx-mx', 20],['mx-hg', 32] ],states:{ hover:{ brightness :0 }}},
    {color: "#F6CC48",name: "No Registradas", data: [ ['mx-si', 9], ['mx-ch', 9], ['mx-ve', 9],['mx-sl', 45] ],states:{ hover:{ brightness :0 }}},
    {color: "#F58426",name: "No Registradas", data: [ ['mx-ag', 18],['mx-ja', 20],['mx-mi', 18] ],states:{ hover:{ brightness :0 }}},
    {color: "#F56F00",name: "No Registradas", data: [ ['mx-oa', 18],['mx-pu', 18],['mx-gr', 24] ],states:{ hover:{ brightness :0 }}},
    {color: "#DE6500",name: "No Registradas", data: [ ['mx-za', 18],['mx-tl', 25],['mx-gj', 45] ],states:{ hover:{ brightness :0 }}},
    {color: "#DE4600",name: "No Registradas", data: [ ['mx-mo', 24],['mx-df', 24],['mx-dg', 12] ],states:{ hover:{ brightness :0 }}},
    {color: "#BA2E00",name: "No Registradas", data: [ ['mx-tb', 10],['mx-cs', 10],['mx-yu', 12] ],states:{ hover:{ brightness :0 }}},
    {color: "#7D1F00",name: "No Registradas", data: [ ['mx-cl', 32],['mx-na', 32],['mx-co', 12] ],states:{ hover:{ brightness :0 }}},
    {color: "#521400",name: "No Registradas", data: [ ['mx-qt', 24],['mx-nl', 45],['mx-tm', 12] ],states:{ hover:{ brightness :0 }}}
  ];


  /*Checar hasc de usa-all para obtener el id y colocarle el valor ["US.KY", 23]*/

  Highcharts.mapChart('placeMap', {
    chart: {
      map: 'countries/mx/mx-all'
      /*
       Tipos de Mapas

        Para utilizar el mapa de mexico:
          - map: countries/mx/mx-all

        Para utilizar el mapa de EUA:
          - countries/us/us-all
      */
    },
    legend: {
      enabled: true,
      padding: 12,
      itemMarginTop: 0,
      itemMarginBottom: 0,
      symbolRadius: 0,
      symbolHeight: 12,
      symbolWidth: 12,
      itemStyle: {
         font: 'Poppins, sans-serif',
         color: '#747474',
         fontSize: "10px",
         fontWeight: "500"
      },
      padding: 10
    },
    title: {
      text: ''
    },
    plotOptions: {
      map:{
        allAreas: false
      }
    },
    tooltip: {
      headerFormat: '',
      pointFormat: '<b>{point.value} %</b>',
      shape:"arrow",
      backgroundColor: "#535353",
      borderColor: "#535353",
      style:{
        color:"#FFFFFF",
        fontSize:"10px"
      }
    },
    exporting: {
      enabled: false
    },
    series: data
  });
})
