package chartsCache

import (
	"context"
	"fmt"
	"time"

	"github.com/go-redis/redis/v8"
)

var ctx = context.Background()

func Get(symbol string) (error, []byte) {
	rdb := redis.NewClient(&redis.Options{
		Addr:     "localhost:6379",
		Password: "", // no password set
		DB:       0,  // use default DB
	})

	value, err := rdb.Get(ctx, symbol).Bytes()
	if err == redis.Nil {
		return nil, nil
	}
	if err != nil {
		return err, nil
	}

	return nil, value
}

func Save(symbol string, bytes []byte) error {
	rdb := redis.NewClient(&redis.Options{
		Addr:     "localhost:6379",
		Password: "", // no password set
		DB:       0,  // use default DB
	})

	val, err := rdb.Set(ctx, symbol, bytes, time.Hour).Result()

	fmt.Println(val)

	return err
}
