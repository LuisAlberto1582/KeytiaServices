function ExportToExcel(periodo) {

    var tab_text = "<table style='margin:20px;color:black;border-collapse: collapse;' cellspacing='0' cellpadding='0'><tr><td></td></tr><tr><td style='background-color: #696CAC; color: white;text-align:center'colspan='2'>Periodo:</td><td style='vertical-align:middle; text-align: center; padding: 17px; background-color:#EEEEEE;'>" + periodo +"</td></tr><tr><td></td></tr>";
    var textRange;
    var j = 0;
    tab = document.getElementById('TableFacturable');
    for (j = 0; j < tab.rows.length; j++) {
        tab_text = tab_text + "<tr>";
        tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
    }
    tab_text = tab_text + "</table>";
    tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "");//remove if u want links in your table
    tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
    tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params

    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");

    console.log(tab_text);
    //other browser not tested on IE 11
    // If Internet Explorer
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\\:11\./)) {
        jQuery('body').append(" <iframe id=\"iframeExport\" style=\"display:none\"></iframe>");
        iframeExport.document.open("txt/html", "replace");
        iframeExport.document.write(tab_text);
        iframeExport.document.close();
        iframeExport.focus();
        sa = iframeExport.document.execCommand("SaveAs", true, "RepFacturableNoFacturable-" + periodo + ".xls");
    }
    else {
        var link = document.createElement('a');
        document.body.appendChild(link); // Firefox requires the link to be in the body
        link.download = "RepFacturableNoFacturable-" + periodo+".xls";
        link.href = 'data:application/vnd.ms-excel;charset=utf-8,' + escape(tab_text);
        link.click();
        document.body.removeChild(link);
    }
}