package chart

import (
	"app/services/repository"
	"bytes"
	"time"

	"github.com/wcharczuk/go-chart"
)

func GetHistoryYearlyImage(symbol string) ([]byte, error) {
	var yValues = []float64{}
	var xValues = []time.Time{}

	err, rows := repository.ReadSampledHistoryDataYearly(symbol)

	if err != nil {
		return nil, err
	}

	for _, row := range rows {
		yValues = append(yValues, row.Closed)
		xValues = append(xValues, row.DateTime)
	}

	graph := chart.Chart{
		Width:  500,
		Height: 100,
		XAxis: chart.XAxis{
			ValueFormatter: func(v interface{}) string {
				t := v.(time.Time)
				return t.Format(time.RFC3339)
			},
			Name: "Date",
		},
		Series: []chart.Series{

			chart.TimeSeries{
				Style: chart.Style{
					StrokeColor: chart.GetDefaultColor(0).WithAlpha(64),
					FillColor:   chart.GetDefaultColor(0).WithAlpha(64),
					Show:        true,
				},
				Name:    "SomeName",
				YAxis:   chart.YAxisPrimary,
				XValues: xValues,
				YValues: yValues,
			},
		},
	}

	buffer := bytes.NewBuffer([]byte{})
	err = graph.Render(chart.PNG, buffer)
	if err != nil {
		return nil, err
	}

	image := buffer.Bytes()

	return image, nil
}
