/*
 Ocean Theme v0.0.3
 FusionCharts JavaScript Library

 Copyright FusionCharts Technologies LLP
 License Information at <http://www.fusioncharts.com/license>
*/
FusionCharts.register("theme", {
    name: "KeytiaNewNaranja",
    theme: {
        base: {
            chart: {
                paletteColors: "#F79D51,#ffa75b,#ffb165,#ffbb6f,#ffc579,#ffcf83,#ffd98d,#ffe397,#ffeda1,#fff7ab,#ffffb5,#ffffbf",
                labelDisplay: "auto",
                baseFontColor: "#333333",
                baseFont: "Helvetica Neue,Arial",
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
                fillColor: "#F79D51",
                showBorder: "1",
                borderColor: "#eeeeee",
                borderThickness: "1",
                borderAlpha: "50",
                entityFillhoverColor: "#F79D51",
                entityFillhoverAlpha: "80",
                connectorColor: "#cccccc",
                connectorThickness: "1.5",
                markerFillHoverAlpha: "90"
            }
        },
        pie2d: {
            chart: {
                placeValuesInside: "0",
                use3dlighting: "0",
                valueFontColor: "#333333",
                captionPadding: "15"
            },
            data: function(c, a, b) {
                a = window.Math;
                return {
                    alpha: 100 -
                        (50 < b ? a.round(100 / a.ceil(b / 10)) : 20) * a.floor(c / 10)
                }
            }
        },
        doughnut2d: {
            chart: {
                placeValuesInside: "0",
                use3dlighting: "0",
                valueFontColor: "#333333",
                centerLabelFontSize: "12",
                centerLabelBold: "1",
                centerLabelFontColor: "#333333",
                captionPadding: "15"
            },
            data: function(c, a, b) {
                a = window.Math;
                return {
                    alpha: 100 - (50 < b ? a.round(100 / a.ceil(b / 10)) : 20) * a.floor(c / 10)
                }
            }
        },
		doughnut3d: {
            chart: {
                placeValuesInside: "0",
                use3dlighting: "0",
                valueFontColor: "#333333",
                centerLabelFontSize: "12",
                centerLabelBold: "1",
                centerLabelFontColor: "#333333",
                captionPadding: "15",
            },
            data: function(c, a, b) {
                a = window.Math;
                return {
                    alpha: 100 - (50 < b ? a.round(100 / a.ceil(b / 10)) : 20) * a.floor(c / 10)
                }
            }
        },
        line: {
            chart: {
                lineThickness: "2"
            }
        },
        spline: {
            chart: {
                lineThickness: "2"
            }
        },
        column2d: {
            chart: {
                paletteColors: "#F79D51",
                valueFontColor: "#ffffff",
                placeValuesInside: "1",
                rotateValues: "1"
            }
        },        
        column3d: {
            chart: {
                paletteColors: "#F79D51",
                valueFontColor: "#ffffff",
                placeValuesInside: "1",
                rotateValues: "1"
            }
        },
		bar2d: {
            chart: {
				paletteColors: "#F79D51",
                valueFontColor: "#ffffff",
                placeValuesInside: "1"
            }
        },
        bar3d: {
            chart: {
                paletteColors: "#F79D51",
                valueFontColor: "#ffffff",
                placeValuesInside: "1"
            }
        },
        area2d: {
            chart: {
                valueBgColor: "#ffffff",
                valueBgAlpha: "90",
                valueBorderPadding: "-2",
                valueBorderRadius: "2"
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
                valueFontColor: "#ffffff",
                placeValuesInside: "1",
                rotateValues: "1"
            }
        },
        mscolumn3d: {
            chart: {
                showValues: "0"
            }
        },
        msstackedcolumn2dlinedy: {
            chart: {
                showValues: "0"
            }
        },
        stackedcolumn2d: {
            chart: {
                showValues: "0"
            }
        },
        stackedarea2d: {
            chart: {
                valueBgColor: "#ffffff",
                valueBgAlpha: "90",
                valueBorderPadding: "-2",
                valueBorderRadius: "2"
            }
        },
        stackedbar2d: {
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
                paletteColors: "#F79D51",
                showValues: "0"
            }
        },
        pareto3d: {
            chart: {
                paletteColors: "#F79D51",
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
                gaugeFillColor: "#F79D51"
            }
        },
        cylinder: {
            chart: {
                cylFillColor: "#F79D51"
            }
        },
        sparkline: {
            chart: {
                linecolor: "#F79D51"
            }
        },
        sparkcolumn: {
            chart: {
                plotFillColor: "#F79D51"
            }
        },
        sparkwinloss: {
            chart: {
                winColor: "#F79D51",
                lossColor: "#ffa75b",
                drawColor: "#ffb165",
                scoreLessColor: "#ffbb6f"
            }
        },
        hbullet: {
            chart: {
                plotFillColor: "#F79D51",
                targetColor: "#ffa75b"
            }
        },
        vbullet: {
            chart: {
                plotFillColor: "#F79D51",
                targetColor: "#ffa75b"
            }
        }
    }
});