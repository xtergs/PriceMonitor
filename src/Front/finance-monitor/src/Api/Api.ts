import {Stock} from "../Models/Stock";

const host = "https://localhost:5001";

export const Api = {
    addStock: async (symbol: string,
                          price: number,
                          count: number) : Promise<Stock> => {
        const response = await fetch(`${host}/UserPricing`,
            {
                method: "POST",
                body: JSON.stringify({
                    symbol,
                    price,
                    count,
                    dateTime: new Date(),
                }),
                headers:{
                    'Content-Type': 'application/json'
                }
            });

        return response.json();

    }
}