import * as React from 'react'
import {useEffect} from 'react'
import {DetailsList, ProgressIndicator, SelectionMode} from "@fluentui/react";
import {Stock, StockClient, StockListItemDto} from "../../Api/ApiClients";
import {host} from "../../Api/Consts";
import { useHistory } from 'react-router-dom';

interface IProps {

}

export const Stocks = (props: IProps) => {

    const history = useHistory();

    const [isLoading, setLoading] = React.useState(true);
    const [stocks, setStocks] = React.useState<StockListItemDto[]>([])

    useEffect(() => {
        new StockClient({},host).list()
            .then(stocks => {
                setStocks(stocks);
            })
            .finally(() => setLoading(false))
    }, [])

    const onRowClick = (item: Stock, index?: number) => {
        if (!item)
            return;

        history.push(`${history.location.pathname}/${item.symbol}` )
    }

    if (isLoading) {
        return (<ProgressIndicator label={"Loading"}/>)
    }

    return (<><h1>Stocks</h1>
        <DetailsList
            items={stocks}
            selectionMode={SelectionMode.none}
            compact={true}
            onActiveItemChanged={onRowClick}
        ></DetailsList></>)
}