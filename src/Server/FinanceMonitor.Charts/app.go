package main

import (
	"crypto/tls"
	"fmt"
	"log"
	"net/http"
	"net/url"
	"os"
	"sync"
	"time"

	"app/preview"
	"app/services/chart"
	"app/services/repository"

	"github.com/go-chi/chi/v5"
	"github.com/gosidekick/goconfig"
	_ "github.com/gosidekick/goconfig/json"
	"github.com/streadway/amqp"
)

var sqlConnectionString = "Server=localhost:5434;Database=PriceMonitor;MultipleActiveResultSets=True;User Id=sa;Password=Pass@word;"

type msg string

func (m msg) ServeHTTP(resp http.ResponseWriter, req *http.Request) {

}

type sqlConfig struct {
	Host     string `json:"host" cfg:"host"`
	Database string `json:"database" cfg:"database"`
	User     string `json:"user" cfg:"user"`
	Password string `json:"password" cfg:"password"`
}

type mqConfig struct {
	ConnectionString string `json:"connectionString" cfg:"connectionString"`
}

type appConfig struct {
	SQL      sqlConfig `json:"SQL" cfg:"SQL"`
	RabbitMQ mqConfig  `json:"RabbitMQ" cfg:"RabbitMQ"`
}

func main() {

	fmt.Println("Server is listening...")

	config := appConfig{}
	goconfig.File = "config.json"
	err := goconfig.Parse(&config)
	if err != nil {
		log.Fatal(err.Error())
		return
	}

	router := chi.NewRouter()
	router.Mount("/api/", preview.NewRouter())

	initDb(config.SQL)
	initMQ(config.RabbitMQ)

	server := http.Server{
		Addr:    ":8181",
		Handler: router,
		TLSConfig: &tls.Config{
			NextProtos: []string{"h2", "http/1.1"},
		},
	}

	server.ListenAndServeTLS("certs/localhost.crt", "certs/localhost.key")
}

func handleError(err error) {
	if err != nil {
		log.Fatal(err.Error())
	}
}

func initDb(config sqlConfig) {
	query := url.Values{}
	query.Add("database", config.Database)
	u := &url.URL{
		Scheme:   "sqlserver",
		User:     url.UserPassword(config.User, config.Password),
		Host:     config.Host, // fmt.Sprintf("%s:%d", "localhost", 5434),
		RawQuery: query.Encode(),
	}
	err := repository.OpenConnection(sqlConnectionString, u)

	if err != nil {
		fmt.Println(err.Error())
	}
}

func initMQ(config mqConfig) {
	conn, err := amqp.Dial(config.ConnectionString)
	handleError(err)

	amqpChannel, err := conn.Channel()
	handleError(err)

	queue, err := amqpChannel.QueueDeclare("charts", true, false, false, false, nil)
	handleError(err)

	err = amqpChannel.Qos(1, 0, false)
	handleError(err)

	messageChannel, err := amqpChannel.Consume(queue.Name, "charts",
		false, false, false, false, nil)
	handleError(err)

	//stopChan := make(chan bool)

	go func() {
		log.Printf("Consumer ready, PID: %d", os.Getpid())
		for d := range messageChannel {
			log.Printf("Received a message: %s", d.Body)

			err, symbols := chart.GetSymbols()
			if err != nil {
				d.Reject(true)
			}

			var wg sync.WaitGroup
			start := time.Now()
			for _, v := range symbols {
				wg.Add(1)
				go prerenderImage(v, &wg)
			}
			wg.Wait()
			elapsed := time.Since(start)
			log.Printf("Elasped %s", elapsed)

			d.Ack(true)
		}
	}()

}

func prerenderImage(symbol string, wg *sync.WaitGroup) {
	defer wg.Done()
	chart.RetrieveSymbolChart(symbol)
}
