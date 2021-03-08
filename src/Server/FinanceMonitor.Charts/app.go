package main

import (
	"crypto/tls"
	"fmt"
	"net/http"
	"net/url"

	"app/preview"
	"app/services/repository"

	"github.com/go-chi/chi/v5"
)

var sqlConnectionString = "Server=localhost:5434;Database=PriceMonitor;MultipleActiveResultSets=True;User Id=sa;Password=Pass@word;"

type msg string

func (m msg) ServeHTTP(resp http.ResponseWriter, req *http.Request) {

}
func main() {

	fmt.Println("Server is listening...")

	router := chi.NewRouter()
	router.Mount("/api/", preview.NewRouter())

	query := url.Values{}
	query.Add("database", "PriceMonitor")
	u := &url.URL{
		Scheme: "sqlserver",
		User:   url.UserPassword("sa", "Pass@word"),
		Host:   fmt.Sprintf("%s:%d", "localhost", 5434),
		// Path:  instance, // if connecting to an instance instead of a port
		RawQuery: query.Encode(),
	}
	err := repository.OpenConnection(sqlConnectionString, u)

	if err != nil {
		fmt.Println(err.Error())
	}

	server := http.Server{
		Addr:    ":8181",
		Handler: router,
		TLSConfig: &tls.Config{
			NextProtos: []string{"h2", "http/1.1"},
		},
	}

	server.ListenAndServeTLS("certs/localhost.crt", "certs/localhost.key")
}
