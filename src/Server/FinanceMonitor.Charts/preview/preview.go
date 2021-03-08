package preview

import (
	"log"
	"net/http"

	"github.com/go-chi/chi/v5"

	"app/services/chart"
)

func RenderYearChart(w http.ResponseWriter, r *http.Request) {
	symbol := chi.URLParam(r, "symbol")

	log.Println("Received " + symbol)

	err, image := chart.RetrieveSymbolChart(symbol)

	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte("500 - Something bad happened!"))
		return
	}

	w.Header().Set("Content-Type", "image/png")
	w.Write(image)
}

func NewRouter() http.Handler {
	r := chi.NewRouter()

	r.Get("/preview/{symbol}", RenderYearChart)

	return r
}
