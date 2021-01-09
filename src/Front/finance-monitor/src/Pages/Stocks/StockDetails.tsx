import * as React from "react";
import {useEffect, useState} from "react";
import {Link, useParams} from "react-router-dom";
import {Stock, StockClient} from "../../Api/ApiClients";
import {ProgressIndicator, Stack} from "@fluentui/react";
import {host} from "../../Api/Consts";

interface IProps {

}

export const StockDetails = (props: IProps) => {
    const [loading, setLoading] = useState(true)
    const {symbol} = useParams<{ symbol: string }>()

    const [stock, setStock] = useState<Stock | null>(null)

    useEffect(() => {
        new StockClient({},host).stock(symbol)
            .then(x => {
                setStock(x);
            })
            .finally(() => setLoading(false))
    }, [])

    if (loading) {
        return <ProgressIndicator label={"Loading"}/>
    }

    if (!stock){
        return <span>Error rendering content</span>
    }

    return (<>
        <Stack>
        <h3>{stock.longName ?? stock.shortName}</h3>
        <table>
            <tr>
                <td>Symbol</td>
                <td>{stock.symbol}</td>
            </tr>
            <tr>
                <td>Currency:</td>
                <td>{stock.currency}</td>
            </tr>
            <tr>
                <td>Language</td>
                <td>{stock.language}</td>
            </tr>
            <tr>
                <td>Market</td>
                <td>{stock.market}</td>
            </tr>
            <tr>
                <td>QuoteType</td>
                <td>{stock.quoteType}</td>
            </tr>

            <tr>
                <td>ID</td>
                <td>{stock.id}</td>
            </tr>
        </table>

        <Link to={`${stock.symbol}/history`}>Show history</Link>
        <Link to={`${stock.symbol}/daily`}>Show daily</Link>
        </Stack>
    </>)
}