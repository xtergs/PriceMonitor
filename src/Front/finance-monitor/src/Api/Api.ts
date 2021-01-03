import {Stock} from "../Models/Stock";
import {GetManager} from "../App";

const host = "https://localhost:5001";
const identityHost = "https://localhost:5002"

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

    },

    register: async (email: string, password: string)=>{
        const response = await fetch(`${identityHost}/api/Account/Register/Register`,
            {
                method: "POST",
                body: JSON.stringify({
                    email,
                    password
                }),
                headers:{
                    'Content-Type': 'application/json'
                }
            });

        return response.json();
    },
    isAuthorized: async()=>{
        const user =  (await GetManager().getUser())
        return user && !user.expired
    }
}