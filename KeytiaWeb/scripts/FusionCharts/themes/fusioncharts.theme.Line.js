/*
 Ocean Theme v0.0.3
 FusionCharts JavaScript Library

 Copyright FusionCharts Technologies LLP
 License Information at <http://www.fusioncharts.com/license>
*/
FusionCharts.register("theme", {
    name: "Line",
    theme: {
        base: {
            chart: {
                paletteColors: "#0E2B63, #004F9F, #00B1EB, #EF7D00, #FFBB00, #50AF47, #AFCA0B, #5A328A, #E72582",
                labelDisplay: "auto",
                baseFontColor: "#333333",
                baseFont: "Poppins,Helvetica Neue,Arial",
                captionFontSize: "14",
                subcaptionFontSize: "14",
                subcaptionFontBold: "0",
                showBorder: "0",
                bgColor: "#ffffff",
                showShadow: "0",
                canvasBgColor: "#ffffff",
                showCanvasBorder: "0",
                useplotgradientcolor: "0",
                useRoundEdges: "0",
                showPlotBorder: "0",
                showAlternateHGridColor: "0",
                showAlternateVGridColor: "0",
                toolTipColor: "#ffffff",
                toolTipBorderThickness: "0",
                toolTipBgColor: "#000000",
                toolTipBgAlpha: "80",
                toolTipBorderRadius: "2",
                toolTipPadding: "5",
                legendBgAlpha: "0",
                legendBorderAlpha: "0",
                legendShadow: "0",
                legendItemFontSize: "10",
                legendItemFontColor: "#666666",
                legendCaptionFontSize: "9",
                divlineAlpha: "100",
                divlineColor: "#E8E8E8",
                divlineThickness: "1",
                divLineIsDashed: "0",
                divLineDashLen: "0",
                divLineGapLen: "1",
                scrollheight: "10",
                flatScrollBars: "1",
                scrollShowButtons: "0",
                scrollColor: "#cccccc",
                showHoverEffect: "1",
                valueFontSize: "10",
                showXAxisLine: "1",
                xAxisLineThickness: "1",
                xAxisLineColor: "#999999"
            },
            dataset: [{}],
            trendlines: [{}]
        },
        geo: {
            chart: {
                showLabels: "0",
                fillColor: "#E42177",
                showBorder: "1",
                borderColor: "#eeeeee",
                borderThickness: "1",
                borderAlpha: "50",
                entityFillhoverColor: "#696CAC",
                entityFillhoverAlpha: "80",
                connectorColor: "#cccccc",
                connectorThickness: "1.5",
                markerFillHoverAlpha: "90"
            }
        },
        pie2d: {
            chart: {
                paletteColors: "#E42177",//EDITAR
                placeValuesInside: "0",
                use3dlighting: "0",
                valueFontColor: "#333333",
                captionPadding: "15"
            },
            data: function (c, a, b) {
                a = window.Math;
                return {
                    alpha: 100 -
                        (50 < b ? a.round(100 / a.ceil(b / 10)) : 20) * a.floor(c / 10)
                }
            }
        },
        doughnut2d: {
            chart: {
                paletteColors: "#0E2B63, #004F9F, #00B1EB, #EF7D00, #FFBB00, #50AF47, #AFCA0B, #5A328A, #E72582",
                placeValuesInside: "0",
                use3dlighting: "0",
                valueFontColor: "#333333",
                centerLabelFontSize: "12",
                centerLabelBold: "1",
                centerLabelFontColor: "#333333",
                captionPadding: "15"
            },
            data: function (c, a, b) {
                a = window.Math;
                return {
                    alpha: 100 - (50 < b ? a.round(100 / a.ceil(b / 10)) : 20) * a.floor(c / 10)
                }
            }
        },
        doughnut3d: {
            chart: {
                paletteColors: "#0E2B63, #004F9F, #00B1EB, #EF7D00, #FFBB00, #50AF47, #AFCA0B, #5A328A, #E72582",
                placeValuesInside: "0",
                use3dlighting: "0",
                valueFontColor: "#333333",
                centerLabelFontSize: "12",
                centerLabelBold: "1",
                centerLabelFontColor: "#333333",
                captionPadding: "15",
            },
            data: function (c, a, b) {
                a = window.Math;
                return {
                    alpha: 100 - (50 < b ? a.round(100 / a.ceil(b / 10)) : 20) * a.floor(c / 10)
                }
            }
        },
        line: {
            chart: {
                //"drawAnchors": "0"
                lineThickness: "1",

                anchorBorderColor: "#e72582",//ROSA
                drawAnchors: "1",
                //anchorBorderThickness: "1"
                //,
                //anchoralpha: "100",
                anchorradius: "1",
                anchorBgColor: "#CCCCCC",


            }
        },
        spline: {
            chart: {
                lineThickness: "2"
            }
        },
        column2d: {
            chart: {
                paletteColors: "#0E2B63, #004F9F, #00B1EB, #EF7D00, #FFBB00, #50AF47, #AFCA0B, #5A328A, #E72582", //"#502C7F",//MORADO
                valueFontColor: "#ffffff",
                placeValuesInside: "1",
                rotateValues: "1"
            }
        },
        column3d: {
            chart: {
                paletteColors: "#A6C30B",/*VERDELIMA*/
                valueFontColor: "#ffffff",
                placeValuesInside: "1",
                rotateValues: "1"
            }
        },
        bar2d: {
            chart: {
                paletteColors: "#46A53E",/*VERDE*/
                valueFontColor: "#ffffff",
                placeValuesInside: "1"
            }
        },
        bar3d: {
            chart: {
                paletteColors: "#00B1EB",//CELESTE
                valueFontColor: "#ffffff",
                placeValuesInside: "1"
            }
        },
        area2d: {
            chart: {
                valueBgColor: "#ffffff",
                valueBgAlpha: "90",
                valueBorderPadding: "-2",
                valueBorderRadius: "2",



                plotFillAlpha: "15",
                vDivLineAlpha: "100",
                vDivlineColor: "#E8E8E8",
                vDivlineThickness: "1",
                vDivLineIsDashed: "0",
                vDivLineDashLen: "0",
                vDivLineGapLen: "1",


                showPlotBorder: "1",
                plotBorderColor: "#696CAC",
                plotBorderThickness: "1",
                plotBorderDashGap: "1",
                plotBorderAlpha: "100",


                //Punto en la grafica
                anchorBorderColor: "#5A5D94",
                drawAnchors: "1",
                anchorBorderThickness: "1",
                anchoralpha: "100",
                anchorradius: "5",
                anchorBgColor: "#CCCCCC",

            }
        },
        splinearea: {
            chart: {
                valueBgColor: "#ffffff",
                valueBgAlpha: "90",
                valueBorderPadding: "-2",
                valueBorderRadius: "2"
            }
        },
        mscolumn2d: {
            chart: {
                paletteColors: "#0E2B63, #004F9F, #00B1EB, #EF7D00, #FFBB00, #50AF47, #AFCA0B, #5A328A, #E72582",
                valueFontColor: "#ffffff",
                placeValuesInside: "1",
                rotateValues: "1"
            }
        },
        mscolumn3d: {
            chart: {
                paletteColors: "#0E2B63, #004F9F, #00B1EB, #EF7D00, #FFBB00, #50AF47, #AFCA0B, #5A328A, #E72582",
                showValues: "0"
            }
        },
        msstackedcolumn2dlinedy: {
            chart: {
                showValues: "0"
            }
        },
        stackedcolumn2d: {
            paletteColors: "#0E2B63, #004F9F, #00B1EB, #EF7D00, #FFBB00, #50AF47, #AFCA0B, #5A328A, #E72582",
            chart: {
                showValues: "0"
            }
        },
        stackedarea2d: {
            paletteColors: "#0E2B63, #004F9F, #00B1EB, #EF7D00, #FFBB00, #50AF47, #AFCA0B, #5A328A, #E72582",
            chart: {
                valueBgColor: "#ffffff",
                valueBgAlpha: "90",
                valueBorderPadding: "-2",
                valueBorderRadius: "2"
            }
        },
        stackedbar2d: {
            paletteColors: "#0E2B63, #004F9F, #00B1EB, #EF7D00, #FFBB00, #50AF47, #AFCA0B, #5A328A, #E72582",
            chart: {
                showValues: "0"
            }
        },
        msstackedcolumn2d: {
            chart: {
                showValues: "0"
            }
        },
        mscombi3d: {
            chart: {
                showValues: "0"
            }
        },
        mscombi2d: {
            chart: {
                showValues: "0"
            }
        },
        mscolumn3dlinedy: {
            chart: {
                showValues: "0"
            }
        },
        stackedcolumn3dline: {
            chart: {
                showValues: "0"
            }
        },
        stackedcolumn2dline: {
            chart: {
                showValues: "0"
            }
        },
        scrollstackedcolumn2d: {
            chart: {
                valueFontColor: "#ffffff"
            }
        },
        scrollcombi2d: {
            chart: {
                showValues: "0"
            }
        },
        scrollcombidy2d: {
            chart: {
                showValues: "0"
            }
        },
        logstackedcolumn2d: {
            chart: {
                showValues: "0"
            }
        },
        logmsline: {
            chart: {
                showValues: "0"
            }
        },
        logmscolumn2d: {
            chart: {
                showValues: "0"
            }
        },
        msstackedcombidy2d: {
            chart: {
                showValues: "0"
            }
        },
        scrollcolumn2d: {
            chart: {
                valueFontColor: "#ffffff",
                placeValuesInside: "1",
                rotateValues: "1"
            }
        },
        pareto2d: {
            chart: {
                paletteColors: "#00B1EB",//CELESTE
                showValues: "0"
            }
        },
        pareto3d: {
            chart: {
                paletteColors: "##00b1eb",//CELESTE
                showValues: "0"
            }
        },
        angulargauge: {
            chart: {
                pivotFillColor: "#ffffff",
                pivotRadius: "4",
                gaugeFillMix: "{light+0}",
                showTickValues: "1",
                majorTMHeight: "12",
                majorTMThickness: "2",
                majorTMColor: "#000000",
                minorTMNumber: "0",
                tickValueDistance: "10",
                valueFontSize: "24",
                valueFontBold: "1",
                gaugeInnerRadius: "50%",
                showHoverEffect: "0"
            },
            dials: {
                dial: [{
                    baseWidth: "10",
                    rearExtension: "7",
                    bgColor: "#000000",
                    bgAlpha: "100",
                    borderColor: "#666666",
                    bgHoverAlpha: "20"
                }]
            }
        },
        hlineargauge: {
            chart: {
                pointerFillColor: "#ffffff",
                gaugeFillMix: "{light+0}",
                showTickValues: "1",
                majorTMHeight: "3",
                majorTMColor: "#000000",
                minorTMNumber: "0",
                valueFontSize: "18",
                valueFontBold: "1"
            },
            pointers: {
                pointer: [{}]
            }
        },
        bubble: {
            chart: {
                use3dlighting: "0",
                showplotborder: "0",
                showYAxisLine: "1",
                yAxisLineThickness: "1",
                yAxisLineColor: "#999999",
                showAlternateHGridColor: "0",
                showAlternateVGridColor: "0"
            },
            categories: [{
                verticalLineDashed: "1",
                verticalLineDashLen: "1",
                verticalLineDashGap: "1",
                verticalLineThickness: "1",
                verticalLineColor: "#000000",
                category: [{}]
            }],
            vtrendlines: [{
                line: [{
                    alpha: "0"
                }]
            }]
        },
        scatter: {
            chart: {
                use3dlighting: "0",
                showYAxisLine: "1",
                yAxisLineThickness: "1",
                yAxisLineColor: "#999999",
                showAlternateHGridColor: "0",
                showAlternateVGridColor: "0"
            },
            categories: [{
                verticalLineDashed: "1",
                verticalLineDashLen: "1",
                verticalLineDashGap: "1",
                verticalLineThickness: "1",
                verticalLineColor: "#000000",
                category: [{}]
            }],
            vtrendlines: [{
                line: [{
                    alpha: "0"
                }]
            }]
        },
        boxandwhisker2d: {
            chart: {
                valueBgColor: "#ffffff",
                valueBgAlpha: "90",
                valueBorderPadding: "-2",
                valueBorderRadius: "2"
            }
        },
        thermometer: {
            chart: {
                gaugeFillColor: "##00b1eb"//CELESTE
            }
        },
        cylinder: {
            chart: {
                cylFillColor: "##00b1eb"//CELESTE
            }
        },
        sparkline: {
            chart: {
                linecolor: "#696CAC"
            }
        },
        sparkcolumn: {
            chart: {
                plotFillColor: "#696CAC"
            }
        },
        sparkwinloss: {
            chart: {
                winColor: "#004F9F",
                lossColor: "#0E2B63",
                drawColor: "#00B1EB",
                scoreLessColor: "#EF7D00"
            }
        },
        hbullet: {
            chart: {
                plotFillColor: "#00B1EB",
                targetColor: "#AFC1FF"
            }
        },
        vbullet: {
            chart: {
                plotFillColor: "#00B1EB",
                targetColor: "#AFC1FF"
            }
        }
    }
});