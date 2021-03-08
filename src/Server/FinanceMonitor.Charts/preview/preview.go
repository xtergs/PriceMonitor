package preview

import (
	"log"
	"net/http"

	"github.com/go-chi/chi/v5"

	"app/services/chart"
	"app/services/chartsCache"
)

func RenderYearChart(w http.ResponseWriter, r *http.Request) {
	symbol := chi.URLParam(r, "symbol")

	log.Println("Received " + symbol)

	err, imageBytes := chartsCache.Get(symbol)
	if err != nil {
		log.Println(err.Error())
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte("500 - Something bad happened!"))
		return
	}
	if imageBytes == nil {
		log.Println("Image doesn't not cached")
	} else {
		w.Header().Set("Content-Type", "image/png")
		w.Write(imageBytes)
		return
	}

	image, err := chart.GetHistoryYearlyImage(symbol)

	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte("500 - Something bad happened!"))
		return
	}

	chartsCache.Save(symbol, image)

	w.Header().Set("Content-Type", "image/png")
	w.Write(image)
}

func NewRouter() http.Handler {
	r := chi.NewRouter()

	r.Get("/preview/{symbol}", RenderYearChart)

	return r
}
