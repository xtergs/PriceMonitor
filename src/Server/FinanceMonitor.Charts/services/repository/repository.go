package repository

import (
	"context"
	"fmt"
	"log"
	"net/url"
	"time"

	_ "github.com/denisenkom/go-mssqldb"
	"github.com/jmoiron/sqlx"
)

type SampledHistoryDataYearly struct {
	DateTime time.Time `db:"DateTime"`
	Closed   float64   `db:"Closed"`
}

type Stock struct {
	Symbol string `db:"Symbol"`
}

var db *sqlx.DB

func OpenConnection(connectionString string, url *url.URL) error {
	var err error

	db, err = sqlx.Open("sqlserver", url.String())
	if err != nil {
		log.Println("Error creating connection pool: ", err.Error())
		return err
	}
	ctx := context.Background()
	err = db.PingContext(ctx)
	if err != nil {
		log.Println(err.Error())
		return err
	}
	fmt.Printf("Connected!\n")

	return nil
}

func ReadSampledHistoryDataYearly(symbol string) (error, []SampledHistoryDataYearly) {
	ctx := context.Background()

	err := db.PingContext(ctx)
	if err != nil {
		log.Println(err.Error())
		return err, []SampledHistoryDataYearly{}
	}

	tsql := fmt.Sprintf("exec dbo.GetStockFullYearPoints N'%s'", symbol)

	result := []SampledHistoryDataYearly{}
	err = db.Select(&result, tsql)
	if err != nil {
		log.Println(err.Error())
		return err, []SampledHistoryDataYearly{}
	}

	return nil, result
}

func GetStocks() (error, []Stock) {
	ctx := context.Background()

	err := db.PingContext(ctx)
	if err != nil {
		log.Println(err.Error())
		return err, nil
	}

	tsql := fmt.Sprintf("exec dbo.GetStocks")

	result := []Stock{}
	err = db.Select(&result, tsql)
	if err != nil {
		log.Println(err.Error())
		return err, nil
	}

	return nil, result
}
