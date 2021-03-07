package preview

import (
	"bytes"
	"log"
	"net/http"
	"time"

	"github.com/wcharczuk/go-chart"

	"github.com/go-chi/chi/v5"

	repository "app/services"
)

func RenderYearChart(w http.ResponseWriter, r *http.Request) {
	symbol := chi.URLParam(r, "symbol")

	log.Println("Received " + symbol)

	err, rows := repository.ReadSampledHistoryDataYearly(symbol)

	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte("500 - Something bad happened!"))
		return
	}

	var yValues = []float64{}
	var xValues = []time.Time{}

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
		log.Fatal(err)
	}
	w.Header().Set("Content-Type", "image/png")
	w.Write(buffer.Bytes())
}

func NewRouter() http.Handler {
	r := chi.NewRouter()

	r.Get("/preview/{symbol}", RenderYearChart)

	return r
}
